using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;

namespace Project.Sprites.Items;

public class MeleeWeapon : Item
{
    public MeleeWeapon(SpriteBatch spriteBatch, Game1 game, ItemTypeEnums weapon = ItemTypeEnums.WoodenGreatsword) :
        base(spriteBatch, game, weapon)
    {
        // Scythes have a wonky handle position
        if (weapon == ItemTypeEnums.BoneScythe || weapon == ItemTypeEnums.WoodenScythe)
        {
            SpriteRotationPivot = new Vector2(TextureSourceRectangle.Width / 4f, TextureSourceRectangle.Height-18);
        }
        else
        {
            SpriteRotationPivot = new Vector2(TextureSourceRectangle.Width / 2f, TextureSourceRectangle.Height-18);
        }
    }

    public override void Use()
    {
        if (ItemTimeSinceLastUsage > ItemStats.UsageTime) SoundManager.Instance.PlaySound(SfxEnums.SwordAttack);
        base.Use();
    }

    // Simple swinging animation suitable for basic weapons
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (ItemState == ItemStateEnums.Equipped)
        {
            if (ItemTimeSinceLastUsage > ItemStats.UsageTime)
            {
                SpriteAnimationRotation = 0;
            }
            else
            {
                if (SpriteFlip == SpriteEffects.None)
                {
                    SpriteAnimationRotation = 90f * ItemTimeSinceLastUsage / ItemStats.UsageTime;
                } 
                else if (SpriteFlip == SpriteEffects.FlipHorizontally)
                {
                    SpriteAnimationRotation = -90f * ItemTimeSinceLastUsage / ItemStats.UsageTime;
                }
                SpriteAnimationRotation = MathHelper.ToRadians(SpriteAnimationRotation);
            }
        }
    }
}