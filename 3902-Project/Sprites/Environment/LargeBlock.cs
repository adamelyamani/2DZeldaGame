using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Sprites.Environment
{
    public class LargeBlock : Environment
    {
        public LargeBlock(SpriteBatch spriteBatch, Game game, EnvironmentTypeEnums type) : base(spriteBatch, game, type.ToString())
        {
        }
        public override void Update(GameTime gameTime)
        {
        }
    }
}
