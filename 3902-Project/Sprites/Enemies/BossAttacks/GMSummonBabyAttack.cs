using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using System.Collections.Generic;

namespace Project.Sprites.Enemies.BossAttacks
{
    public class GMSummonBabyAttack : IBossAttack
    {
        private const float ChargeTime = 2000;
        private const float CastTime = 1000;
        private const float CooldownTime = 1000;

        private const float SummonDist = 50.0f;
        private const int CastParticleCount = 100;
        private const float CastParticleRadius = 15.0f;
        private const float CastParticleSize = 8.0f;

        private const float SpinSummonMultiplier = 1.1f;
        private const float SpinChargeIncrement = (float)Math.PI / 20;

        private int _state; // 0 idle, 1 charging, 2 summoning
        private float _stateTimer; // Timer since last state
        private bool _exitFlag;

        readonly Texture2D _castTex;

        private readonly Random _random;

        private readonly List<float> _castParticles;
        private float _spin;
        private float _retraction;

        // Vars to copy
        private readonly Enemy _enemy;

        public GMSummonBabyAttack(Enemy enemy)
        {
            this._enemy = enemy;

            _castTex = new Texture2D(this._enemy.GameObject.GraphicsDevice, 1, 1);
            _castTex.SetData(new[] { Color.Red });

            _random = new Random();

            _castParticles = new List<float>();
            _spin = 0;
            _retraction = 0;

            _state = 0;
            _stateTimer = 0;
            _exitFlag = false;
        }

        public bool Update(float elapsedTime, Vector2 bossPosition, Vector2 targetPosition)
        {
            _stateTimer += elapsedTime;

            switch (_state)
            {
                case 0: // idle
                    SetState(1);
                    break;
                case 1: // charging
                    // charge
                    ChargeParticles(_stateTimer);
                    if (_stateTimer > ChargeTime)
                        SetState(2);
                    break;
                case 2: // summoning
                    UpdateSummon(_stateTimer, CastTime);
                    if (_stateTimer > CastTime) {
                        CreateMob(GetPosOnRadius(bossPosition, SummonDist));
                        SetState(3);
                    }
                    break;
                case 3: // end
                    if (_stateTimer > CooldownTime)
                        _exitFlag = true;
                    break;
            }

            // Clean up old laser tex
            if (_exitFlag)
                _castTex.Dispose();

            return _exitFlag;
        }

        public void Draw()
        {
            if (_state == 1 || _state == 2) {
                DrawCastParticles(_enemy.GetPosition(), _castParticles, _spin, (int) (CastParticleRadius - _retraction), (int) CastParticleSize);
            }

        }

        void ChargeParticles(float time)
        {
            int targetParticles = (int) (CastParticleCount * time / ChargeTime);
            while (_castParticles.Count < targetParticles)
            {
                _castParticles.Add(RandomFloat(0, 2 * (float) Math.PI));
            }

            _spin += SpinChargeIncrement;
        }

        void UpdateSummon(float time, float completionTime)
        {
            _spin *= SpinSummonMultiplier;
            _retraction = (time / completionTime) * CastParticleRadius;
        }

        void CreateMob(Vector2 targetPosition)
        {
            IEnemy mob = new SkeletonRogue(_enemy.SpriteBatchObject, _enemy.GameObject);
            mob.Position = targetPosition;

            _enemy.GameObject.CurrentLevel.SummonEnemy(mob);
            SoundManager.Instance.PlaySound(_enemy.AttackSfx);
        }

        Vector2 GetPosOnRadius(Vector2 origin, float radius)
        {
            double angle = RandomFloat(0, (float)Math.PI);
            Vector2 pos = new Vector2((float)Math.Cos(angle), (float) Math.Sin(angle));
            return pos * radius + origin;
        }

        public float RandomFloat(float minValue, float maxValue)
        {
            return (float) _random.NextDouble() * (maxValue - minValue) + minValue;
        }

        // Provide the size of the laser and width
        void DrawCastParticles(Vector2 bossPosition, List<float> particles, float spin, int radius, int particleSize)
        {
            Rectangle destinationRectangle = new((int)bossPosition.X + radius, (int)bossPosition.Y, particleSize, particleSize);

            Vector2 origin = new Vector2(-radius, 0);

            _enemy.SpriteBatchObject.Begin();
            for (int i = 0; i < particles.Count; i++) {
                _enemy.SpriteBatchObject.Draw(_castTex, destinationRectangle, null, Color.Red, particles[i] + spin, origin, SpriteEffects.None, 0);
            }
            _enemy.SpriteBatchObject.End();
        }

        void SetState(int state)
        {
            this._state = state;
            this._stateTimer = 0;
        }
    }
}
