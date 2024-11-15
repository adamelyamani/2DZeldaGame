using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Sprites;

namespace Project.App
{
    public class TransitionScreen : ISprite
    {
        private const int TransitionTimeInMilliseconds = 500;
        private const int WaitTimeInMilliseconds = 250;
        private readonly SpriteBatch _spriteBatch;
        private readonly Game1 _game;
        private Texture2D _whitePixel;
        private bool _fadeOut;
        private float _opacity;
        private int _gameFadeTimeRemaining;
        private int _gameWaitTimeRemaining;


        public TransitionScreen(SpriteBatch spriteBatch, Game1 game)
        {
            _spriteBatch = spriteBatch;
            _game = game;
            _fadeOut = true;
            _opacity = 1.0f;
            _gameFadeTimeRemaining = TransitionTimeInMilliseconds;
            _gameWaitTimeRemaining = WaitTimeInMilliseconds;
        }

        private Texture2D WhitePixel
        {
            get
            {
                if (_whitePixel == null)
                {
                    _whitePixel = new Texture2D(_game.GraphicsDevice, 1, 1);
                    _whitePixel.SetData(new Color[] { Color.White });
                }

                return _whitePixel;
            }
        }

        private bool FadeOut
        {
            get => _fadeOut;
            set
            {
                _fadeOut = value;

                if (value)
                {
                    _gameFadeTimeRemaining = TransitionTimeInMilliseconds;
                    _gameWaitTimeRemaining = WaitTimeInMilliseconds;
                }
                else
                {
                    _game.Player.Position = _game.NewPlayerPosition;
                    _gameFadeTimeRemaining = TransitionTimeInMilliseconds;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            //Draw screen to black in 1 second.
            //Wait .25 seconds
            //Draw black to screen in 1 second.

            if (FadeOut)
            {
                //Percentage of elapsed time;
                _gameFadeTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
                _opacity = (TransitionTimeInMilliseconds - _gameFadeTimeRemaining) / (float)TransitionTimeInMilliseconds;

                //Return if still fading
                if (_gameFadeTimeRemaining > 0)
                {
                    return;
                }

                //Wait .25 seconds at full opacity to change levels
                _opacity = 1.0f;
                _gameWaitTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;

                if (_gameWaitTimeRemaining > 0)
                {
                    return;
                }

                FadeOut = false;
                _game.Player.LeftHand.Update(gameTime);
                _game.Player.RightHand.Update(gameTime);
            }
            else
            {
                _gameFadeTimeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
                _opacity = 1 - ((TransitionTimeInMilliseconds - _gameFadeTimeRemaining) / (float)TransitionTimeInMilliseconds);

                //Return if still fading
                if (_gameFadeTimeRemaining > 0)
                {
                    return;
                }

                FadeOut = true;
                _game.GameState = GameStateEnums.Running;
            }
        }

        public void Draw()
        {
            var coverScreen = new Rectangle(0, 0, _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth,
                _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight);

            if (_fadeOut)
            {
                _game.OldLevel.Draw();
            }
            else 
            {
                _game.CurrentLevel.Draw();
            }

            _game.Player.Draw();

            ProjectileManager.Instance.Draw();

            _spriteBatch.Begin();
            _spriteBatch.Draw(WhitePixel, coverScreen, new Color(0, 0, 0, _opacity));
            _spriteBatch.End();
        }
    }
}
