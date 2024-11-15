using Microsoft.Xna.Framework.Input;
using Project.App;
using Project.Commands;
using System.Collections.Generic;

namespace Project.Controllers
{
    public class MainKeyboardController : KeyboardController
    {

        private Dictionary<Keys, ICommand> _keysToCommandsOnPress;
        private Dictionary<Keys, ICommand> _keysToCommandsOnHold;

        public MainKeyboardController(Game1 currentGame)
        {
            PreviousPressedKeys = new HashSet<Keys>();
            FillDictionary(currentGame);
        }

        public override void Update()
        {
            UpdateOnPress();
            UpdateWhileHeld();
        }

        private void FillDictionary(Game1 currentGame)
        {
            
            //These commands will only execute once when the key is pressed
            _keysToCommandsOnPress = new Dictionary<Keys, ICommand>
            {
                { Keys.Q, new Quit(currentGame) },
                { Keys.R, new ResetGame(currentGame)},
                { Keys.M, new ToggleMusic(currentGame)},
            };

            //These commands will execute while the key is held down
            _keysToCommandsOnHold = new Dictionary<Keys, ICommand>
            {
               
            };
        }

        private void UpdateOnPress()
        {
            var state = Keyboard.GetState();

            var currentPressed = new HashSet<Keys>(state.GetPressedKeys());
            currentPressed.ExceptWith(PreviousPressedKeys);

            foreach (var key in currentPressed)
            {
                if (_keysToCommandsOnPress.ContainsKey(key))
                {
                    _keysToCommandsOnPress[key].Execute();
                }
            }

            PreviousPressedKeys = new HashSet<Keys>(state.GetPressedKeys());
        }

        private void UpdateWhileHeld()
        {
            var state = Keyboard.GetState();

            foreach (var pressed in state.GetPressedKeys())
            {
                if (_keysToCommandsOnHold.ContainsKey(pressed))
                {
                    _keysToCommandsOnHold[pressed].Execute();
                }
            }
        }
    }
}
