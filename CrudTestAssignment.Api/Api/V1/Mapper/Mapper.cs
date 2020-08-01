
using System;
using System.Collections.Generic;
using System.Linq;
using CrudTestAssignment.Api.Api.V1.Models;
using CrudTestAssignment.DAL.Models;

namespace CrudTestAssignment.Api.Api.V1.Mapper
{
    public static class Mapper
    {
        public static UserEntity MapToEntity(UserModel user)
        {
            return new UserEntity()
            {
                Id = user.Id,
                Name = user.Name,
                CreatedDate = user.CreatedDate
            };
        }

        public static UserModel MapToModel(UserEntity entity)
        {
            return new UserModel()
            {
                Id = entity.Id,
                Name = entity.Name,
                CreatedDate = entity.CreatedDate
            };
        }

        public static IEnumerable<UserModel> MapToModels(IEnumerable<UserEntity> entities)
        {
            var buffer = new List<UserModel>(entities.Count());
            foreach (var entity in entities)
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                buffer.Add(MapToModel(entity));
            }

            return buffer;
        }
    }
}
