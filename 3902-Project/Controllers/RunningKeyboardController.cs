using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Microsoft.Xna.Framework.Input;
using Project.App;
using Project.Commands;

namespace Project.Controllers
{
    public class RunningKeyboardController : KeyboardController
    {
        private Dictionary<Keys, ICommand> _keysToCommandsOnPress;
        private Dictionary<Keys, ICommand> _keysToCommandsOnHold;
        private Dictionary<Keys, ICommand> _keysToCommandsOnDouble;
        private Game1 _game;
        private HashSet<Keys> _currentPressedKeys => new HashSet<Keys>(Keyboard.GetState().GetPressedKeys());

        private int frameCounter, doubleInterval, dashCoolDown, dashWaitCounter;
        private bool dblPresent;
        

        public RunningKeyboardController(Game1 currentGame)
        {
            PreviousPressedKeys = new HashSet<Keys>();
            FillDictionary(currentGame);
            _game = currentGame;

            doubleInterval = 3;
            frameCounter = 0;
            dashCoolDown = 60;
            dashWaitCounter = 60;
            dblPresent = false;

        }

        public override void Update()
        {
            UpdateOnPress();
            UpdateWhileHeld();

            dashWaitCounter++;
            if (dblPresent)
                frameCounter++;
        }

        private void FillDictionary(Game1 currentGame)
        {
            // Create Base Directions
            Vector2 right = new Vector2(1, 0);
            Vector2 down = new Vector2(0, 1);
            Vector2 left = new Vector2(-1, 0);
            Vector2 up = new Vector2(0, -1);

            //These commands will only execute once when the key is pressed
            _keysToCommandsOnPress = new Dictionary<Keys, ICommand>
            {
                { Keys.Q, new Quit(currentGame) },
                { Keys.R, new ResetGame(currentGame)},
                { Keys.O, new SwitchPreviousLevel(currentGame)},
                { Keys.P, new SwitchNextLevel(currentGame)},
                { Keys.Z, new PlayerItemObjectUse(currentGame)},
                { Keys.N,new PlayerItemObjectUse(currentGame)},
                { Keys.E, new DamageSelfCommand(currentGame)},
                { Keys.Escape, new PauseGame(currentGame)},
                { Keys.Space, new DashCommand(currentGame) },
                { Keys.M, new ToggleMusic(currentGame)},
                { Keys.D1, new DrinkHealthPotion(currentGame)},
                { Keys.D2, new DrinkShieldPotion(currentGame)},
            };

            //These commands will execute while the key is held down
            _keysToCommandsOnHold = new Dictionary<Keys, ICommand>
            {
                // Arrows Movement
                { Keys.Right, new MoveCommand(currentGame, right)},
                { Keys.Down, new MoveCommand(currentGame, down)},
                { Keys.Left, new MoveCommand(currentGame, left)},
                { Keys.Up, new MoveCommand(currentGame, up)},

                // WASD Movement
                { Keys.D, new MoveCommand(currentGame, right)},
                { Keys.S, new MoveCommand(currentGame, down)},
                { Keys.A, new MoveCommand(currentGame, left)},
                { Keys.W, new MoveCommand(currentGame, up)}
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
