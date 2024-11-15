using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Project.App;
using Project.Commands;

namespace Project.Controllers
{
    public class MainMouseController : MouseController
    {
        private Dictionary<int, ICommand> _clicksToCommands;
        private readonly Game1 _game;

        public MainMouseController(Game1 currentGame)
        {
            _game = currentGame;
            FillDictionary(currentGame);
        }

        public override void Update()
        {
            var state = Mouse.GetState();

            var xBound = _game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            var yBound = _game.GraphicsDevice.PresentationParameters.BackBufferHeight;

            if (state.X < 0 || state.Y < 0 || state.X > xBound || state.Y > yBound)
            {
                return;
            }

            if (state.LeftButton != ButtonState.Pressed)
            {
                return;
            }

            // Attack testing (likely refactor in future)
            _clicksToCommands[0].Execute();
        }

        private void FillDictionary(Game1 currentGame)
        {
            _clicksToCommands = new Dictionary<int, ICommand>
            {
                { 0, new PlayerItemObjectUse(currentGame) }                 // Attack testing (likely refactor in future)
            };
        }
    }
}
