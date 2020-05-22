using System;
using System.Collections.Generic;
using System.Text;

namespace ContactDetailsServiceA.BusinessInterfaces
{
    public interface IServiceBus
    {
        event EventHandler OnBusinessChange;
        void Close();
        void Send(string input);
        string Receive();
    }
}
