using System.Xml.Serialization;

namespace Anomoly.KitsPlusv2.Entities
{
    public class KitWeapon: KitItem
    {
        public Attachment Barrel { get; set; }

        public Attachment Sight { get; set; }

        public Attachment Grip { get; set; }

        public Attachment Tactical { get; set; }

        public Attachment Magazine { get; set; }

        public KitWeapon(ushort id, byte amount): base(id, amount) { }

        public KitWeapon() { }
    }

    public class Attachment
    {
        [XmlAttribute("Id")]
        public ushort AttachmentId { get; set; }

        [XmlAttribute("Durability")]
        public byte Durability { get; set; } = 100;

        public Attachment(ushort attachmentId, byte durability)
        {
            AttachmentId = attachmentId;
            Durability = durability;
        }

        public Attachment() { }
    }
}
