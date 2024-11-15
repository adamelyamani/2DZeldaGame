using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Project.Sprites.Environment
{
    public abstract class  Trap : Environment
    {
        public bool Active;
        public float Damage;
        protected Trap(SpriteBatch spriteBatch, Game game, string textureString) : base(spriteBatch, game, textureString)
        {
        }
    }
}
