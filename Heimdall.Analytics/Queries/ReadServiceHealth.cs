using System;
using MediatR;

namespace Heimdall.Analytics.Queries
{
    public class ReadServiceHealth : IRequest<Models.ServiceHealth>
    {
        public ReadServiceHealth(string serviceName, DateTime from, DateTime to)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentNullException(nameof(serviceName));
            if (from > to)
                throw new ArgumentOutOfRangeException($"invalid date range: {from} -> {to}");

            if (to > DateTime.UtcNow)
                to = DateTime.UtcNow;

            if(from.Year < 2017)
                from = to.AddDays(-7);

            this.ServiceName = serviceName;
            this.From = from;
            this.To = to;
        }

        public string ServiceName { get; private set; }
        public DateTime From { get; private set; }
        public DateTime To { get; private set; }
    }
}
