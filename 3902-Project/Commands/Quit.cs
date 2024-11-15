using Project.App;

namespace Project.Commands;
    public class Quit : ICommand
    {
        private readonly Game1 _game;

        public Quit(Game1 currentGame)
        {
            _game = currentGame;
        }

        public void Execute()
        {
            _game.Exit();
        }
    }
