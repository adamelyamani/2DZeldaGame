using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;

namespace Project.Sprites.Items;

public class Shield : Item
{
    public Shield(SpriteBatch spriteBatch, Game1 game, ItemTypeEnums shield) : base(spriteBatch, game, shield)
    {
        // Left side center
        SpriteRotationPivot = new Vector2(0, TextureSourceRectangle.Height/2f);
    }

    // Simple swinging animation for shields
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
                SpriteAnimationRotation = 15 - 15f * ItemTimeSinceLastUsage / ItemStats.UsageTime;
            }
            else
            {
                SpriteAnimationRotation = 15 + 15f * ItemTimeSinceLastUsage / ItemStats.UsageTime;
            }
            
            SpriteAnimationRotation = MathHelper.ToRadians(SpriteAnimationRotation);
        }
    }
}