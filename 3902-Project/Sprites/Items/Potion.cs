using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;

namespace Project.Sprites.Items;

public class Potion : Item
{
    public Potion(SpriteBatch spriteBatch, Game1 game, ItemTypeEnums potion) : base(spriteBatch, game, potion)
    {
        // Slightly left of potion
        SpriteRotationPivot = new Vector2(-TextureSourceRectangle.Width / 2f, TextureSourceRectangle.Height);
    }

    public override void Use()
    {
        if (ItemTimeSinceLastUsage > ItemStats.UsageTime)
        {
            // Some of this stuff is kind of dangerous and relies on the empty health potion textures being similar to the full ones
            switch (ItemType)
            {
                case ItemTypeEnums.SmallHealthPotion:
                    GameObject.Player.Health += ItemStats.EffectMagnitude;
                    SoundManager.Instance.PlaySound(SfxEnums.Potion);
                    ItemType = ItemTypeEnums.SmallEmptyPotion;
                    ItemStats = ItemStatSheet.GetStats(ItemType);
                    Texture = GameObject.Content.Load<Texture2D>(ItemType.ToString());
                    break;
                case ItemTypeEnums.LargeHealthPotion:
                    GameObject.Player.Health += ItemStats.EffectMagnitude;
                    SoundManager.Instance.PlaySound(SfxEnums.Potion);
                    ItemType = ItemTypeEnums.LargeEmptyPotion;
                    ItemStats = ItemStatSheet.GetStats(ItemType);
                    Texture = GameObject.Content.Load<Texture2D>(ItemType.ToString());
                    break;
                case ItemTypeEnums.SmallShieldPotion:
                    GameObject.Player.Shield += ItemStats.EffectMagnitude;
                    SoundManager.Instance.PlaySound(SfxEnums.Potion);
                    ItemType = ItemTypeEnums.SmallEmptyPotion;
                    ItemStats = ItemStatSheet.GetStats(ItemType);
                    Texture = GameObject.Content.Load<Texture2D>(ItemType.ToString());
                    break;
                case ItemTypeEnums.LargeShieldPotion:
                    GameObject.Player.Shield += ItemStats.EffectMagnitude;
                    SoundManager.Instance.PlaySound(SfxEnums.Potion);
                    ItemType = ItemTypeEnums.LargeEmptyPotion;
                    ItemStats = ItemStatSheet.GetStats(ItemType);
                    Texture = GameObject.Content.Load<Texture2D>(ItemType.ToString());
                    break;
            }

            base.Use();
        }
    }

    // Simple swinging animation suitable for basic potions
    public override void Update(GameTime gameTime)
    {
        ItemTimeSinceLastUsage += gameTime.ElapsedGameTime.Milliseconds;
        if (ItemTimeSinceLastUsage > ItemStats.UsageTime)
        {
            SpriteAnimationRotation = 0;
        }
        else
        {
            if (SpriteFlip == SpriteEffects.None)
            {
                SpriteAnimationRotation = -45f * ItemTimeSinceLastUsage / ItemStats.UsageTime;
            }
            else if (SpriteFlip == SpriteEffects.FlipHorizontally)
            {
                SpriteAnimationRotation = 45f * ItemTimeSinceLastUsage / ItemStats.UsageTime;
            }

            SpriteAnimationRotation = MathHelper.ToRadians(SpriteAnimationRotation);
        }
    }
}