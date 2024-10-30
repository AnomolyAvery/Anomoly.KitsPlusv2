using Anomoly.KitsPlusv2.Entities;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anomoly.KitsPlusv2.Repositories
{
    public class MySQLRepository : IKitRepository
    {
        public string Name => throw new NotImplementedException();

        public void CreateKit(Kit kit)
        {
            throw new NotImplementedException();
        }

        public void DeleteKit(Kit kit)
        {
            throw new NotImplementedException();
        }

        public Kit[] GetAllKits()
        {
            throw new NotImplementedException();
        }

        public Kit[] GetAllKits(IRocketPlayer player)
        {
            throw new NotImplementedException();
        }

        public Kit GetKitByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}
