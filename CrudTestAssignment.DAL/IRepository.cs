using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CrudTestAssignment.DAL.Models;

namespace CrudTestAssignment.DAL
{
    public interface IRepository
    {
        Task<UserEntity> CreateAsync(UserEntity user, CancellationToken cancellationToken);

        Task<UserEntity> GetByNameAsync(string userName, CancellationToken cancellationToken);

        Task<UserEntity> GetByIdAsync(int userId, CancellationToken cancellationToken);

        Task<IEnumerable<UserEntity>> GetAllAsync(CancellationToken cancellationToken);

        Task UpdateAsync(int userId, string name, CancellationToken cancellationToken);

        Task DeleteAsync(int userId, CancellationToken cancellationToken);
       
    }
}