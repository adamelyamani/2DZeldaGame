using Project.App;

namespace Project.Commands;
public class PauseGame : ICommand
{
    private readonly Game1 _game;

    public PauseGame(Game1 game)
    {
        _game = game;
    }

    public void Execute()
    {
        _game.GameState = GameStateEnums.Paused;
    }
}
