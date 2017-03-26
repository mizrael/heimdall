using Heimdall.Web.DTO;
using LibCore.Web.HTTP;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Heimdall.Web.Proxies
{

    public class ServicesProxy : IServicesProxy
    {
        private readonly IApiClient _servicesApiClient;

        public ServicesProxy(IApiClient apiClient)
        {
            _servicesApiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        }

        public async Task<IEnumerable<ServiceArchiveItem>> Read()
        {
            var request = new RequestData("/services/");
            var result = await _servicesApiClient.GetAsync<IEnumerable<ServiceArchiveItem>>(request);
            return result;
        }
    }
}
