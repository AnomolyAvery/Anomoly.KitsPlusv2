using Anomoly.KitsPlusv2.Utils;
using Cysharp.Threading.Tasks;
using Rocket.API;
using Rocket.Unturned.Player;
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
            if(kit == null)
            {
                message = _plugin.Translate("command_kit_not_found", name);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            
            FastTaskDispatcher.QueueOnMainThread(() =>
            {
                kit.GiveKit((UnturnedPlayer)caller);

                message = _plugin.Translate("command_kit_redeemed", kit.Name);
                ChatUtility.Say(caller, message);
            });

            return UniTask.CompletedTask;
        }

        public void Execute(IRocketPlayer caller, string[] args)
            => UniTask.RunOnThreadPool(async () => await ExecuteAsync(caller, args));
    }
}
