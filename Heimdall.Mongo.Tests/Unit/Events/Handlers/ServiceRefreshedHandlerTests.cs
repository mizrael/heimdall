using Heimdall.Core.Events;
using Heimdall.Mongo.Commands.Handlers;
using Heimdall.Mongo.Events.Handlers;
using Heimdall.Mongo.Infrastructure;
using Heimdall.Mongo.Tests.Utils;
using LibCore.CQRS.Validation;
using LibCore.Web.Services;
using MediatR;
using Moq;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace Heimdall.Mongo.Tests.Unit.Events.Handlers
{
    public class ServiceRefreshedHandlerTests
    {
        [Fact]
        public void should_throw_ArgumentNullException_when_dbContext_null()
        { 
            Assert.Throws<ArgumentNullException>(() => new ServiceRefreshedHandler(null));
        }

        [Fact]
        public void should_throw_ArgumentNullException_when_event_null()
        {
            var mockDbContext = new Mock<IDbContext>();
            var sut = new ServiceRefreshedHandler(mockDbContext.Object);
            Assert.ThrowsAsync<ArgumentNullException>(() => sut.Handle(null));
        }

        [Fact]
        public void should_throw_ArgumentOutOfRangeException_when_service_does_not_exist()
        {
            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = true,
                Name = "lorem",
                Endpoints = Enumerable.Empty<Mongo.Infrastructure.Entities.ServiceEndpoint>()
            };
            var mockRepo = RepositoryUtils.MockRepository(service);

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockRepo.Object);

            var @event = new ServiceRefreshed(Guid.NewGuid());
            var sut = new ServiceRefreshedHandler(mockDbContext.Object);
            Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => sut.Handle(@event));
        }

        [Fact]
        public async Task should_create_trace_data_when_input_valid()
        {
            var startTicks = DateTime.UtcNow.Ticks;

            var service = new Mongo.Infrastructure.Entities.Service()
            {
                Id = Guid.NewGuid(),
                Active = true,
                Name = "lorem",
                Endpoints = Enumerable.Empty<Mongo.Infrastructure.Entities.ServiceEndpoint>()
            };
            var mockServicesRepo = RepositoryUtils.MockRepository(service);

            var mockEventsRepo = RepositoryUtils.MockRepository<Mongo.Infrastructure.Entities.TraceEvent>();

            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(db => db.Services).Returns(mockServicesRepo.Object);
            mockDbContext.Setup(db => db.TraceEvents).Returns(mockEventsRepo.Object);

            var @event = new ServiceRefreshed(service.Id);
            var sut = new ServiceRefreshedHandler(mockDbContext.Object);
            await sut.Handle(@event);

            mockEventsRepo.Verify(m => m.InsertOneAsync(It.Is<Mongo.Infrastructure.Entities.TraceEvent>(
                    te => te.Id != Guid.Empty &&
                    te.CreationDate >= startTicks &&
                    te.Name == Core.TraceEventNames.ServiceRefreshed &&
                    te.CustomData == service
                )
            ));
        }
    }
}
