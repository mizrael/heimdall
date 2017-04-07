using LibCore.Mongo;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Heimdall.Mongo.Tests.Utils
{
    public static class RepositoryUtils
    {
        public static Mock<IRepository<TEntity>> MockRepository<TEntity>(TEntity entity = default(TEntity))
        {
            var entities = (null != entity) ? new[] { entity } : Enumerable.Empty<TEntity>();

            return RepositoryUtils.MockRepository(entities);
        }

        public static Mock<IRepository<TEntity>> MockRepository<TEntity>(IEnumerable<TEntity> entities)
        {
            entities = entities ?? Enumerable.Empty<TEntity>();

            var mockRepo = new Mock<IRepository<TEntity>>();
            mockRepo.Setup(r => r.FindOneAsync(It.IsAny<Expression<Func<TEntity, bool>>>()))
               .ReturnsAsync((Expression<Func<TEntity, bool>> filter) =>
               {
                   var s = entities.FirstOrDefault(filter.Compile());
                   return s;
               });

            mockRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<TEntity, bool>>>(), It.IsAny<PagingOptions>()))
                 .ReturnsAsync((Expression<Func<TEntity, bool>> filter, PagingOptions pagingOptions) =>
                 {
                     var s = entities.Where(filter.Compile());
                     return s;
                 });

            return mockRepo;
        }
    }
}
