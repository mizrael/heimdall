namespace Heimdall.API.Models
{
    public class UpdateEndpoint
    {
        public System.Guid ServiceId { get; set; }
        public System.Guid EndpointId { get; set; }
        public string Address { get; set; }
        public string Protocol { get; set; }
    }
}
