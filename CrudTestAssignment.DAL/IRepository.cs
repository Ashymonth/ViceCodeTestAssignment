using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CrudTestAssignment.DAL.Models;

namespace CrudTestAssignment.DAL
{
    public interface IRepository
    {
        Task<User> CreateAsync(User user, CancellationToken cancellationToken);

        Task<User> GetByNameAsync(string userName, CancellationToken cancellationToken);

        Task<User> GetByIdAsync(int userId, CancellationToken cancellationToken);

        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken);

        Task UpdateAsync(int userId, string name, CancellationToken cancellationToken);

        Task DeleteAsync(int userId, CancellationToken cancellationToken);
       
    }
}