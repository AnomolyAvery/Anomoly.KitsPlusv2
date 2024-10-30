using Anomoly.KitsPlusv2.Utils;
using Cysharp.Threading.Tasks;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var message = _plugin.Translate("command_kits", string.Join(", ", kits.Select(x => x.Name)));

            FastTaskDispatcher.QueueOnMainThread(() => ChatUtility.Say(caller, message));

            return UniTask.CompletedTask;
        }

        public void Execute(IRocketPlayer caller, string[] args)
            => UniTask.RunOnThreadPool(() => ExecuteAsync(caller, args));
    }
}
