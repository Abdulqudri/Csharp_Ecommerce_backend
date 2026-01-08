using AutoMapper;
using Ecommerce.API.Constants;
using Ecommerce.API.Data.Entities;
using Ecommerce.API.Dtos.Requests.Orders;
using Ecommerce.API.Dtos.Responses.Orders;
using Ecommerce.API.Helpers;
using Ecommerce.API.Repository.Interfaces;
using Ecommerce.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.API.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IGenericRepository<Address> _addressRepository;
        private readonly IGenericRepository<OrderItem> _orderItemRepository;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IGenericRepository<Address> addressRepository,
            IGenericRepository<OrderItem> orderItemRepository,
            IMapper mapper)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _addressRepository = addressRepository;
            _orderItemRepository = orderItemRepository;
            _mapper = mapper;
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, string userId)
        {
            // Validate addresses if provided
            if (request.ShippingAddressId.HasValue)
            {
                var shippingAddress = await _addressRepository.GetByIdAsync(request.ShippingAddressId.Value);
                if (shippingAddress == null || shippingAddress.UserId != userId)
                    throw new ArgumentException("Invalid shipping address");
            }

            if (request.BillingAddressId.HasValue)
            {
                var billingAddress = await _addressRepository.GetByIdAsync(request.BillingAddressId.Value);
                if (billingAddress == null || billingAddress.UserId != userId)
                    throw new ArgumentException("Invalid billing address");
            }

            // Validate products and calculate total
            decimal totalAmount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in request.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null || !product.IsActive)
                    throw new ArgumentException($"Product {item.ProductId} not found or inactive");

                if (product.StockQuantity < item.Quantity)
                    throw new ArgumentException($"Insufficient stock for product {product.Name}");

                var unitPrice = product.DiscountPrice ?? product.Price;
                var itemTotal = unitPrice * item.Quantity;

                orderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = itemTotal
                });

                totalAmount += itemTotal;
            }

            // Create order
            var order = new Order
            {
                UserId = userId,
                TotalAmount = totalAmount,
                Status = OrderStatus.Pending,
                Notes = request.Notes,
                ShippingAddressId = request.ShippingAddressId,
                BillingAddressId = request.BillingAddressId,
                CreatedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            // Save order
            var createdOrder = await _orderRepository.AddAsync(order);

            // Update product stock
            foreach (var item in orderItems)
            {
                await _productRepository.UpdateStockAsync(item.ProductId, -item.Quantity);
            }

            return _mapper.Map<OrderResponse>(createdOrder);
        }

        public async Task<OrderResponse?> GetOrderByIdAsync(int id, string userId)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(id);
            if (order == null || (order.UserId != userId && !IsAdmin(userId)))
                return null;

            return _mapper.Map<OrderResponse>(order);
        }

        public async Task<PaginatedResponse<OrderResponse>> GetUserOrdersAsync(string userId, int page = 1, int pageSize = 10)
        {
            page = Math.Max(1, page);
            pageSize = Math.Clamp(pageSize, 1, AppConstants.MaxPageSize);

            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);
            var totalCount = orders.Count();

            var pagedOrders = orders
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var orderResponses = _mapper.Map<List<OrderResponse>>(pagedOrders);

            return new PaginatedResponse<OrderResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = orderResponses
            };
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, string userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return false;

            // Check permissions
            if (order.UserId != userId && !IsAdmin(userId))
                return false;

            // Validate status transition
            if (!IsValidStatusTransition(order.Status, status))
                throw new ArgumentException($"Invalid status transition from {order.Status} to {status}");

            order.Status = status;
            order.UpdateAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);
            return true;
        }

        public async Task<bool> CancelOrderAsync(int orderId, string userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                return false;

            if (order.UserId != userId && !IsAdmin(userId))
                return false;

            if (order.Status != OrderStatus.Pending)
                throw new ArgumentException($"Cannot cancel order with status {order.Status}");

            order.Status = OrderStatus.Cancelled;
            order.UpdateAt = DateTime.UtcNow;

            // Restore product stock
            var orderItems = await _orderItemRepository.FindAsync(oi => oi.OrderId == orderId);
            foreach (var item in orderItems)
            {
                await _productRepository.UpdateStockAsync(item.ProductId, item.Quantity);
            }

            await _orderRepository.UpdateAsync(order);
            return true;
        }

        public async Task<OrderSummaryResponse> GetOrderSummaryAsync(string userId)
        {
            var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

            return new OrderSummaryResponse
            {
                TotalOrders = orders.Count(),
                TotalSpent = orders.Where(o => o.Status == OrderStatus.Delivered).Sum(o => o.TotalAmount),
                PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending),
                DeliveredOrders = orders.Count(o => o.Status == OrderStatus.Delivered),
                CancelledOrders = orders.Count(o => o.Status == OrderStatus.Cancelled)
            };
        }

        public async Task<IEnumerable<OrderResponse>> GetOrdersByStatusAsync(string status)
        {
            var orders = await _orderRepository.GetOrdersByStatusAsync(status);
            return _mapper.Map<IEnumerable<OrderResponse>>(orders);
        }

        private bool IsAdmin(string userId)
        {
            // In a real implementation, you would check the user's roles
            // For now, we'll implement this in the controller using Authorize attribute
            return false;
        }

        private bool IsValidStatusTransition(string currentStatus, string newStatus)
        {
            var validTransitions = new Dictionary<string, List<string>>
            {
                [OrderStatus.Pending] = new List<string> { OrderStatus.Processing, OrderStatus.Cancelled },
                [OrderStatus.Processing] = new List<string> { OrderStatus.Shipped, OrderStatus.Cancelled },
                [OrderStatus.Shipped] = new List<string> { OrderStatus.Delivered, OrderStatus.Refunded },
                [OrderStatus.Delivered] = new List<string> { OrderStatus.Refunded },
                [OrderStatus.Cancelled] = new List<string> { },
                [OrderStatus.Refunded] = new List<string> { }
            };

            return validTransitions.ContainsKey(currentStatus) && 
                   validTransitions[currentStatus].Contains(newStatus);
        }
    }
}