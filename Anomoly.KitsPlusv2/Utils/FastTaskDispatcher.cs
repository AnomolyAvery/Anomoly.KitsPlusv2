using SDG.Unturned;
using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;
using Action = System.Action;
using Logger = Rocket.Core.Logging.Logger;

namespace Anomoly.KitsPlusv2.Utils
{
    public class FastTaskDispatcher: MonoBehaviour
    {
        private static Lazy<FastTaskDispatcher> _instance = new Lazy<FastTaskDispatcher>(() =>
        {
            var obj = new GameObject("KitsPlusv2.TaskDispatcher");
            DontDestroyOnLoad(obj);

            return obj.AddComponent<FastTaskDispatcher>();
        });

        private ConcurrentQueue<Action> _queue = new ConcurrentQueue<Action>();

        public static void QueueOnMainThread(Action action)
        {
            if (Thread.CurrentThread.IsGameThread())
                action();
            else
                _instance.Value._queue.Enqueue(action);
        }

        private void FixedUpdate()
        {
            while (_queue.TryDequeue(out var action))
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Logger.LogError($"KitPlusv2.TaskDispatcher Error: {ex.Message}");
                    Logger.LogError(ex.StackTrace);
                }
            }
        }
    }
}
