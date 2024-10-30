using Anomoly.KitsPlusv2.Entities;

namespace Anomoly.KitsPlusv2.Repositories
{
    public interface IKitRepository
    {
        string Name { get; }

        Kit[] GetAllKits();
        Kit GetKitByName(string name);
    }
}
