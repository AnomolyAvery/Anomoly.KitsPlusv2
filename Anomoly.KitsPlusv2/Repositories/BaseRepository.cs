using Anomoly.KitsPlusv2.Entities;
using System;
using System.Linq;

namespace Anomoly.KitsPlusv2.Repositories
{
    public class BaseRepository : IKitRepository
    {
        public string Name => "default";

        public Kit[] GetAllKits()
            => KitsPlusPlugin.Instance.Configuration.Instance.Kits.ToArray();

        public Kit GetKitByName(string name)
            => GetAllKits().FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
