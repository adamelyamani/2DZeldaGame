using Microsoft.Xna.Framework;

namespace Project.Sprites
{
    public interface ISprite
    {
        public void Draw();

        public void Update(GameTime gameTime);
    }
}
