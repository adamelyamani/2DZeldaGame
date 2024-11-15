using Project.App;
using System.Numerics;

namespace Project.Commands;
    public class MoveCommand : ICommand
    {
        private readonly Game1 _game;
        private readonly Vector2 _direction;

        public MoveCommand(Game1 game, Vector2 direction)
        {
            _game = game;
            _direction = direction;
        }

        public void Execute()
        {
            _game.Player.Move(_direction);
        }
    }
