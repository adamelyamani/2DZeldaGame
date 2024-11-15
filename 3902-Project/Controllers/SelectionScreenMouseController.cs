using Microsoft.Xna.Framework.Input;
using Project.App;

namespace Project.Controllers
{
    public class SelectionScreenMouseController : MouseController
    {
        private readonly Game1 _game;
        private const int RectangleWidth = 400;
        private const int RectangleHeight = 55;
        private const int RectangleX = 432;
        private const int QuitRectangleY = 400;
        private const int RespawnRectangleY = 300;
        private bool _pressedOnQuit;
        private bool _pressedOnRespawn;

        public SelectionScreenMouseController(Game1 currentGame)
        {
            _game = currentGame;
            _pressedOnQuit = false;
            _pressedOnRespawn = false;
        }

        public override void Update()
        {
            var mouse = Mouse.GetState();

            var xBound = _game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            var yBound = _game.GraphicsDevice.PresentationParameters.BackBufferHeight;

            if (mouse.X < 0 || mouse.Y < 0 || mouse.X > xBound || mouse.Y > yBound)
            {
                return;
            }

            if (mouse.LeftButton == ButtonState.Released)
            {
                if (_pressedOnQuit)
                {
                    _game.Exit();
                    _pressedOnQuit = false;
                }

                if (_pressedOnRespawn)
                {
                    _game.ResetGame();
                    _pressedOnRespawn = false;
                }

                return;
            }

            if (mouse is { LeftButton: ButtonState.Pressed, X: > RectangleX } &&
                mouse.X < RectangleX + RectangleWidth && mouse.Y > RespawnRectangleY &&
                mouse.Y < RespawnRectangleY + RectangleHeight)
            {
                _pressedOnRespawn = true;
            }

            if (mouse.LeftButton == ButtonState.Pressed && mouse.X > RectangleX &&
                mouse.X < RectangleX + RectangleWidth && mouse.Y > QuitRectangleY &&
                mouse.Y < QuitRectangleY + RectangleHeight)
            {
                _pressedOnQuit = true;
            }
        }
    }
}
