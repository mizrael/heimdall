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
                        var forceLoad = false;
                        if (resContext.Items.ContainsKey("forceLoad"))
                            forceLoad = (bool)resContext.Items["forceLoad"];

                        var endpoints = forceLoad ? src.Endpoints : src.GetActiveEndpoints();
                        return endpoints.Select(se => AutoMapper.Mapper.Map<Core.Queries.Models.ServiceEndpoint>(se))
                                                 .ToArray();
                    });
                }).ForMember(e => e.BestEndpoint, mo =>
                {
                    mo.ResolveUsing((Entities.Service s) =>
                    {
                        var availableEndpoints = s.GetActiveEndpoints();
                        if (null == availableEndpoints || !availableEndpoints.Any())
                            return null;
                        var bestEndpoint = availableEndpoints.OrderByDescending(e => e.RoundtripTime).First();
                        return AutoMapper.Mapper.Map<Core.Queries.Models.ServiceEndpoint>(bestEndpoint);
                    });
                });

            cfg.CreateMap<Entities.Service, Heimdall.Core.Queries.Models.ServiceArchiveItem>()
                .ForMember(e => e.EndpointsCount, mo => {
                    mo.ResolveUsing((Entities.Service s) =>
                    {
                        return (null != s.Endpoints) ? s.Endpoints.Count(se => se.Active) : 0;
                    });
                })
                .ForMember(e => e.RoundtripTime, mo => {
                    mo.ResolveUsing((Entities.Service s) =>
                    {
                        return (null != s.Endpoints && s.Endpoints.Any(se => se.Active)) ? s.Endpoints.Where(se => se.Active).Min(se => se.RoundtripTime) : long.MaxValue;
                    });
                });
        }
        
    }
}
