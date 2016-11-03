using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ether.Network
{
    public static class NetDelayer
    {
        private static List<NetDelayerAction> actions = new List<NetDelayerAction>();

        public static bool IsRunning { get; internal set; }


        static NetDelayer()
        {
            IsRunning = false;
        }

        public static void Register(string actionName, Action action, uint milliseconds)
        {
        }

        public static void Unregister(string actionName)
        {
        }

        public static void Run()
        {
            if (IsRunning)
                return;

            IsRunning = true;

            while (IsRunning)
            {

                Thread.Sleep(1);
            }   
        }
    }

    internal class NetDelayerAction
    {
    }
}
