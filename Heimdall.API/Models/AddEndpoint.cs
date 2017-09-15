namespace Heimdall.API.Models
{
    public class AddEndpoint
    {
        public System.Guid ServiceId { get; set; }
        public string Address { get; set; }
        public string Protocol { get; set; }
    }
}
