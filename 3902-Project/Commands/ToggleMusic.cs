using Project.App;

namespace Project.Commands;

public class ToggleMusic : ICommand
{
    private readonly Game1 _game;

    public ToggleMusic(Game1 currentGame)
    {
        _game = currentGame;
    }

    public void Execute()
    {
        SoundManager.Instance.MusicToggle();
    }
}
