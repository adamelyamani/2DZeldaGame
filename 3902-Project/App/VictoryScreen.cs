using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Sprites;

namespace Project.App
{
    public class VictoryScreen : ISprite
    {
        private const float Opacity = 0;
        private const int RectangleWidth = 400;
        private const int RectangleHeight = 55;
        private const int RectangleX = 432;
        private const int QuitRectangleY = 400;
        private const int RespawnRectangleY = 300;
        private Rectangle _respawnRectangle;
        private readonly Rectangle _quitRectangle;
        private readonly SpriteBatch _spriteBatch;
        private readonly Game1 _game;
        private Texture2D _whitePixel;

        public VictoryScreen(SpriteBatch spriteBatch, Game1 game)
        {
            _spriteBatch = spriteBatch;
            _game = game;
            _respawnRectangle = new Rectangle(RectangleX, RespawnRectangleY, RectangleWidth, RectangleHeight);
            _quitRectangle = new Rectangle(RectangleX, QuitRectangleY, RectangleWidth, RectangleHeight);
        }

        private Texture2D WhitePixel
        {
            get
            {
                if (_whitePixel == null)
                {
                    _whitePixel = new Texture2D(_game.GraphicsDevice, 1, 1);
                    _whitePixel.SetData(new[] { new Color(255, 255, 255, Opacity) });
                }

                return _whitePixel;
            }
        }

        public void Update(GameTime gameTime)
        {
            
        }

        public void Draw()
        {
            var coverScreen = new Rectangle(0, 0, _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth,
                _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight);

            _game.CurrentLevel.Draw();
            _game.Player.Draw();
            ProjectileManager.Instance.Draw();

            _spriteBatch.Begin();
            _spriteBatch.Draw(WhitePixel, coverScreen, Color.DarkGreen);
            _spriteBatch.End();

            //Draw Respawn and Quit rectangles
            _spriteBatch.Begin();
            _spriteBatch.Draw(WhitePixel, _respawnRectangle, Color.Gray);
            _spriteBatch.Draw(WhitePixel, _quitRectangle, Color.Gray);
            _spriteBatch.End();

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_game.Font, "You Won!", new Vector2(RectangleX + RectangleWidth / 2f - 100, 150), Color.White);
            _spriteBatch.DrawString(_game.Font, "Play Again", new Vector2(RectangleX + RectangleWidth / 2f - 108, RespawnRectangleY), Color.SlateGray);
            _spriteBatch.DrawString(_game.Font, "Quit", new Vector2(RectangleX + RectangleWidth / 2f - 50, QuitRectangleY), Color.SlateGray);
            _spriteBatch.End();
        }
    }
}
