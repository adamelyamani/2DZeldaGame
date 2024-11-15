using Microsoft.Xna.Framework;
using Project.Interfaces;
using Project.Sprites.Players;

namespace Project.Sprites.Enemies
{
    public interface IEnemy : ISprite, ICollidable
    {
        public Vector2 Position { get; set; }

        public Vector2 Spawn { get; set; }

        public float Health { get; set; }

        public int MaxHealth { get; set; }
        
        public int AttackDamage { get; set; }

        public int AttackTime { get; set; }

        public bool IsAlive { get; set; }

        public bool IsDamaged { get; set; }

        public bool Dying {  get; set; }

        public void Attack(IPlayer player);

        public void TakeDamage(float damage);
    }
}
  