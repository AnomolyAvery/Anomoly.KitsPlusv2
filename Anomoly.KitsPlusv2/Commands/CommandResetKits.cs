using Anomoly.KitsPlusv2.Utils;
using Cysharp.Threading.Tasks;
using Rocket.API;
using System.Collections.Generic;

namespace Anomoly.KitsPlusv2.Commands
{
    public class CommandResetKits : IRocketCommand
    {
        private KitsPlusPlugin _plugin
            => KitsPlusPlugin.Instance;
        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "resetkits";

        public string Help => "Reset kit data, and or usages.";

        public string Syntax => "[all|usages|data]";

        public List<string> Aliases => new List<string>() { "rkits" };

        public List<string> Permissions => new List<string>() { "resetkits" };

        private UniTask ExecuteAsync(IRocketPlayer caller, string[] args)
        {
            string message;
            if(args.Length == 0 || args.Length > 1)
            {
                message = _plugin.Translate("error_invalid_args", Name, Syntax);
                FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                return UniTask.CompletedTask;
            }

            switch (args[0].ToLower())
            {
                case "all":
                    _plugin.UsageManager.ResetUsages();
                    _plugin.KitRepository.ResetKits();

                    message = _plugin.Translate("command_resetkits", "data & usages");
                    FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                    break;
                case "data":
                    _plugin.KitRepository.ResetKits();

                    message = _plugin.Translate("command_resetkits", "data");
                    FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                    break;
                case "usages":
                    _plugin.UsageManager.ResetUsages();

                    message = _plugin.Translate("command_resetkits","usages");
                    FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                    break;
                default:
                    message = _plugin.Translate("error_invalid_args", Name, Syntax);
                    FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));
                    break;
            }

            return UniTask.CompletedTask;
        }

        public void Execute(IRocketPlayer caller, string[] args)
            => UniTask.RunOnThreadPool(() => ExecuteAsync(caller, args));
    }
}
