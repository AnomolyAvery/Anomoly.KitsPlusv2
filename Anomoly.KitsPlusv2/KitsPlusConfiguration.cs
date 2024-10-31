using Anomoly.KitsPlusv2.Entities;
using Rocket.API;
using System.Collections.Generic;

namespace Anomoly.KitsPlusv2
{
    public class KitsPlusConfiguration : IRocketPluginConfiguration
    {
        public string Repository { get; set; }

        public string MySQLConnectionString { get; set; }

        public int GlobalCooldownSeconds { get; set; }
        public bool DisplayKitUsages { get; set; }
        public List<Kit> Kits { get; set; }
        public void LoadDefaults()
        {
            Repository = "default";
            MySQLConnectionString = "Server=localhost;Port=3306;Database=unturned;Uid=root;Pwd=my_password;";
            GlobalCooldownSeconds = 10;
            DisplayKitUsages = true;
            Kits = new List<Kit>()
            {
                new Kit("Starter", new List<KitItem>()
                {
                    new KitItem(211,1),
                    new KitItem(209,1),
                    new KitWeapon(363, 1)
                    {
                        Barrel = new Attachment(4, 100),
                        Sight = new Attachment(1004, 100),
                        Grip = new Attachment(8, 100),
                        Tactical = new Attachment(151, 100),
                        Magazine = new Attachment(17, 100)
                    }
                }, 1000, 17, 120)
            };
        }
    }
}
