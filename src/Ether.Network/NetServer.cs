using System;
using System.Collections.Generic;
using System.Text;

namespace Ether.Network
{
    public abstract class NetServer<T> where T : NetConnection, new()
    {
        protected NetServer()
        {

        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}
