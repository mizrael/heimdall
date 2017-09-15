using System.Linq;

namespace Heimdall.Mongo.Infrastructure
{
    public static class MapperConfiguration
    {
        public static void Register(AutoMapper.IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<Entities.ServiceEndpoint, Heimdall.Core.Queries.Models.ServiceEndpoint>();

            cfg.CreateMap<Entities.Service, Heimdall.Core.Queries.Models.ServiceDetails>()
                .ForMember(e => e.Endpoints, mo =>
                {
                    mo.ResolveUsing((src, dest, destMember, resContext) =>
                    {
                        var activeEndpointsOnly = false;
                        if (resContext.Items.ContainsKey("activeEndpointsOnly"))
                        {
                            object value = null;
                            resContext.Items.TryGetValue("activeEndpointsOnly", out value);
                            activeEndpointsOnly = (value is bool && ((bool)value));
                        }

                        return ExtractEndpoints(src, activeEndpointsOnly);
                    });
                }).ForMember(e => e.BestEndpoint, mo =>
                {
                    mo.ResolveUsing((Entities.Service s) =>
                    {
                        return ExtractBestEndpoint(s);
                    });
                });

            cfg.CreateMap<Entities.Service, Heimdall.Core.Queries.Models.ServiceArchiveItem>()
                .ForMember(e => e.EndpointsCount, mo => {
                    mo.ResolveUsing((Entities.Service s) =>
                    {
                        var endpoints = ExtractEndpoints(s, false);
                        return endpoints.Count();
                    });
                })
                .ForMember(e => e.RoundtripTime, mo => {
                    mo.ResolveUsing((Entities.Service s) =>
                    {
                        var bestEndpoint = ExtractBestEndpoint(s);
                        return (null != bestEndpoint) ? bestEndpoint.RoundtripTime : long.MaxValue;
                    });
                });
        }

        private static Core.Queries.Models.ServiceEndpoint ExtractBestEndpoint(Entities.Service s)
        {
            var bestEndpoint = s.FindBestEndpoint();
            
            return (null != bestEndpoint) ? AutoMapper.Mapper.Map<Core.Queries.Models.ServiceEndpoint>(bestEndpoint) : null;
        }

        private static Core.Queries.Models.ServiceEndpoint[] ExtractEndpoints(Entities.Service src, bool activeEndpointsOnly)
        {
            var endpoints = activeEndpointsOnly ? src.FindActiveEndpoints() : src.Endpoints;
            endpoints = endpoints ?? Enumerable.Empty<Entities.ServiceEndpoint>();
            return endpoints.Select(se => AutoMapper.Mapper.Map<Core.Queries.Models.ServiceEndpoint>(se))
                                     .ToArray();
        }
    }
}
