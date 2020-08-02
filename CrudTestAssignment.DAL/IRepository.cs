using CrudTestAssignment.DAL.Entities;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CrudTestAssignment.DAL
{
    public interface IRepository
    {
        ValueTask<UserEntity> CreateAsync(UserEntity user, CancellationToken cancellationToken);

        ValueTask<UserEntity> GetByNameAsync(string userName, CancellationToken cancellationToken);

        ValueTask<UserEntity> GetByIdAsync(int userId, CancellationToken cancellationToken);

        ValueTask<IEnumerable<UserEntity>> GetAllAsync(CancellationToken cancellationToken);

        ValueTask UpdateAsync(int userId, string name, CancellationToken cancellationToken);

        ValueTask DeleteAsync(int userId, CancellationToken cancellationToken);
    }
}