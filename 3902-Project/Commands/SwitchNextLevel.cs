using Project.App;

namespace Project.Commands;

public class SwitchNextLevel : ICommand
{
    private readonly Game1 _game;

    public SwitchNextLevel(Game1 currentGame)
    {
        _game = currentGame;
    }

    public void Execute()
    {
        _game.CurrentLevelIndex = ((_game.CurrentLevelIndex + 1) % _game.FullMap.Count + _game.FullMap.Count) % _game.FullMap.Count;
        _game.CurrentLevel = _game.FullMap[_game.CurrentLevelIndex].Item1;
    }
}
