using Anomoly.KitsPlusv2.Utils;
using Cysharp.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;

namespace Anomoly.KitsPlusv2.Commands
{
    public class CommandGiftKit : IRocketCommand
    {

        private KitsPlusPlugin _plugin
            => KitsPlusPlugin.Instance;

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "giftkit";

        public string Help => "Gift a kit to a player while taking the cooldown yourself";

        public string Syntax => "<player> <kit_name>";

        public List<string> Aliases => new List<string>() { "gkit" };

        public List<string> Permissions => new List<string>() { "giftkit" };

        private UniTask ExecuteAsync(IRocketPlayer caller, string[] args)
        {
            string message;
            if(args.Length == 0 || args.Length > 2)
            {
                message = _plugin.Translate("error_invalid_args", Name, Syntax);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            var target = UnturnedPlayer.FromName(args[0]);
            if(target == null)
            {
                message = _plugin.Translate("error_player_not_found", args[0]);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            if(target.Id == caller.Id)
            {
                message = _plugin.Translate("command_giftkit_self");
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            var kit = _plugin.KitRepository.GetKitByName(args[1]);
            if(kit == null || !caller.HasPermission($"kit.{kit.Name.ToLower()}"))
            {
                message = _plugin.Translate("error_kit_not_found", args[1]);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            var isConsolePlayer = caller is ConsolePlayer;

            if (isConsolePlayer)
            {
                message = _plugin.Translate("command_giftkit_gifted", kit.Name, target.DisplayName);
                FastTaskDispatcher.QueueOnMainThread(() =>
                {
                    kit.GiveKit(target);
                    ChatUtility.Say(caller, message);
                    ChatUtility.Say(target, _plugin.Translate("command_giftkit_gifted_target", caller.DisplayName, kit.Name));
                });
                return UniTask.CompletedTask;
            }

            if (kit.MaxUsages > 0 && _plugin.UsageManager.GetKitUsage(caller.Id, kit.Name) >= kit.MaxUsages)
            {
                message = _plugin.Translate("error_max_usage", kit.MaxUsages, kit.Name);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            var gCooldown = _plugin.CooldownManager.GetTimeLeftForGlobal(caller.Id);
            if (gCooldown > 0)
            {
                message = _plugin.Translate("error_global_cooldown", TimeSpan.FromSeconds(gCooldown).ToReadableString());
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            var kCooldown = _plugin.CooldownManager.GetTimeLeftForKit(caller.Id, kit.Name);
            if (kCooldown > 0)
            {
                message = _plugin.Translate("error_kit_cooldown", TimeSpan.FromSeconds(kCooldown).ToReadableString(), kit.Name);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            message = _plugin.Translate("command_giftkit_gifted", kit.Name, target.DisplayName);
            FastTaskDispatcher.QueueOnMainThread(() =>
            {
                kit.GiveKit(target);
                ChatUtility.Say(caller, message);
                ChatUtility.Say(target, _plugin.Translate("command_giftkit_gifted_target", caller.DisplayName, kit.Name));
            });

            _plugin.CooldownManager.SetGlobalCooldown(caller.Id);
            _plugin.CooldownManager.SetKitCooldown(caller.Id, kit.Name);
            if (kit.MaxUsages > 0)
                _plugin.UsageManager.AddUsage(caller.Id, kit.Name);

            return UniTask.CompletedTask;
        }

        public void Execute(IRocketPlayer caller, string[] args)
            => UniTask.RunOnThreadPool(() => ExecuteAsync(caller, args));
    }
}
