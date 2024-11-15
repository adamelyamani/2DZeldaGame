using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Sprites.Projectiles;

namespace Project.Sprites.Enemies.BossAttacks
{
    public class GMChargeAttack : IBossAttack
    {
        private const float ChargeTime = 1000;
        private const float FireTime = 500;
        private const float CooldownTime = 1000;

        private const float ProjSpeed = 1.0f;
        private const float ProjDmg = 50f;
        private const float ProjDistOffset = 16;

        private const float LaserLength = 300.0f;
        private const float LaserWidth = 15.0f;

        private float _fireAngle;

        private int _state; // 0 charging, 1 aiming, 2 firing
        private float _stateTimer; // Timer since last state
        private bool _exitFlag;

        readonly Texture2D _laserTex;

        // Vars to copy
        private readonly ProjectileManager _projectileManager = ProjectileManager.Instance;
        private readonly Enemy _enemy;

        public GMChargeAttack(Enemy enemy)
        {
            _enemy = enemy;

            _laserTex = new Texture2D(this._enemy.GameObject.GraphicsDevice, 1, 1);
            _laserTex.SetData(new[] { Color.Red } );

            _state = 0;
            _stateTimer = 0;
            _exitFlag = false;

            _fireAngle = 0;
        }

        public bool Update(float elapsedTime, Vector2 bossPosition, Vector2 targetPosition)
        {
            _stateTimer += elapsedTime;

            switch (_state)
            {
                case 0:
                    SetState(1);
                    break;
                case 1:
                    SetFireAngle(bossPosition, targetPosition);
                    if (_stateTimer > ChargeTime)
                        SetState(2);
                    break;
                case 2:
                    FireShot(bossPosition, targetPosition);
                    if (_stateTimer > FireTime)
                        SetState(3);
                    break;
                case 3:
                    if (_stateTimer > CooldownTime)
                        _exitFlag = true;
                    break;
            }

            // Clean up old laser tex
            if (_exitFlag)
                _laserTex.Dispose();

            return _exitFlag;
        }

        public void Draw()
        {
            if (_state == 1)
            {
                DrawLaser(_enemy.GetPosition(), _fireAngle, _stateTimer * LaserLength / ChargeTime, _stateTimer * LaserWidth / ChargeTime);
            }
        }

        private void FireShot(Vector2 bossPosition, Vector2 targetPosition)
        {
            Vector2 targetDir = targetPosition - bossPosition;
            targetDir.Normalize();

            _projectileManager.AddProjectile(new Projectile(ProjectileEnums.FireballLarge, _enemy.SpriteBatchObject, _enemy.GameObject, _enemy, bossPosition + targetDir * ProjDistOffset, _fireAngle, (int)ProjDmg, ProjSpeed));
            SoundManager.Instance.PlaySound(_enemy.AttackSfx);
        }

        private void SetFireAngle(Vector2 bossPosition, Vector2 targetPosition)
        {
            Vector2 targetDir = targetPosition - bossPosition;
            
            _fireAngle = (float)Math.Atan2(targetDir.Y, targetDir.X);
        }

        // Provide the size of the laser and width
        private void DrawLaser(Vector2 bossPosition, float fireAngle, float length, float width)
        {
            Rectangle destinationRectangle = new((int)bossPosition.X - (int)ProjDistOffset, (int)bossPosition.Y, (int) length, (int) width);

            _enemy.SpriteBatchObject.Begin();
            _enemy.SpriteBatchObject.Draw(_laserTex, destinationRectangle, null, Color.Red, fireAngle + 2 * ((float) Math.PI), Vector2.Zero, SpriteEffects.None, 0);
            _enemy.SpriteBatchObject.End();
        }

        private void SetState(int newState)
        {
            _state = newState;
            _stateTimer = 0;
        }
    }
}
