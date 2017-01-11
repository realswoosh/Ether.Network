using Ether.Network.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Ether.Network
{
    /// <summary>
    /// Ether.Network action delayer.
    /// </summary>
    public static class NetDelayer
    {
        private static object syncActionsRoot = new object();
        private static ICollection<NetDelayerAction> actions;
        private static Thread delayerThread;

        /// <summary>
        /// Gets the running state of the NetDelayer.
        /// </summary>
        public static bool IsRunning { get; internal set; }

        /// <summary>
        /// Gets the actions count.
        /// </summary>
        public static int ActionCount { get; private set; }

        /// <summary>
        /// Initialize the static properties of the NetDelayer.
        /// </summary>
        static NetDelayer()
        {
            IsRunning = false;
            actions = new List<NetDelayerAction>();
        }

        /// <summary>
        /// Register an action starting now with no recursion.
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <returns>Action Id</returns>
        public static int Register(Action action)
        {
            return Register(action, DateTime.Now, 0, false);
        }

        /// <summary>
        /// Register an action starting at the specified time.
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="callTime">Time to execute the action</param>
        /// <returns>Action Id</returns>
        public static int Register(Action action, DateTime callTime)
        {
            return Register(action, callTime, 0, false);
        }

        /// <summary>
        /// Register an action starting at the speficied time.
        /// This action is recursive every specified milliseconds.
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="callTime">Time to execute the action</param>
        /// <param name="recursionTime">Recursion time in milliseconds</param>
        /// <returns>Action Id</returns>
        public static int Register(Action action, DateTime callTime, int recursionTime)
        {
            return Register(action, callTime, recursionTime, true);
        }

        /// <summary>
        /// Register an action.
        /// </summary>
        /// <param name="action">Action to execute</param>
        /// <param name="callTime">Time to execute the action</param>
        /// <param name="recursionTime">Recursion time in milliseconds</param>
        /// <param name="isRecursive">Is action recursive</param>
        /// <returns>Action Id</returns>
        private static int Register(Action action, DateTime callTime, int recursionTime, bool isRecursive)
        {
            var delayedAction = new NetDelayerAction(
                action,
                callTime,
                recursionTime,
                isRecursive);

            lock (syncActionsRoot)
            {
                actions.Add(delayedAction);
                ActionCount = actions.Count;
            }

            return delayedAction.Id;
        }

        /// <summary>
        /// Unregister an action.
        /// </summary>
        /// <param name="actionId">Action Id</param>
        public static void Unregister(int actionId)
        {
            lock (syncActionsRoot)
            {
                var action = actions.FirstOrDefault(x => x.Id == actionId);

                if (action != null)
                    actions.Remove(action);

                ActionCount = actions.Count;
            }
        }

        /// <summary>
        /// Run the delayer thread.
        /// </summary>
        private static void Run()
        {
            while (IsRunning)
            {
                var actionsReady = new List<NetDelayerAction>();

                lock (syncActionsRoot)
                {
                    actionsReady = actions.Where(x => x.CallTime < DateTime.Now).ToList();
                }

                if (actionsReady.Any())
                foreach (var action in actionsReady)
                {
                    action.Action();
                    if (action.IsRecursive)
                        action.CallTime = DateTime.Now.AddMilliseconds(action.RecursionTime);
                    else
                        Unregister(action.Id);
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// Starts the NetDelayer.
        /// </summary>
        public static void Start()
        {
            if (IsRunning)
                return;

            IsRunning = true;
            delayerThread = new Thread(Run);
            delayerThread.Start();
        }

        /// <summary>
        /// Stop the NetDelayer.
        /// </summary>
        public static void Stop()
        {
            IsRunning = false;

            if (delayerThread != null)
                delayerThread.Join();
            delayerThread = null;

            lock (syncActionsRoot)
                actions.Clear();

            ActionCount = 0;
        }
    }

    internal class NetDelayerAction
    {
        /// <summary>
        /// Gets or sets the NetDelayerAction Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the action to execute.
        /// </summary>
        public Action Action { get; set; }

        /// <summary>
        /// Gets or sets the time that the action will be executed.
        /// </summary>
        public DateTime CallTime { get; set; }

        /// <summary>
        /// Gets or sets the recursion time in milliseconds.
        /// </summary>
        public int RecursionTime { get; set; }

        /// <summary>
        /// Gets or sets if the action is recursive or not.
        /// </summary>
        public bool IsRecursive { get; set; }

        /// <summary>
        /// Creates a new NetDelayerAction instance.
        /// </summary>
        /// <param name="action"></param>
        public NetDelayerAction(Action action)
            : this(action, DateTime.Now, 0, false)
        {
        }

        /// <summary>
        /// Creates a new NetDelayerAction instance.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callTime"></param>
        public NetDelayerAction(Action action, DateTime callTime)
            : this(action, callTime, 0, false)
        {
        }

        /// <summary>
        /// Creates a new NetDelayerAction instance.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callTime"></param>
        /// <param name="recursionTime"></param>
        public NetDelayerAction(Action action, DateTime callTime, int recursionTime)
            : this(action, callTime, recursionTime, true)
        {
        }

        /// <summary>
        /// Creates a new NetDelayerAction instance.
        /// </summary>
        /// <param name="action"></param>
        /// <param name="callTime"></param>
        /// <param name="recursionTime"></param>
        /// <param name="isRecursive"></param>
        public NetDelayerAction(Action action, DateTime callTime, int recursionTime, bool isRecursive)
        {
            this.Id = Helper.GenerateUniqueId();
            this.Action = action;
            this.CallTime = callTime;
            this.RecursionTime = recursionTime;
            this.IsRecursive = isRecursive;
        }
    }
}
