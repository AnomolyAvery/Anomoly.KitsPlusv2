﻿using Anomoly.KitsPlusv2.Entities;
using Rocket.API;
using System.Threading.Tasks;

namespace Anomoly.KitsPlusv2.Repositories
{
    public interface IKitRepository
    {
        string Name { get; }

        Kit[] GetAllKits();
        Kit[] GetAllKits(IRocketPlayer player);
        Kit GetKitByName(string name);
        void DeleteKit(Kit kit);
        void CreateKit(Kit kit);
        void ResetKits();

        void Unload();
    }
}
