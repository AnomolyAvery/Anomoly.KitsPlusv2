using Cysharp.Threading.Tasks;
using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using System;
using System.Reflection;
using System.Threading;
using UnityEngine.LowLevel;

namespace Anomoly.KitsPlusv2
{
    public class KitsPlusPlugin: RocketPlugin
    {
        public static KitsPlusPlugin Instance { get; private set; }

        protected override void Load()
        {
            base.Load();

            if (!IsInitialized())
            {
                var unitySynchronizationContextField = typeof(PlayerLoopHelper).GetField("unitySynchronizationContext", BindingFlags.Static | BindingFlags.NonPublic);

                unitySynchronizationContextField.SetValue(null, SynchronizationContext.Current);

                var mainThreadIdField =
                    typeof(PlayerLoopHelper).GetField("mainThreadId", BindingFlags.Static | BindingFlags.NonPublic) ?? throw new Exception("Could not find PlayerLoopHelper.mainThreadId field");
                mainThreadIdField.SetValue(null, Thread.CurrentThread.ManagedThreadId);

                var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
                PlayerLoopHelper.Initialize(ref playerLoop);
            }

            Instance = this;
        }

        protected override void Unload()
        {
            base.Unload();

            Instance = null;
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            {"","" }
        };

        private static bool IsInitialized()
        {
            var unitySyncContext = typeof(PlayerLoopHelper).GetField("unitySynchronizationContext", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);
            var mainThreadId = typeof(PlayerLoopHelper).GetField("mainThreadId", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);

            return unitySyncContext != null && mainThreadId != null && (int)mainThreadId == Thread.CurrentThread.ManagedThreadId;
        }

    }
}
