using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Xunit;
using FluentAssertions;
using Heimdall.Mongo.Infrastructure.Entities;

namespace Heimdall.Mongo.Tests.Unit.Infrastructure
{
    public class MapperConfigurationTests
    {
        public MapperConfigurationTests()
        {
            AutoMapper.Mapper.Initialize(cfg =>
            {
                Mongo.Infrastructure.MapperConfiguration.Register(cfg);
            });
        }

        [Fact]
        public void ServiceDetails_mapping_should_not_return_best_active_endpoint_when_service_inactive()
        {
            var sut = new Service()
            {
                Active = false,
                Endpoints = new[]
                {
                    new ServiceEndpoint(){Active = true, RoundtripTime = 200, Url = "lorem"},
                }
            };

            var result = AutoMapper.Mapper.Map<Core.Queries.Models.ServiceDetails>(sut);
            result.BestEndpoint.Should().BeNull();
        }

        [Fact]
        public void ServiceDetails_mapping_should_return_best_active_endpoint_when_service_active()
        {
            var bestEndpointUrl = "ipsum";

            var sut = new Service()
            {
                Active = true,
                Endpoints = new[]
                {
                    new ServiceEndpoint(){Active = true, RoundtripTime = 200, Url = "lorem"},
                    new ServiceEndpoint(){Active = true, RoundtripTime = 10, Url = bestEndpointUrl},
                    new ServiceEndpoint(){Active = true, RoundtripTime = 400, Url = "dolor"},
                }
            };

            var result = AutoMapper.Mapper.Map<Core.Queries.Models.ServiceDetails>(sut);
            result.BestEndpoint.Should().NotBeNull();
            result.BestEndpoint.Url.ShouldBeEquivalentTo(bestEndpointUrl);
        }

        [Fact]
        public void ServiceDetails_mapping_should_return_only_active_endpoints()
        {
            var sut = new Service()
            {
                Active = true,
                Endpoints = new[]
                {
                    new ServiceEndpoint(){Active = false, RoundtripTime = 200, Url = "lorem"},
                    new ServiceEndpoint(){Active = true, RoundtripTime = 400, Url = "dolor"},
                }
            };

            var result = AutoMapper.Mapper.Map<Core.Queries.Models.ServiceDetails>(sut);
            result.Endpoints.Should().NotBeNullOrEmpty();
            result.Endpoints.Count().ShouldBeEquivalentTo(1);
            result.Endpoints.ElementAt(0).Active.Should().BeTrue();
            result.Endpoints.ElementAt(0).Url.ShouldBeEquivalentTo("dolor");
        }

        [Fact]
        public void ServiceDetails_mapping_should_return_all_endpoints_when_forceLoad_true()
        {
            var sut = new Service()
            {
                Active = true,
                Endpoints = new[]
                {
                    new ServiceEndpoint(){Active = false, RoundtripTime = 200, Url = "lorem"},
                    new ServiceEndpoint(){Active = true, RoundtripTime = 400, Url = "dolor"},
                }
            };

            var result = AutoMapper.Mapper.Map<Core.Queries.Models.ServiceDetails>(sut, opts => opts.Items["forceLoad"] = true);
            result.Endpoints.Should().NotBeNullOrEmpty();
            result.Endpoints.Count().ShouldBeEquivalentTo(2);
        }

        [Fact]
        public void ServiceDetails_mapping_should_not_return_all_endpoints_when_forceLoad_invalid()
        {
            var sut = new Service()
            {
                Active = true,
                Endpoints = new[]
                {
                    new ServiceEndpoint(){Active = false, RoundtripTime = 200, Url = "lorem"},
                    new ServiceEndpoint(){Active = true, RoundtripTime = 400, Url = "dolor"},
                }
            };

            var result = AutoMapper.Mapper.Map<Core.Queries.Models.ServiceDetails>(sut, opts => opts.Items["forceLoad"] = "asdasdasd");
            result.Endpoints.Should().NotBeNullOrEmpty();
            result.Endpoints.Count().ShouldBeEquivalentTo(1);
        }

        [Fact]
        public void ServiceDetails_mapping_should_not_return_all_endpoints_when_forceLoad_false()
        {
            var sut = new Service()
            {
                Active = true,
                Endpoints = new[]
                {
                    new ServiceEndpoint(){Active = false, RoundtripTime = 200, Url = "lorem"},
                    new ServiceEndpoint(){Active = true, RoundtripTime = 400, Url = "dolor"},
                }
            };

            var result = AutoMapper.Mapper.Map<Core.Queries.Models.ServiceDetails>(sut, opts => opts.Items["forceLoad"] = false);
            result.Endpoints.Should().NotBeNullOrEmpty();
            result.Endpoints.Count().ShouldBeEquivalentTo(1);
        }
    }
}
