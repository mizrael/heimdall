using MediatR;
using System;


namespace Heimdall.Core.Commands
{

    public class RefreshServiceStatus : INotification
    {
        public RefreshServiceStatus(string name, int timeout)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            this.Name = name;

            this.Timeout = timeout;
        }

        public string Name { get; private set; }
        public int Timeout { get; private set; }
    }
    
}
