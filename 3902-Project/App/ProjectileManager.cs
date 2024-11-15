using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Project.Sprites.Projectiles;

namespace Project.App
{
    public class ProjectileManager
    {
        private static ProjectileManager _instance;
        private readonly List<IProjectile> _projectiles = new();

        public static ProjectileManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ProjectileManager();
                }

                return _instance;
            }
        }

        public void AddProjectile(IProjectile projectile)
        {
            _projectiles.Add(projectile);
        }

        public void RemoveProjectile(IProjectile projectile)
        {
            _projectiles.Remove(projectile);
        }

        public void ClearProjectiles()
        {
            _projectiles.Clear();
        }

        public List<IProjectile> GetProjectiles()
        {
            return _projectiles;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var projectile in _projectiles)
            {
                projectile.Update(gameTime);
            }

            _projectiles.RemoveAll(projectile => projectile.Live == false);
        }

        public void Draw()
        {
            foreach (var projectile in _projectiles)
            {
                projectile.Draw();
            }
        }
    }
}
