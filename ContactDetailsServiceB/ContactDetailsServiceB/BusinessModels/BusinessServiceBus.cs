using System;
using System.Collections.Generic;
using System.Text;
using ContactDetailsServiceB.BusinessInterface;
using ContactDetailsServiceB.DataAccessLayer.ServiceBus.RPC_ContactDetails;

namespace ContactDetailsServiceB.BusinessModels
{
    public class BusinessServiceBus : IServiceBus
    {
        public event EventHandler OnBusinessChange;
        private enum DataModel
        {
            NONE = 0,
            RPCCLIENT = 1,
            RPCSERVER = 2,
            WORKSENDER = 3,
            WORKRECEIVER = 4
        }
        RpcBase _rpcC = null;
        RpcBase _rpcS = null;

        private DataModel _dataModelStateHandler;

        public BusinessServiceBus(int dataAccessModel)
        {
            switch (dataAccessModel)
            {
                case 1:
                    _rpcC = new RpcClient_ContactDetails();
                    _dataModelStateHandler = DataModel.RPCCLIENT;
                    ((RpcClient_ContactDetails)_rpcC).OnDataChange += new EventHandler(OnDataChange);
                    break;
                case 2:
                    _rpcS = new RpcServer_ContactDetails();
                    _dataModelStateHandler = DataModel.RPCSERVER;
                    ((RpcServer_ContactDetails)_rpcS).OnDataChange += new EventHandler(OnDataChange);
                    break;
                default:
                    break;
            }
        }

        public void Close()
        {
            switch (_dataModelStateHandler)
            {
                case DataModel.RPCCLIENT:
                    _rpcC.Close();
                    break;
                case DataModel.RPCSERVER:
                    _rpcS.Close();
                    break;
                default:
                    break;
            }
        }

        public void Send(string input)
        {
            switch (_dataModelStateHandler)
            {
                case DataModel.RPCCLIENT:
                    _rpcC.Send(input); 
                    break;
                case DataModel.RPCSERVER:
                    _rpcS.Send(input);
                    break;
                default:
                    break;
            }
        }

        public string Receive()
        {
            switch (_dataModelStateHandler)
            {
                case DataModel.RPCCLIENT:
                    return ((RpcClient_ContactDetails)_rpcC).GetResponse();
                case DataModel.RPCSERVER:
                return ((RpcServer_ContactDetails)_rpcS).Response;
                default:
                    return null;
            }
        }

        public void OnDataChange(object sender, EventArgs ea)
        {
            OnBusinessChange?.Invoke(this, ea);

        }

        public override string ToString()
        {
            switch (_dataModelStateHandler)
            {
                case DataModel.RPCCLIENT:
                    return _rpcC.ToString();
                case DataModel.RPCSERVER:
                    return _rpcS.ToString();
                default:
                    return "DataModel is empty";
            }
        }
    }
}
