using System.Diagnostics;
using Project.App;

namespace Project.Commands;

public class ResetGame : ICommand
{
    private readonly Game1 _game;

    public ResetGame(Game1 currentGame)
    {
        _game = currentGame;
    }

    public void Execute()
    {
        Debug.WriteLine("RESET");
        _game.ResetGame();
    }
}
