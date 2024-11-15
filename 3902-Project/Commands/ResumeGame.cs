using System.Diagnostics;
using Project.App;

namespace Project.Commands;
public class ResumeGame : ICommand
{
    private readonly Game1 _game;

    public ResumeGame(Game1 game)
    {
        _game = game;
    }

    public void Execute()
    {
        _game.GameState = GameStateEnums.Running;
        Debug.WriteLine("Resume");
    }
}
