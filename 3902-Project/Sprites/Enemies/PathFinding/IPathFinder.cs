using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Interfaces;
using Project.Sprites.Players;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Project.Sprites.Enemies.PathFinding
{
    public interface IPathFinder
    {
        public Vector2 start { get; }
        public Vector2 target { get; }

        public Vector2 GetTargetAlongPath(Vector2 pos);
        public void Draw(Game1 game, SpriteBatch obj);

        public bool PathFound();
    }
}
