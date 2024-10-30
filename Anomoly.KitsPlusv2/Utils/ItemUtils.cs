using Anomoly.KitsPlusv2.Entities;
using System;

namespace Anomoly.KitsPlusv2.Utils
{

    public static class ItemUtils
    {
        public static Attachment GetWeaponAttachment(byte[] metadata, AttachmentType type)
        {
            if (metadata.Length < 18)
                return null;

            var indexes = GetAttachmentIndexes(type);

            var durability = metadata[indexes[2]];
            var id = BitConverter.ToUInt16(metadata, indexes[0]);

            if (id == 0)
                return null;

            return new Attachment(id, durability);
        }

        private static int[] GetAttachmentIndexes(AttachmentType type)
        {
            switch (type)
            {
                case AttachmentType.SIGHT: return [0, 1, 13];
                case AttachmentType.TACTICAL: return [2, 3, 14];
                case AttachmentType.GRIP: return [4, 5, 15];
                case AttachmentType.BARREL: return [6, 7, 16];
                case AttachmentType.MAGAZINE: return [8, 9, 17];
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}
