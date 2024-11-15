
using Microsoft.Xna.Framework;

namespace Project.Sprites.Enemies.BossAttacks
{
    public interface IBossAttack
    {
        // returns true if complete
        public bool Update(float elapsedTime, Vector2 bossPosition, Vector2 targetPosition);

        public void Draw();
    }
}
