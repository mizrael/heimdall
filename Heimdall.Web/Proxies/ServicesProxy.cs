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

        public async Task<IEnumerable<ServiceArchiveItem>> ReadAsync()
        {
            var request = new RequestData("/services/");
            var result = await _servicesApiClient.GetAsync<IEnumerable<ServiceArchiveItem>>(request);
            return result;
        }

        public async Task<ServiceDetails> ReadDetailsAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var request = new RequestData($"/services/{name}/force");
            var result = await _servicesApiClient.GetAsync<ServiceDetails>(request);
            return result;
        }

        public async Task CreateAsync(CreateService dto)
        {
            if (null == dto)
                throw new ArgumentNullException(nameof(dto));

            var request = new RequestData("/services", dto);

            var result = await _servicesApiClient.PostAsync(request);
            result.EnsureSuccessStatusCode();
        }

        public async Task<ServiceDetails> RefreshAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var request = new RequestData("/services/refresh", name);

            var result = await _servicesApiClient.PostAsync<ServiceDetails>(request);
            return result;
        }

        public async Task DeleteAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var request = new RequestData("/services/", name);

            var response = await _servicesApiClient.DeleteAsync(request);
            if (null == response)
                throw new System.Net.Http.HttpRequestException($"unable to perform DELETE request to '{request.Url}'");

            await response.AssertSuccessfulAsync();
        }
    }
}
