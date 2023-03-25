using Braintree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunNGoneBetter_Utility.BrainTree
{
    public interface IBrainTreeBridge
    {
        IBraintreeGateway CreateGateWay();
        IBraintreeGateway GetGateway();
    }
}
