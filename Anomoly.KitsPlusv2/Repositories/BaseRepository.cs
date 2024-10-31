using Anomoly.KitsPlusv2.Entities;
using Rocket.API;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Anomoly.KitsPlusv2.Repositories
{
    public class BaseRepository : IKitRepository
    {
        private KitsPlusPlugin _plugin
            => KitsPlusPlugin.Instance;

        public string Name => "default";

        public void CreateKit(Kit kit)
        {
            _plugin.Configuration.Instance.Kits.Add(kit);
            _plugin.Configuration.Save();
        }

        public void DeleteKit(Kit kit)
        {
            _plugin.Configuration.Instance.Kits.Remove(kit);
            _plugin.Configuration.Save();
        }

        public Kit[] GetAllKits()
            => _plugin.Configuration.Instance.Kits.ToArray();

        public Kit[] GetAllKits(IRocketPlayer player)
            => GetAllKits().Where(x => player.HasPermission($"kit.{x.Name.ToLower()}")).ToArray();

        public Kit GetKitByName(string name)
            => GetAllKits().FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public void ResetKits()
        {
            _plugin.Configuration.Instance.Kits.Clear();
            _plugin.Configuration.Save();
        }

        public void Unload() { }

        #region NotImplemented
        public Task<Kit[]> GetAllKitsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Kit[]> GetAllKitsAsync(IRocketPlayer player)
        {
            throw new NotImplementedException();
        }

        public Task<Kit> GetKitByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task DeleteKitAsync(Kit kit)
        {
            throw new NotImplementedException();
        }

        public Task CreateKitAsync(Kit kit)
        {
            throw new NotImplementedException();
        }

        public Task ResetKitsAsync()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
