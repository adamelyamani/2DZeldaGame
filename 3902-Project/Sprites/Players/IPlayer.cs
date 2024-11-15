using Microsoft.Xna.Framework;
using Project.Interfaces;
using Project.Sprites.Items;

namespace Project.Sprites.Players
{
    public interface IPlayer : ISprite, ICollidable
    {
        Vector2 Position { get; set; }

        float Health { get; set; }

        float Shield { get; set; }

        float Speed { get; set; }

        bool IsAlive { get; set; }

        bool IsDamaged { get; set; }
        
        Vector2 Move(Vector2 direction);

        Vector2 Dash(Vector2 direction);

        Hand LeftHand { get; set; }

        Hand RightHand { get; set; }

        void InitAnimations(Animation idle, Animation moving, Animation death);

        void Attack();

        void TakeDamage(float damage);

        void Equip(IItem  item);
    }
}
