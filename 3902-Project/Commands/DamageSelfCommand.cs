using Project.App;

namespace Project.Commands;
public class DamageSelfCommand : ICommand
{
    private readonly Game1 _game;

    public DamageSelfCommand(Game1 game)
    {
        _game = game;
    }

    public void Execute()
    {
        _game.Player.TakeDamage(1f);
    }
}
