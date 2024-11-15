using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project.Sprites;
using Project.Sprites.Players;

namespace Project.App
{
    public class MainScreen : ISprite
    {
        private const string ChooseCharacterString = "Welcome! Choose your character from below!";
        private const int CharacterOffsetX = 200;
        private const int CharacterOffsetY = 400;
        private const int TitleBarHeight = 100;
        private readonly SpriteBatch _spriteBatch;
        private readonly Game1 _game;
        private Texture2D _whitePixel;
        private IPlayer _roguePlayer;
        private IPlayer _wizardPlayer;
        private IPlayer _knightPlayer;
        private bool _firstUpdate = true;

        public MainScreen(SpriteBatch spriteBatch, Game1 game)
        {
            _spriteBatch = spriteBatch;
            _game = game;

            InitializeMainScreenSprites();
        }

        private Texture2D WhitePixel
        {
            get
            {
                if (_whitePixel == null)
                {
                    _whitePixel = new Texture2D(_game.GraphicsDevice, 1, 1);
                    _whitePixel.SetData(new[] { Color.White });
                }

                return _whitePixel;
            }
        }

        public void Update(GameTime gameTime)
        {
            //This is needed to get the hands to be draw on start screen. Not sure but it is a work around that works.
            if (_firstUpdate)
            {
                _roguePlayer.Update(gameTime);
                _knightPlayer.Update(gameTime);
                _wizardPlayer.Update(gameTime);
                _firstUpdate = false;
            }

            var mouse = Mouse.GetState();
            if (mouse.X < 0 || mouse.X > _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth)
            {
                return;
            }

            if(mouse.Y < TitleBarHeight || mouse.Y > _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight)
            {
                return;
            }

            if (mouse.X < _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth / 3)
            {
                _roguePlayer.Update(gameTime);

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    _game.Player = new Rogue(_spriteBatch, _game);
                    _game.GameState = GameStateEnums.Running;
                }
            }
            else if (mouse.X < 2 * _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth / 3)
            {
                _wizardPlayer.Update(gameTime);

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    _game.Player = new Wizard(_spriteBatch, _game);
                    _game.GameState = GameStateEnums.Running;
                }
            }
            else
            {
                _knightPlayer.Update(gameTime);

                if (mouse.LeftButton == ButtonState.Pressed)
                {
                    _game.Player = new Knight(_spriteBatch, _game);
                    _game.GameState = GameStateEnums.Running;
                }
            }
        }

        public void Draw()
        {
            var backWidth = _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth;
            var backHeight = _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight;
            //Split the screen into thirds. One for each player choice
            var greenBackground = new Rectangle(0, 100, backWidth / 3, backHeight);
            var purpleBackground = new Rectangle(backWidth / 3,TitleBarHeight , backWidth / 3, backHeight);
            var blueBackground = new Rectangle(2 * backWidth / 3, TitleBarHeight, backWidth / 3, backHeight);

            _spriteBatch.Begin();
            _spriteBatch.DrawString(_game.Font, ChooseCharacterString, new Vector2(125, 25), Color.White);
            _spriteBatch.Draw(WhitePixel, greenBackground, Color.DarkGreen);
            _spriteBatch.Draw(WhitePixel, purpleBackground, Color.Purple);
            _spriteBatch.Draw(WhitePixel, blueBackground, Color.DarkBlue);
            _spriteBatch.End();

            _roguePlayer.Draw();
            _wizardPlayer.Draw();
            _knightPlayer.Draw();
        }

        private void InitializeMainScreenSprites()
        { 
            var backWidth = _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth;

            _roguePlayer = new Rogue(_spriteBatch, _game);
            _roguePlayer.Position = new Vector2(CharacterOffsetX, CharacterOffsetY);

            _knightPlayer = new Knight(_spriteBatch, _game);
            _knightPlayer.Position = new Vector2(CharacterOffsetX + 2 * backWidth / 3f, CharacterOffsetY);

            _wizardPlayer = new Wizard(_spriteBatch, _game);
            _wizardPlayer.Position = new Vector2(CharacterOffsetX + backWidth / 3f, CharacterOffsetY);
        }
    }
}
