using System;
using System.Collections.Generic;
using System.Text;

namespace Heimdall.Core.Queries.Models
{
    public class ServiceArchiveItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}
