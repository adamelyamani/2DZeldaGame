using Microsoft.Xna.Framework;

namespace Project.Interfaces;

public interface ICollidable
{
    Rectangle BoundingRectangle { get; }
}