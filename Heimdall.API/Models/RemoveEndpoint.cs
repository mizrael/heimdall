namespace Heimdall.API.Models
{
    public class RemoveEndpoint
    {
        public string ServiceName { get; set; }
        public string Address { get; set; }
        public string Protocol { get; set; }
    }
}
