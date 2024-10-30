using Anomoly.KitsPlusv2.Utils;
using Cysharp.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;

namespace Anomoly.KitsPlusv2.Commands
{
    public class CommandKit : IRocketCommand
    {
        private KitsPlusPlugin _plugin
            => KitsPlusPlugin.Instance;
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "kit";

        public string Help => "Redeem a kit";

        public string Syntax => "<kit_name>";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "kit" };


        private UniTask ExecuteAsync(IRocketPlayer caller, string[] args)
        {
            string message;
            if(args.Length != 1)
            {
                message = _plugin.Translate("error_invalid_args", Name, Syntax);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            var name = args[0];

            var kit = _plugin.KitRepository.GetKitByName(name);
            if(kit == null || !caller.HasPermission($"kit.{kit.Name.ToLower()}"))
            {
                message = _plugin.Translate("error_kit_not_found", name);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            var gCooldown = _plugin.CooldownManager.GetTimeLeftForGlobal(caller.Id);
            if(gCooldown > 0)
            {
                message = _plugin.Translate("error_global_cooldown", TimeSpan.FromSeconds(gCooldown).ToReadableString());
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            var kCooldown = _plugin.CooldownManager.GetTimeLeftForKit(caller.Id, kit.Name);
            if(kCooldown > 0)
            {
                message = _plugin.Translate("error_kit_cooldown", TimeSpan.FromSeconds(kCooldown).ToReadableString(),kit.Name);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            if(kit.MaxUsages > 0 && _plugin.UsageManager.GetKitUsage(caller.Id, kit.Name) >= kit.MaxUsages)
            {
                message = _plugin.Translate("error_max_usage", kit.MaxUsages, kit.Name);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }
            
            FastTaskDispatcher.QueueOnMainThread(() =>
            {
                kit.GiveKit((UnturnedPlayer)caller);

                message = _plugin.Translate("command_kit_redeemed", kit.Name);
                ChatUtility.Say(caller, message);
            });

            _plugin.CooldownManager.SetGlobalCooldown(caller.Id);
            _plugin.CooldownManager.SetKitCooldown(caller.Id, kit.Name);
            if (kit.MaxUsages > 0)
                _plugin.UsageManager.AddUsage(caller.Id, kit.Name);

            return UniTask.CompletedTask;
        }

        public void Execute(IRocketPlayer caller, string[] args)
            => UniTask.RunOnThreadPool(async () => await ExecuteAsync(caller, args));
    }
}
