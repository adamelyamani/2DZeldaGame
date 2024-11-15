using Project.App;
using Project.Sprites.Items;

namespace Project.Commands;
public class DrinkHealthPotion : ICommand
{
    private readonly Game1 _game;

    public DrinkHealthPotion(Game1 currentGame)
    {
        _game = currentGame;
    }

    public void Execute()
    {
        if (_game.Player.IsAlive && _game.GameState is GameStateEnums.Running)
        {   
            // Search for LargeHealthPotion and then Small Health Potion
            var pot = _game.Inventory.InventoryList.Find(x => x.ItemType is ItemTypeEnums.LargeHealthPotion) ?? _game.Inventory.InventoryList.Find(x => x.ItemType is ItemTypeEnums.SmallHealthPotion);

            // If a potion was found, equip it, drink it, then re-equip the first item
            if (pot != null) {
                var currentItem = _game.Player.LeftHand.HeldItem;
                if (currentItem != null)
                {
                    currentItem.UnEquip();
                    pot.Equip();
                    _game.Inventory.Equip(pot);
                    pot.Use();
                    pot.UnEquip();
                    currentItem.Equip();
                    _game.Inventory.Equip(currentItem);
                }
            }
        }
    }
}