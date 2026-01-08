using Ecommerce.API.Data.Entities;

namespace Ecommerce.API.Repository.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(string userId);
        Task<Order?> GetOrderWithDetailsAsync(int id);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(string status);
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);
    }
}