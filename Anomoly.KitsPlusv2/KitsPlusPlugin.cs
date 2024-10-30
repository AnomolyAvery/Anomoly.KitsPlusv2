﻿using Anomoly.KitsPlusv2.Managers;
using Anomoly.KitsPlusv2.Repositories;
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
    public class KitsPlusPlugin: RocketPlugin<KitsPlusConfiguration>
    {
        public static KitsPlusPlugin Instance { get; private set; }

        public IKitRepository KitRepository { get; private set; }
        public CooldownManager CooldownManager { get; private set; }
        public UsageManager UsageManager { get; private set; }

        protected override void Load()
        {
            base.Load();
            Instance = this;

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


            switch (Configuration.Instance.Repository.ToLower())
            {
                case "mysql":
                    //todo
                    break;
                case "default":
                case "base":
                case "local":
                default:
                    KitRepository = new BaseRepository();
                    break;

            }

            Logger.Log($"Initialized Kit repository: {KitRepository.Name}");

            CooldownManager = new CooldownManager(Configuration.Instance.GlobalCooldownSeconds);
            UsageManager = new UsageManager();
            
            Logger.Log($"KitsPlus v{Assembly.GetName().Version} by Anomoly has loaded");
        }

        protected override void Unload()
        {
            base.Unload();

            Instance = null;
            Logger.Log("KitsPlus has unloaded");
        }

        public override TranslationList DefaultTranslations => new TranslationList()
        {
            {"error_invalid_args","Invalid arguments! /{0} {1}" },
            {"command_kit_not_found","Failed to find kit by the name of \"{0}\"." },
            {"command_kit_global_cooldown", "Please wait {0} before redeeming another kit." },
            {"command_kit_kit_cooldown", "Please wait {0} before redeeming kit \"{1}\" again." },
            {"command_kit_max_usage", "You have used {0}/{0} uses of the \"{1}\" kit!" },
            {"command_kit_redeemed","You've successfully redeemed the kit: \"{0}\"" },
            {"command_kits","Available Kits: {0}" }
        };

        private static bool IsInitialized()
        {
            var unitySyncContext = typeof(PlayerLoopHelper).GetField("unitySynchronizationContext", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);
            var mainThreadId = typeof(PlayerLoopHelper).GetField("mainThreadId", BindingFlags.Static | BindingFlags.NonPublic)?.GetValue(null);

            return unitySyncContext != null && mainThreadId != null && (int)mainThreadId == Thread.CurrentThread.ManagedThreadId;
        }
    }
}
