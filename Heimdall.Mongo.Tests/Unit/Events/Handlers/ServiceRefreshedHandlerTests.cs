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
    }
}
