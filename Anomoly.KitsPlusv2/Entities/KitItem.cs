using System.Xml.Serialization;

namespace Anomoly.KitsPlusv2.Entities
{
    [XmlInclude(typeof(KitWeapon))]
    public class KitItem
    {
        [XmlAttribute("Id")]
        public ushort Id { get; set; }

        [XmlAttribute("Amount")]
        public byte Amount { get; set; }

        public KitItem(ushort id, byte amount)
        {
            Id = id;
            Amount = amount;
        }

        public KitItem() { }
    }
}
