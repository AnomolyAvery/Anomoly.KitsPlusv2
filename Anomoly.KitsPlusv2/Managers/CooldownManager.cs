using System;
using System.Collections.Generic;

namespace Anomoly.KitsPlusv2.Managers
{
    public class CooldownManager
    {
        private KitsPlusPlugin _plugin
            => KitsPlusPlugin.Instance;

        private Dictionary<string, DateTime> _global;
        private Dictionary<Tuple<string, string>, DateTime> _kits;

        private int _globalCooldown;

        public CooldownManager(int globalCooldown)
        {
            _globalCooldown = globalCooldown;
            _global = new Dictionary<string, DateTime>();
            _kits = new Dictionary<Tuple<string, string>, DateTime>();
        }

        public void SetGlobalCooldown(string playerId)
        {
            if (_globalCooldown <= 0)
                return;

            _global[playerId] = DateTime.Now;
        }

        public int GetTimeLeftForGlobal(string playerId)
        {
            if (_global.ContainsKey(playerId))
            {
                var start = _global[playerId];
                var wait = (DateTime.Now - start).TotalSeconds;

                var left = (_globalCooldown - Convert.ToInt32(wait));

                if(left <= 0)
                {
                    left = 0;
                    _global.Remove(playerId);
                }

                return left;
            }

            return 0;
        }

        public void SetKitCooldown(string playerId, string kit)
        {
            var key = Tuple.Create(playerId, kit);

            _kits[key] = DateTime.Now;
        }

        public int GetTimeLeftForKit(string playerId, string name)
        {
            var key = Tuple.Create(playerId, name);

            if (!_kits.ContainsKey(key))
                return 0;

            var kit = _plugin.KitRepository.GetKitByName(name);
            if (kit == null)
                return 0;

            var start = _kits[key];
            var wait = (DateTime.Now - start).TotalSeconds;

            var left = (kit.Cooldown - Convert.ToInt32(wait));

            if(left <= 0)
            {
                left = 0;
                _kits.Remove(key);
            }

            return left;
        }
    }
}
