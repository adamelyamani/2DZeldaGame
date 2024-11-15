using Microsoft.Xna.Framework;
using Project.Interfaces;

namespace Project.Sprites.Environment
{
    public interface IEnvironment : ISprite, ICollidable
    {
        public Vector2 Position { get; set; }

        public bool IsCollidable { get; set; } 
    }
}
