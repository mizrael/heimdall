namespace Heimdall.API.Models
{
    public class UpdateEndpoint
    {
        public System.Guid EndpointId { get; set; }
        public string ServiceName { get; set; }
        public string Address { get; set; }
        public string Protocol { get; set; }
    }
}
