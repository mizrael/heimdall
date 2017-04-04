using Heimdall.Mongo.Infrastructure.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using FluentAssertions;

namespace Heimdall.Mongo.Tests.Unit.Infrastructure.Entities
{
    public class ServiceTests
    {
        [Fact]
        public void GetActiveEndpoints_should_return_Empty_if_service_not_active()
        {
            var sut = new Service()
            {
                Active = false,
                Endpoints = new[]
                {
                    new ServiceEndpoint()
                }
            };
            var result = sut.GetActiveEndpoints();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetActiveEndpoints_should_return_Empty_if_inactive_service_has_null_endpoints()
        {
            var sut = new Service()
            {
                Active = false,
                Endpoints = null
            };
            var result = sut.GetActiveEndpoints();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetActiveEndpoints_should_return_Empty_if_active_service_has_null_endpoints()
        {
            var sut = new Service()
            {
                Active = true,
                Endpoints = null
            };
            var result = sut.GetActiveEndpoints();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetActiveEndpoints_should_return_Empty_if_inactive_service_has_empty_endpoints()
        {
            var sut = new Service()
            {
                Active = false,
                Endpoints = Enumerable.Empty<ServiceEndpoint>()
            };
            var result = sut.GetActiveEndpoints();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetActiveEndpoints_should_return_Empty_if_active_service_has_empty_endpoints()
        {
            var sut = new Service()
            {
                Active = true,
                Endpoints = Enumerable.Empty<ServiceEndpoint>()
            };
            var result = sut.GetActiveEndpoints();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetActiveEndpoints_should_return_Empty_if_active_service_has_no_active_endpoints()
        {
            var sut = new Service()
            {
                Active = true,
                Endpoints = new[]
                {
                    new ServiceEndpoint(){Active = false}
                }
            };
            var result = sut.GetActiveEndpoints();
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void GetActiveEndpoints_should_return_active_endpoints_only()
        {
            var sut = new Service()
            {
                Active = true,
                Endpoints = new[]
                {
                     new ServiceEndpoint(){Active = false},
                     new ServiceEndpoint(){Active = true},
                     new ServiceEndpoint(){Active = true},
                }
            };
            var result = sut.GetActiveEndpoints();
            result.Should().NotBeNullOrEmpty();
            result.Count().ShouldBeEquivalentTo(2);
        }
    }
}
