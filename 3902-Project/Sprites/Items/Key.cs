using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;

namespace Project.Sprites.Items;

public class Key : Item
{
    public Key(SpriteBatch spriteBatch, Game1 game) : base(spriteBatch, game, ItemTypeEnums.Key)
    {
        SpriteRotationPivot = new Vector2(0, TextureSourceRectangle.Height / 2f);
    }

    // Simple swinging animation for keys
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
                SpriteAnimationRotation = 80 - 110f * ItemTimeSinceLastUsage / ItemStats.UsageTime;
            }
            else if (SpriteFlip == SpriteEffects.FlipHorizontally)
            {
                SpriteAnimationRotation = -(80 - 110f * ItemTimeSinceLastUsage / ItemStats.UsageTime);
            }

            SpriteAnimationRotation = MathHelper.ToRadians(SpriteAnimationRotation);
        }
    }
}