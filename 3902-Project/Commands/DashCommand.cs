using Microsoft.Xna.Framework.Input;
using Project.App;
using System;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.VisualBasic;

namespace Project.Commands;
    public class DashCommand : ICommand
    {
        private readonly Game1 _game;

        public DashCommand(Game1 game)
        {
            _game = game;
        }

        public Vector2 CalculateDirection()
        {
            var currentPressed = new HashSet<Keys>(Keyboard.GetState().GetPressedKeys());

            var direction = new Vector2();

            if (currentPressed.Contains(Keys.A))
                direction.X = -1;
            else if (currentPressed.Contains(Keys.D))
                direction.X = 1;
            else
                direction.X = 0;

            if (currentPressed.Contains(Keys.W))
                direction.Y = -1;
            else if (currentPressed.Contains(Keys.S))
                direction.Y = 1;
            else
                direction.Y = 0;

            if(direction is not { X: 0, Y: 0 }) direction = Vector2.Normalize(direction);

            return direction;
        }

        public void Execute()
        {
            _game.Player.Dash(CalculateDirection());
        }
    }
