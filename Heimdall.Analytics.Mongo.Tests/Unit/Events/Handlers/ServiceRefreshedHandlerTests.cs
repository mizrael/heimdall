﻿using Heimdall.Core.Events;
using Heimdall.Analytics.Mongo.Events.Handlers;
using Heimdall.Mongo.Infrastructure;
using Heimdall.Mongo.Tests.Common.Utils;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Heimdall.Analytics.Mongo.Infrastructure;
using System.Linq.Expressions;

namespace Heimdall.Analytics.Mongo.Tests.Unit.Events.Handlers
{
    public class ServiceRefreshedHandlerTests
    {
        [Fact]
        public void should_throw_ArgumentNullException_when_dbContext_null()
        {   
            var mockAnalyticsDb = new Mock<IAnalyticsDbContext>();
            Assert.Throws<ArgumentNullException>(() => new ServiceRefreshedHandler(null, mockAnalyticsDb.Object));
        }

        [Fact]
        public void should_throw_ArgumentNullException_when_analyticsDbContext_null()
        {
            var mockDb = new Mock<IDbContext>();
            Assert.Throws<ArgumentNullException>(() => new ServiceRefreshedHandler(mockDb.Object, null));
        }

        [Fact]
        public void should_throw_ArgumentNullException_when_event_null()
        {
            var mockDbContext = new Mock<IDbContext>();
            var mockAnalyticsDb = new Mock<IAnalyticsDbContext>();
            var sut = new ServiceRefreshedHandler(mockDbContext.Object, mockAnalyticsDb.Object);
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(null));
        }

        [Fact]
        public void should_throw_ArgumentOutOfRangeException_when_service_does_not_exist()
        {
            var service = new Heimdall.Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = true,
                Name = "lorem",
                Endpoints = Enumerable.Empty<Heimdall.Mongo.Infrastructure.Entities.ServiceEndpoint>()
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);
            var mockAnalyticsDb = new Mock<IAnalyticsDbContext>();

            var @event = new ServiceRefreshed(Guid.NewGuid());
            var sut = new ServiceRefreshedHandler(mockDbContext.Object, mockAnalyticsDb.Object);
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.Handle(@event));
        }

        [Fact]
        public async Task should_create_service_health_data_when_not_found()
        {
            var now = DateTime.UtcNow;
            var start = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, DateTimeKind.Utc);

            var service = new Heimdall.Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = true,
                Name = "lorem",
                Endpoints = Enumerable.Empty<Heimdall.Mongo.Infrastructure.Entities.ServiceEndpoint>()
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockEventsRepo = RepositoryUtils.MockRepository<Heimdall.Mongo.Infrastructure.Entities.TraceEvent>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);
            mockDbContext.Setup(db => db.TraceEvents).Returns(mockEventsRepo.Object);

            var mockServicesHealthRepo = RepositoryUtils.MockRepository<Infrastructure.Entities.ServiceHealth>();
            var mockAnalyticsDb = new Mock<IAnalyticsDbContext>();
            mockAnalyticsDb.Setup(db => db.ServicesHealth).Returns(mockServicesHealthRepo.Object);

            var @event = new ServiceRefreshed(service.Id);
            var sut = new ServiceRefreshedHandler(mockDbContext.Object, mockAnalyticsDb.Object);
            await sut.Handle(@event);
            
            mockServicesHealthRepo.Verify(m => m.InsertOneAsync(It.Is<Infrastructure.Entities.ServiceHealth>(
                    sh => null != sh &&
                          sh.ServiceId == service.Id  &&
                          sh.TimestampMinute >= start.Ticks && 
                          null != sh.Details &&
                          sh.Details.Count() == 1
                )
            ), Times.Once);
        }

        [Fact]
        public async Task should_not_create_service_health_data_when_found()
        {
            var now = DateTime.UtcNow;
            var start = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, DateTimeKind.Utc);

            var service = new Heimdall.Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = true,
                Name = "lorem",
                Endpoints = Enumerable.Empty<Heimdall.Mongo.Infrastructure.Entities.ServiceEndpoint>()
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockEventsRepo = RepositoryUtils.MockRepository<Heimdall.Mongo.Infrastructure.Entities.TraceEvent>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);
            mockDbContext.Setup(db => db.TraceEvents).Returns(mockEventsRepo.Object);

            var serviceHealth = new Infrastructure.Entities.ServiceHealth()
            {
                ServiceId = service.Id,
                TimestampMinute = start.Ticks
            };
            var mockServicesHealthRepo = RepositoryUtils.MockRepository<Infrastructure.Entities.ServiceHealth>();
            mockServicesHealthRepo.Setup(r => r.FindOneAndUpdateAsync(It.IsAny<Expression<Func<Infrastructure.Entities.ServiceHealth, bool>>>(), 
                                                        It.IsAny<MongoDB.Driver.UpdateDefinition<Infrastructure.Entities.ServiceHealth>>()))
              .ReturnsAsync((Expression<Func<Infrastructure.Entities.ServiceHealth, bool>> filter, MongoDB.Driver.UpdateDefinition<Infrastructure.Entities.ServiceHealth> update) =>
              {
                  return serviceHealth;
              });

            var mockAnalyticsDb = new Mock<IAnalyticsDbContext>();
            mockAnalyticsDb.Setup(db => db.ServicesHealth).Returns(mockServicesHealthRepo.Object);

            var @event = new ServiceRefreshed(service.Id);
            var sut = new ServiceRefreshedHandler(mockDbContext.Object, mockAnalyticsDb.Object);
            await sut.Handle(@event);

            mockServicesHealthRepo.Verify(m => m.InsertOneAsync(It.Is<Infrastructure.Entities.ServiceHealth>(
                    sh => null != sh &&
                          sh.ServiceId == service.Id &&
                          sh.TimestampMinute >= start.Ticks &&
                          null != sh.Details &&
                          sh.Details.Count() == 1
                )
            ), Times.Never);
        }
    }
}
