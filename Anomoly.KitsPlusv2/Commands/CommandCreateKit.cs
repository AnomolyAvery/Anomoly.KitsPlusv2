using Anomoly.KitsPlusv2.Entities;
using Anomoly.KitsPlusv2.Utils;
using Cysharp.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anomoly.KitsPlusv2.Commands
{
    public class CommandCreateKit : IRocketCommand
    {
        private KitsPlusPlugin _plugin
            => KitsPlusPlugin.Instance;

        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "createkit";

        public string Help => "Create a kit from items in your inventory";

        public string Syntax => "<name> [<cooldown>] [<vehicle>] [<xp>] [<max_usage>]";

        public List<string> Aliases => new List<string>() { "ckit" };

        public List<string> Permissions => new List<string>() { "createkit" };

        private UniTask ExecuteAsync(IRocketPlayer caller, string[] args)
        {
            string message;
            if(args.Length == 0 || args.Length > 5)
            {
                message = _plugin.Translate("error_invalid_args", Name, Syntax);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            var name = args[0];
            int cooldown = 0;
            ushort vehicle = 0;
            uint xp = 0;
            int maxUsage = 0;

            if(args.Length >= 2 && !int.TryParse(args[1], out cooldown))
            {
                message = _plugin.Translate("error_invalid_args", Name, Syntax);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            if(args.Length >= 3 && !ushort.TryParse(args[2], out vehicle))
            {
                message = _plugin.Translate("error_invalid_args", Name, Syntax);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            if(args.Length >= 4 && !uint.TryParse(args[3], out xp))
            {
                message = _plugin.Translate("error_invalid_args", Name, Syntax);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            if(args.Length == 5 && !int.TryParse(args[4], out maxUsage))
            {
                message = _plugin.Translate("error_invalid_args", Name, Syntax);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            var existingKit = _plugin.KitRepository.GetKitByName(name);
            if (existingKit != null)
            {
                message = _plugin.Translate("command_createkit_exists", name);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            var player = (UnturnedPlayer)caller;

            var kit = new Kit()
            {
                Name = name,
                Items = player.GetKitItemsFromInventory(),
                Cooldown = cooldown,
                MaxUsages = maxUsage,
                Vehicle = vehicle > 0 ? vehicle : null,
                XP = xp > 0 ? xp : null,
            };

            _plugin.KitRepository.CreateKit(kit);

            message = _plugin.Translate("command_createkit_created", name);
            FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));

            return UniTask.CompletedTask;
        }

        public void Execute(IRocketPlayer caller, string[] args)
            => UniTask.RunOnThreadPool(() => ExecuteAsync(caller, args));
    }
}
