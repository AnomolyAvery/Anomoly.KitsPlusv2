using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Anomoly.KitsPlusv2.Entities
{
    public class Kit
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        public uint? XP { get; set; }

        public ushort? Vehicle { get; set; }

        public int Cooldown { get; set; }


        [XmlArray("Items")]
        [XmlArrayItem("Item")]
        public List<KitItem> Items { get; set; }

        public Kit(string name, List<KitItem> items, uint? xp, ushort? vehicle, int cooldown)
        {
            Name = name;
            Items = items;
            XP = xp;
            Vehicle = vehicle;
            Cooldown = cooldown;
        }

        public Kit() { }

        public void GiveKit(UnturnedPlayer player)
        {

            if (XP.HasValue)
                player.Experience += XP.Value;

            if (Vehicle.HasValue)
                player.GiveVehicle(Vehicle.Value);

            foreach(var item in Items)
            {
                if(item is KitWeapon weapon)
                {
                    var wItem = new Item(weapon.Id, true);

                    // Code for assembling weapon attachments adapted from uEssentials repository.
                    // Source: https://github.com/uEssentials/uEssentials/blob/master/src/NativeModules/Kit/Item/KitItemWeapon.cs
                    // This code initializes the metadata for weapon attachments by mapping attachment IDs and durability
                    // to specific byte indexes in the item metadata array.

                    Action<int[], Attachment> assembleAttach = (indexes, attach) => {
                        if (attach == null || attach.AttachmentId == 0) return;

                        var attachIdBytes = BitConverter.GetBytes(attach.AttachmentId);

                        wItem.metadata[indexes[0]] = attachIdBytes[0];
                        wItem.metadata[indexes[1]] = attachIdBytes[1];
                        wItem.metadata[indexes[2]] = attach.Durability;
                    };

                    assembleAttach([0x0, 0x1, 0xD], weapon.Sight);
                    assembleAttach([0x2, 0x3, 0xE], weapon.Tactical);
                    assembleAttach([0x4, 0x5, 0xF], weapon.Grip);
                    assembleAttach([0x6, 0x7, 0x10], weapon.Barrel);
                    assembleAttach([0x8, 0x9, 0x11], weapon.Magazine);


                    wItem.metadata[0xC] = 1;

                    player.GiveItem(wItem);
                    continue;
                }

                player.GiveItem(item.Id, item.Amount);
            }
        }
    }
}
