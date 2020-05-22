using System;
using System.Collections.Generic;
using System.Text;

namespace ContactDetailsServiceB.BusinessInterface
{
    public interface IServiceBus
    {
        event EventHandler OnBusinessChange;
        void Close();
        void Send(string input);
        string Receive();
    }
}
