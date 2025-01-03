﻿using Anomoly.KitsPlusv2.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Anomoly.KitsPlusv2.Managers
{
    public class UsageManager
    {
        private JsonFileDB<Dictionary<string, int>> _usages;

        public UsageManager()
        {
            var directory = KitsPlusPlugin.Instance.Directory;
            string file = Path.Combine(directory, "kit_usages.json");

            _usages = new JsonFileDB<Dictionary<string, int>>(file, new Dictionary<string, int>());
            _usages.Load();
        }

        public int GetKitUsage(string playerId, string name)
        {
            var key = $"{playerId}_{name}";
            if (!_usages.Instance.ContainsKey(key))
                return 0;

            return _usages.Instance[key];
        }

        public void AddUsage(string playerId, string name)
        {
            var key = $"{playerId}_{name}";
            _usages.Instance[key] = _usages.Instance.TryGetValue(key, out var current) ? current + 1 : 1;
        }

        public void ResetUsages()
        {
            _usages.Instance.Clear();
            Save();
        }

        public void Save()
            => _usages.Save();
    }
}
