using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Sprites
{
    public class StaticText : ISprite
    {
        private readonly List<string> _text;
        private readonly SpriteBatch _spriteBatch;
        private readonly SpriteFont _font;

        public StaticText(SpriteBatch spriteBatch, SpriteFont spriteFont, List<string> text, Vector2 position)
        {
            _spriteBatch = spriteBatch;
            _font = spriteFont;
            _text = text;
            Position = position;
        }

        public Vector2 Position { get; set; }

        public void Draw()
        {
            _spriteBatch.Begin();

            var tempPosition = Position;

            foreach (var text in _text)
            {
                _spriteBatch.DrawString(_font, text, tempPosition, Color.Black);
                tempPosition = new Vector2(tempPosition.X, tempPosition.Y + _font.LineSpacing);
            }

            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
        }

    }
}
