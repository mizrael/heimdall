﻿using System.Linq;

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
                    mo.ResolveUsing((Entities.Service s) =>
                    {
                        if (!s.Active || null == s.Endpoints)
                            return Enumerable.Empty<Core.Queries.Models.ServiceEndpoint>();

                        var availableEndpoints = s.Endpoints.Where(es => es.Active).ToArray();
                        if (!availableEndpoints.Any())
                            return Enumerable.Empty<Core.Queries.Models.ServiceEndpoint>();

                        return availableEndpoints.Select(se => AutoMapper.Mapper.Map<Core.Queries.Models.ServiceEndpoint>(se))
                                                 .ToArray();
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
