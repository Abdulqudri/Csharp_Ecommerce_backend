using Ecommerce.API.Dtos.Requests.Orders;
using Ecommerce.API.Dtos.Responses.Orders;
using Ecommerce.API.Helpers;

namespace Ecommerce.API.Services.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest request, string userId);
        Task<OrderResponse?> GetOrderByIdAsync(int id, string userId);
        Task<PaginatedResponse<OrderResponse>> GetUserOrdersAsync(string userId, int page = 1, int pageSize = 10);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status, string userId);
        Task<bool> CancelOrderAsync(int orderId, string userId);
        Task<OrderSummaryResponse> GetOrderSummaryAsync(string userId);
        Task<IEnumerable<OrderResponse>> GetOrdersByStatusAsync(string status);
    }

    public class OrderSummaryResponse
    {
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
        public int PendingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
    }
}