using Anomoly.KitsPlusv2.Utils;
using Cysharp.Threading.Tasks;
using Rocket.API;
using System.Collections.Generic;
using System.Linq;

namespace Anomoly.KitsPlusv2.Commands
{
    public class CommandKits : IRocketCommand
    {
        private KitsPlusPlugin _plugin
            => KitsPlusPlugin.Instance;

        public AllowedCaller AllowedCaller => AllowedCaller.Both;

        public string Name => "kits";

        public string Help => "View a list of all available kits";

        public string Syntax => "";

        public List<string> Aliases => new List<string>();

        public List<string> Permissions => new List<string>() { "kits" };

        private UniTask ExecuteAsync(IRocketPlayer caller, string[] args)
        {
            var kits = _plugin.KitRepository.GetAllKits(caller);

            var message = _plugin.Translate("command_kits", string.Join(", ", kits.Select(x =>
            {
                string name = x.Name;
                if(_plugin.Configuration.Instance.DisplayKitUsages && x.MaxUsages > 0)
                {
                    var usages = _plugin.UsageManager.GetKitUsage(caller.Id, x.Name);
                    var left = (x.MaxUsages - usages);
                    name = $"{name}({left})";
                }

                return name;
            })));

            FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));

            return UniTask.CompletedTask;
        }

        public void Execute(IRocketPlayer caller, string[] args)
            => UniTask.RunOnThreadPool(() => ExecuteAsync(caller, args));
    }
}
