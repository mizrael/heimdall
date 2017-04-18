﻿using MediatR;
using System;

namespace Heimdall.Core.Commands
{
    public class DeleteService : INotification
    {
        public DeleteService(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            this.Name = name;
        }

        public string Name { get; private set; }
    }
}