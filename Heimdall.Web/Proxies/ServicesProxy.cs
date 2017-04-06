using Heimdall.Web.DTO;
using LibCore.Web.HTTP;
using LibCore.Web.Extensions;
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

        public async Task<ServiceDetails> ReadDetails(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var request = new RequestData("/services/" + name);
            var result = await _servicesApiClient.GetAsync<ServiceDetails>(request);
            return result;
        }

        public async Task<ServiceDetails> Refresh(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            
            var request = new RequestData("/services/refresh", name);
            
            var response = await _servicesApiClient.PostAsync(request);
            if (null == response)
                throw new System.Net.Http.HttpRequestException($"unable to perform POST request to '{request.Url}'");

            await response.AssertSuccessfulAsync();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ServiceDetails>(responseContent);
            return result;
        }
    }
}
