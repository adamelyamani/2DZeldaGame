using Project.App;
using Project.Sprites.Items;

namespace Project.Commands;
public class DrinkShieldPotion : ICommand
{
    private readonly Game1 _game;

    public DrinkShieldPotion(Game1 currentGame)
    {
        _game = currentGame;
    }

    public void Execute()
    {
        if (_game.Player.IsAlive && _game.GameState is GameStateEnums.Running)
        {   
            // Search for Large Shield Potion and then Small Shield Potion
            var pot = _game.Inventory.InventoryList.Find(x => x.ItemType is ItemTypeEnums.LargeShieldPotion) ?? _game.Inventory.InventoryList.Find(x => x.ItemType is ItemTypeEnums.SmallShieldPotion);

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