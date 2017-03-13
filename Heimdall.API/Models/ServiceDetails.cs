using System.Collections.Generic;

namespace Heimdall.API.Models
{
    public class ServiceDetails
    {
        public string Name { get; set; }
        public IEnumerable<string> Endpoints { get; set; }
    }
}
