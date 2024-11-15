using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Interfaces;

namespace Project.Sprites.Items;

internal class ItemFactory : ImportFactory
{
    public ItemFactory(Game1 game, SpriteBatch spriteBatch) : base(game, spriteBatch)
    {
    }

    // Create a new item based on an Enum value
    // Satisfies Factory
    public override IItem Create(Enum itemType)
    {
        return itemType switch
        {
            ItemTypeEnums.WoodenGreatsword =>  new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.WoodenGreatsword),
            ItemTypeEnums.WoodenMallet =>      new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.WoodenMallet),
            ItemTypeEnums.WoodenDagger =>      new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.WoodenDagger),
            ItemTypeEnums.WoodenAxe =>         new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.WoodenAxe),
            ItemTypeEnums.WoodenSpear =>       new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.WoodenSpear),
            ItemTypeEnums.WoodenPickaxe =>     new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.WoodenPickaxe),
            ItemTypeEnums.WoodenShovel =>      new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.WoodenShovel),
            ItemTypeEnums.WoodenScythe =>      new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.WoodenScythe),

            ItemTypeEnums.WoodenBow =>         new BowWeapon  (SpriteBatchObject, GameObject, ItemTypeEnums.WoodenBow),
            ItemTypeEnums.WoodenScepter =>     new MagicWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.WoodenScepter),
            ItemTypeEnums.WoodenWand =>        new MagicWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.WoodenWand),

            ItemTypeEnums.BoneGreatsword =>    new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.BoneGreatsword),
            ItemTypeEnums.BoneMallet =>        new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.BoneMallet),
            ItemTypeEnums.BoneDagger =>        new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.BoneDagger),
            ItemTypeEnums.BoneAxe =>           new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.BoneAxe),
            ItemTypeEnums.BoneSpear =>         new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.BoneSpear),
            ItemTypeEnums.BonePickaxe =>       new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.BonePickaxe),
            ItemTypeEnums.BoneShovel =>        new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.BoneShovel),
            ItemTypeEnums.BoneScythe =>        new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.BoneScythe),
            ItemTypeEnums.BoneRake =>          new MeleeWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.BoneRake),

            ItemTypeEnums.BoneBow =>           new BowWeapon  (SpriteBatchObject, GameObject, ItemTypeEnums.BoneBow),
            ItemTypeEnums.BoneScepter =>       new MagicWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.BoneScepter),
            ItemTypeEnums.BoneWand =>          new MagicWeapon(SpriteBatchObject, GameObject, ItemTypeEnums.BoneWand),

            ItemTypeEnums.LargeHealthPotion => new Potion     (SpriteBatchObject, GameObject, ItemTypeEnums.LargeHealthPotion),
            ItemTypeEnums.SmallHealthPotion => new Potion     (SpriteBatchObject, GameObject, ItemTypeEnums.SmallHealthPotion),
            ItemTypeEnums.LargeShieldPotion => new Potion     (SpriteBatchObject, GameObject, ItemTypeEnums.LargeShieldPotion),
            ItemTypeEnums.SmallShieldPotion => new Potion     (SpriteBatchObject, GameObject, ItemTypeEnums.SmallShieldPotion),

            ItemTypeEnums.Key =>               new Key        (SpriteBatchObject, GameObject),
            ItemTypeEnums.Map =>               new Map (SpriteBatchObject, GameObject),

            _ => throw new NotImplementedException("Item Type \"" + (ItemTypeEnums)itemType + "\" is not supported by ItemFactory"),
        };
    }

    // Converts the LevelData String identifier of an item into an ItemTypeEnums value, then calls Create on it
    // Satisfies ImportFactory
    public override IItem Create(LevelObjectData levelObjectData)
    {
        var rawItemData = levelObjectData as ItemLevelObjectData ?? throw new ArgumentException($"\"{levelObjectData.Type}\" does not have type ItemLevelObjectData.");

        if (!Enum.TryParse(rawItemData.Type, out ItemTypeEnums parsedItemType))
        {
            throw new NotImplementedException("Item String Type: \"" + rawItemData.Type + "\" cannot be parsed into ItemTypeEnums");
        }

        // Create the item object
        var newItem = Create(parsedItemType);

        // Set item position
        newItem.Position = new Vector2(rawItemData.X, rawItemData.Y);

        return newItem;
    }
}