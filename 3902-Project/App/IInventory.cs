using System.Collections.Generic;
using Project.Sprites;
using Project.Sprites.Items;

namespace Project.App
{
    public interface IInventory : ISprite
    {
    
        public List<IItem> InventoryList { get; set; }

        public IItem LeftHand { get; set; }

        public void Equip(IItem item);

        public void Collect(IItem item);

        public void Drop(IItem item);
    }
}
