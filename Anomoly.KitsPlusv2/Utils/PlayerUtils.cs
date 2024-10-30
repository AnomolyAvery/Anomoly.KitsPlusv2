using Anomoly.KitsPlusv2.Entities;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System.Linq;

namespace Anomoly.KitsPlusv2.Utils
{
    public static class PlayerUtils
    {
        public static List<KitItem> GetKitItemsFromInventory(this UnturnedPlayer player)
        {
            var clothing = player.Player.clothing;

            var items = new List<KitItem>();

            if (clothing.backpack != 0)
                items.Add(new KitItem() { Id = clothing.backpack, Amount = 1 });
            if (clothing.vest != 0)
                items.Add(new KitItem() { Id = clothing.vest, Amount = 1 });
            if (clothing.shirt != 0)
                items.Add(new KitItem() { Id = clothing.shirt, Amount = 1 });
            if (clothing.pants != 0)
                items.Add(new KitItem() { Id = clothing.pants, Amount = 1 });
            if (clothing.mask != 0)
                items.Add(new KitItem() { Id = clothing.mask, Amount = 1 });
            if (clothing.hat != 0)
                items.Add(new KitItem() { Id = clothing.hat, Amount = 1 });
            if (clothing.glasses != 0)
                items.Add(new KitItem() { Id = clothing.glasses, Amount = 1 });

            for (byte page = 0; page < PlayerInventory.PAGES - 2; page++)
            {
                for (byte index = 0; index < player.Inventory.getItemCount(page); index++)
                {
                    var jar = player.Inventory.getItem(page, index);
                    if (jar == null)
                        continue;

                    var asset = Assets.find(EAssetType.ITEM, jar.item.id);
                    if (asset == null)
                        continue;

                    if(asset is ItemGunAsset)
                    {
                        items.Add(new KitWeapon()
                        {
                            Amount = 1,
                            Id = jar.item.id,
                            Barrel = ItemUtils.GetWeaponAttachment(jar.item.metadata, AttachmentType.BARREL),
                            Grip = ItemUtils.GetWeaponAttachment(jar.item.metadata, AttachmentType.GRIP),
                            Magazine = ItemUtils.GetWeaponAttachment(jar.item.metadata, AttachmentType.MAGAZINE),
                            Sight = ItemUtils.GetWeaponAttachment(jar.item.metadata, AttachmentType.SIGHT),
                            Tactical = ItemUtils.GetWeaponAttachment(jar.item.metadata, AttachmentType.TACTICAL),
                        });
                    }
                    else
                    {
                        var item = items.FirstOrDefault(x => x.Id == jar.item.id);

                        if (item == null)
                        {
                            items.Add(new KitItem()
                            {
                                Id = jar.item.id,
                                Amount = 1,
                            });

                            continue;
                        }

                        item.Amount++;
                    }
                }
            }

            return items;
        }
    }
}
