using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Sprites.Environment
{
    public class Wall : Environment
    {
        public Wall(SpriteBatch spriteBatch, Game game, EnvironmentTypeEnums type) : base(spriteBatch, game, type.ToString())
        {
        }
        public override void Update(GameTime gameTime)
        {
        }
    }
}
