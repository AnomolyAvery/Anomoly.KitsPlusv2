using Anomoly.KitsPlusv2.Utils;
using Cysharp.Threading.Tasks;
using Rocket.API;
using System.Collections.Generic;

namespace Anomoly.KitsPlusv2.Commands
{
    public class CommandDeleteKit : IRocketCommand
    {
        private KitsPlusPlugin _plugin
            => KitsPlusPlugin.Instance;
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "deletekit";

        public string Help => "Delete a kit";

        public string Syntax => "<kit_name>";

        public List<string> Aliases => new List<string>() { "dkit" };

        public List<string> Permissions => new List<string>() { "deletekit" };

        private UniTask ExecuteAsync(IRocketPlayer caller, string[] args)
        {
            string message;
            if(args.Length == 0 || args.Length > 1)
            {
                message = _plugin.Translate("error_invalid_args", Name, Syntax);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            var name = args[0];

            var kit = _plugin.KitRepository.GetKitByName(name);
            if(kit == null)
            {
                message = _plugin.Translate("error_kit_not_found", name);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            _plugin.KitRepository.DeleteKit(kit);

            message = _plugin.Translate("command_deletekit_deleted", kit.Name);
            FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
            return UniTask.CompletedTask;
        }

        public void Execute(IRocketPlayer caller, string[] args)
            => UniTask.RunOnThreadPool(() => ExecuteAsync(caller, args));
    }
}
