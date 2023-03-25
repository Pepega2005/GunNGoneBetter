using Braintree;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunNGoneBetter_Utility.BrainTree
{
    public class BrainTreeBridge : IBrainTreeBridge
    {
        public SettingsBrainTree SettingsBrainTree { get; set; }

        private IBraintreeGateway gateway;

        public BrainTreeBridge(IOptions<SettingsBrainTree> options)
        {
            SettingsBrainTree = options.Value;
        }

        public IBraintreeGateway CreateGateWay()
        {
            return new BraintreeGateway(SettingsBrainTree.Enviroment,
                SettingsBrainTree.MerchantId, SettingsBrainTree.PublicKey, SettingsBrainTree.PrivateKey);
        }

        public IBraintreeGateway GetGateway()
        {
            if (gateway == null)
            {
                gateway = CreateGateWay();
            }
            return gateway;
        }
    }
}
