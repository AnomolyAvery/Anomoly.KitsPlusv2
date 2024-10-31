using Anomoly.KitsPlusv2.Entities;
using Rocket.API;
using System.Threading.Tasks;

namespace Anomoly.KitsPlusv2.Repositories
{
    public interface IKitRepository
    {
        string Name { get; }

        // Synchronous methods
        Kit[] GetAllKits();
        Kit[] GetAllKits(IRocketPlayer player);
        Kit GetKitByName(string name);
        void DeleteKit(Kit kit);
        void CreateKit(Kit kit);
        void ResetKits();

        // Asynchronous methods
        Task<Kit[]> GetAllKitsAsync();
        Task<Kit[]> GetAllKitsAsync(IRocketPlayer player);
        Task<Kit> GetKitByNameAsync(string name);
        Task DeleteKitAsync(Kit kit);
        Task CreateKitAsync(Kit kit);
        Task ResetKitsAsync();

        void Unload();
    }
}
