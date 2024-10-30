﻿using Anomoly.KitsPlusv2.Entities;
using Rocket.API;
using System;
using System.Linq;

namespace Anomoly.KitsPlusv2.Repositories
{
    public class BaseRepository : IKitRepository
    {
        public string Name => "default";

        public void CreateKit(Kit kit)
        {
            KitsPlusPlugin.Instance.Configuration.Instance.Kits.Add(kit);
            KitsPlusPlugin.Instance.Configuration.Save();
        }

        public void DeleteKit(Kit kit)
        {
            KitsPlusPlugin.Instance.Configuration.Instance.Kits.Remove(kit);
            KitsPlusPlugin.Instance.Configuration.Save();
        }

        public Kit[] GetAllKits()
            => KitsPlusPlugin.Instance.Configuration.Instance.Kits.ToArray();

        public Kit[] GetAllKits(IRocketPlayer player)
            => GetAllKits().Where(x => player.HasPermission($"kit.{x.Name.ToLower()}")).ToArray();

        public Kit GetKitByName(string name)
            => GetAllKits().FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
