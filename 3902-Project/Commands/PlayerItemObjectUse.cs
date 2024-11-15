using Project.App;

namespace Project.Commands;
public class PlayerItemObjectUse : ICommand
{
    private readonly Game1 _game;

    public PlayerItemObjectUse(Game1 currentGame)
    {
        _game = currentGame;
    }

    public void Execute()
    {
        if (_game.Player.IsAlive && _game.GameState is GameStateEnums.Running)
        {
            _game.Player.LeftHand.HeldItem?.Use();
        }
    }
}