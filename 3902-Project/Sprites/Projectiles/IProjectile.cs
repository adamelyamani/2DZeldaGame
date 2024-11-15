using Project.App;
using Project.Interfaces;

namespace Project.Sprites.Projectiles
{
    public interface IProjectile : ISprite, ICollidable
    {
        int Damage { get; set; }

        float Speed { get; set; }

        float Angle { get; set; }

        bool Live { get; set; }

        ISprite Owner { get; set; }

        RotatedRectangle RotatedRectangle { get; }

        void Hit();
    }
}

