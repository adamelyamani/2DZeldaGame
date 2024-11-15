using Project.App;

namespace Project.Commands;

public class SwitchPreviousLevel : ICommand
{
    private readonly Game1 _game;

    public SwitchPreviousLevel(Game1 currentGame)
    {
        _game = currentGame;
    }

    public void Execute()
    {
        _game.CurrentLevelIndex = ((_game.CurrentLevelIndex - 1) % _game.FullMap.Count + _game.FullMap.Count) % _game.FullMap.Count;
        _game.CurrentLevel = _game.FullMap[_game.CurrentLevelIndex].Item1;
   }
}
