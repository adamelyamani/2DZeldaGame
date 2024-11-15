using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;

namespace Project.Sprites.Players
{
    public class Wizard : Player
    {
        private const float StartingSpeed = 37.5f * 0.05f;
        private const float StartingHealth = 250;

        public Wizard(SpriteBatch spriteBatch, Game1 game) : base(spriteBatch, game, PlayerStateEnums.Idle, StartingHealth, StartingSpeed, true)
        {
            // Custom Logic
            var idleTexture = game.Content.Load<Texture2D>("W-Idle-Sheet");
            var runTexture = game.Content.Load<Texture2D>("W-Run-Sheet");
            var deathTexture = game.Content.Load<Texture2D>("W-Death-Sheet");

            var idle = new Animation(
                "idle", 
                idleTexture, 
                4, 
                new Vector2(0, 0), 
                new Vector2(idleTexture.Width / 4f, idleTexture.Height), 
                300
            );

            var move = new Animation(
                "moving",
                runTexture,
                6,
                new Vector2(0, 0),
                new Vector2(runTexture.Width / 6f, runTexture.Height),
                400,
                new Vector2(0, 1)
            );

            var death = new Animation(
                "death",
                deathTexture,
                6,
                new Vector2(0, 0),
                new Vector2(deathTexture.Width / 6f, deathTexture.Height),
                1000
            );

            InitAnimations(idle, move, death);
        }
    }
}
