using Anomoly.KitsPlusv2.Entities;
using Anomoly.KitsPlusv2.Utils;
using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anomoly.KitsPlusv2.Repositories
{
    public class MySQLRepository : IKitRepository
    {
        private MySQLDBConnection _db;

        private const string KitsTableSchema = @"
            CREATE TABLE IF NOT EXISTS `kits` (
                name VARCHAR(255) PRIMARY KEY,
                xp INT UNSIGNED DEFAULT NULL,
                vehicle SMALLINT UNSIGNED DEFAULT NULL,
                cooldown INT NOT NULL,
                max_usages INT NOT NULL DEFAULT 0
            );
        ";

        private const string KitItemsTableSchema = @"
            CREATE TABLE IF NOT EXISTS `kit_items` (
                id INT AUTO_INCREMENT PRIMARY KEY,
                kit_name VARCHAR(255) NOT NULL,
                item_id SMALLINT UNSIGNED NOT NULL,
                amount TINYINT UNSIGNED NOT NULL DEFAULT 1,
                is_weapon BOOLEAN NOT NULL DEFAULT FALSE,
                barrel_attachment_id SMALLINT UNSIGNED DEFAULT NULL,
                barrel_durability TINYINT UNSIGNED DEFAULT NULL,
                sight_attachment_id SMALLINT UNSIGNED DEFAULT NULL,
                sight_durability TINYINT UNSIGNED DEFAULT NULL,
                grip_attachment_id SMALLINT UNSIGNED DEFAULT NULL,
                grip_durability TINYINT UNSIGNED DEFAULT NULL,
                tactical_attachment_id SMALLINT UNSIGNED DEFAULT NULL,
                tactical_durability TINYINT UNSIGNED DEFAULT NULL,
                magazine_attachment_id SMALLINT UNSIGNED DEFAULT NULL,
                magazine_durability TINYINT UNSIGNED DEFAULT NULL,
                FOREIGN KEY(kit_name) REFERENCES kits(name) ON DELETE CASCADE
            );
        ";

        public string Name => "mysql";

        public MySQLRepository(string connectionString)
        {
            _db = new MySQLDBConnection(connectionString);

            CheckSchema();
        }

        public void CreateKit(Kit kit)
            => CreateKitAsync(kit).GetAwaiter().GetResult();

        public void DeleteKit(Kit kit)
            => DeleteKitAsync(kit).GetAwaiter().GetResult();

        public Kit[] GetAllKits()
            => GetAllKitsAsync().GetAwaiter().GetResult();

        public Kit[] GetAllKits(IRocketPlayer player)
            => GetAllKitsAsync(player).GetAwaiter().GetResult();

        public Kit GetKitByName(string name)
            => GetKitByNameAsync(name).GetAwaiter().GetResult();

        public void ResetKits()
            => ResetKitsAsync().GetAwaiter().GetResult();

        private void CheckSchema()
        {
            _db.ExecuteUpdate(KitsTableSchema);
            _db.ExecuteUpdate(KitItemsTableSchema);
        }

        public void Unload()
            => _db.Dispose();

        public async Task<Kit[]> GetAllKitsAsync()
        {
            var kits = new List<Kit>();

            string queryKits = @"
                SELECT name, xp, vehicle, cooldown, max_usages 
                FROM kits;
            ";

            using (var readerKits = await _db.ExecuteAsync(queryKits))
            {
                while (await readerKits.ReadAsync())
                {
                    var kit = new Kit
                    {
                        Name = readerKits.GetString("name"),
                        XP = readerKits.IsDBNull(1) ? null : (uint?)readerKits.GetUInt32("xp"),
                        Vehicle = readerKits.IsDBNull(2) ? null : (ushort?)readerKits.GetUInt16("vehicle"),
                        Cooldown = readerKits.GetInt32("cooldown"),
                        MaxUsages = readerKits.GetInt32("max_usages"),
                        Items = new List<KitItem>()
                    };

                    kits.Add(kit);
                }
            }

            return kits.ToArray();
        }

        public async Task<Kit[]> GetAllKitsAsync(IRocketPlayer player)
            => (await GetAllKitsAsync()).Where(x => player.HasPermission($"kit.{x.Name}")).ToArray();

        public async Task<Kit> GetKitByNameAsync(string name)
        {
            Kit kit = null;

            string queryKit = @"
                SELECT name, xp, vehicle, cooldown, max_usages 
                FROM kits 
                WHERE name = ?;
            ";

            string queryItems = @"
                SELECT item_id, amount, is_weapon, 
                       barrel_attachment_id, barrel_durability,
                       sight_attachment_id, sight_durability,
                       grip_attachment_id, grip_durability,
                       tactical_attachment_id, tactical_durability,
                       magazine_attachment_id, magazine_durability
                FROM kit_items 
                WHERE kit_name = ?;
            ";

            using (var readerKit = await _db.ExecuteAsync(queryKit, name))
            {
                if (await readerKit.ReadAsync())
                {
                    kit = new Kit
                    {
                        Name = readerKit.GetString("name"),
                        XP = readerKit.IsDBNull(1) ? null : (uint?)readerKit.GetUInt32("xp"),
                        Vehicle = readerKit.IsDBNull(2) ? null : (ushort?)readerKit.GetUInt16("vehicle"),
                        Cooldown = readerKit.GetInt32("cooldown"),
                        MaxUsages = readerKit.GetInt32("max_usages"),
                        Items = new List<KitItem>()
                    };
                }
            }

            if (kit == null)
            {
                return null;
            }

            using (var readerItems = await _db.ExecuteAsync(queryItems, name))
            {
                while (await readerItems.ReadAsync())
                {
                    ushort itemID = readerItems.GetUInt16("item_id");
                    byte amount = readerItems.GetByte("amount");
                    bool isWeapon = readerItems.GetBoolean("is_weapon");

                    KitItem kitItem;
                    if (isWeapon)
                    {
                        var weapon = new KitWeapon(itemID, amount)
                        {
                            Barrel = readerItems.IsDBNull(3) ? null : new Attachment
                            {
                                AttachmentId = readerItems.GetUInt16("barrel_attachment_id"),
                                Durability = readerItems.GetByte("barrel_durability")
                            },
                            Sight = readerItems.IsDBNull(5) ? null : new Attachment
                            {
                                AttachmentId = readerItems.GetUInt16("sight_attachment_id"),
                                Durability = readerItems.GetByte("sight_durability")
                            },
                            Grip = readerItems.IsDBNull(7) ? null : new Attachment
                            {
                                AttachmentId = readerItems.GetUInt16("grip_attachment_id"),
                                Durability = readerItems.GetByte("grip_durability")
                            },
                            Tactical = readerItems.IsDBNull(9) ? null : new Attachment
                            {
                                AttachmentId = readerItems.GetUInt16("tactical_attachment_id"),
                                Durability = readerItems.GetByte("tactical_durability")
                            },
                            Magazine = readerItems.IsDBNull(11) ? null : new Attachment
                            {
                                AttachmentId = readerItems.GetUInt16("magazine_attachment_id"),
                                Durability = readerItems.GetByte("magazine_durability")
                            }
                        };

                        kitItem = weapon;
                    }
                    else
                    {
                        kitItem = new KitItem(itemID, amount);
                    }

                    kit.Items.Add(kitItem);
                }
            }

            return kit;
        }

        public async Task DeleteKitAsync(Kit kit)
        {

            string queryDeleteItems = @"
                DELETE FROM kit_items 
                WHERE kit_name = ?;
            ";

            string queryDeleteKit = @"
                DELETE FROM kits 
                WHERE name = ?;
            ";

            using (var transaction = await _db._conn.BeginTransactionAsync())
            {
                try
                {
                    await _db.ExecuteUpdateAsync(queryDeleteItems, kit.Name, transaction);

                    await _db.ExecuteUpdateAsync(queryDeleteKit, kit.Name, transaction);

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Failed to delete kit.", ex);
                }
            }
        }

        public async Task CreateKitAsync(Kit kit)
        {
            string queryInsertKit = @"
                INSERT INTO kits (name, xp, vehicle, cooldown, max_usages) 
                VALUES (?, ?, ?, ?, ?);
            ";

            string queryInsertItems = @"
                INSERT INTO kit_items (kit_name, item_id, amount, is_weapon,
                    barrel_attachment_id, barrel_durability,
                    sight_attachment_id, sight_durability,
                    grip_attachment_id, grip_durability,
                    tactical_attachment_id, tactical_durability,
                    magazine_attachment_id, magazine_durability)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
            ";

            using (var transaction = await _db._conn.BeginTransactionAsync())
            {
                try
                {
                    await _db.ExecuteUpdateAsync(queryInsertKit,
                        kit.Name,
                        kit.XP,
                        kit.Vehicle,
                        kit.Cooldown,
                        kit.MaxUsages,
                        transaction);

                    foreach (var item in kit.Items)
                    {
                        if (item is KitWeapon weapon)
                        {
                            await _db.ExecuteUpdateAsync(queryInsertItems,
                                kit.Name,
                                weapon.Id,
                                weapon.Amount,
                                true,
                                weapon.Barrel?.AttachmentId,
                                weapon.Barrel?.Durability,
                                weapon.Sight?.AttachmentId,
                                weapon.Sight?.Durability,
                                weapon.Grip?.AttachmentId,
                                weapon.Grip?.Durability,
                                weapon.Tactical?.AttachmentId,
                                weapon.Tactical?.Durability,
                                weapon.Magazine?.AttachmentId,
                                weapon.Magazine?.Durability,
                                transaction);
                        }
                        else
                        {
                            await _db.ExecuteUpdateAsync(queryInsertItems,
                                kit.Name,
                                item.Id,
                                item.Amount,
                                false,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null,
                                null,
                                transaction);
                        }
                    }

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw new Exception("Failed to create kit.", ex);
                }
            }
        }

        public async Task ResetKitsAsync()
        {
            await _db.ExecuteUpdateAsync("TRUNCATE TABLE `kit_items`");
            await _db.ExecuteUpdateAsync("DELETE FROM `kits`");
        }
    }
}
