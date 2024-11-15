using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Sprites.Enemies.PathFinding;
using Project.Sprites.Players;
using Project.Sprites.Projectiles;
using System;
using System.Runtime.CompilerServices;

namespace Project.Sprites.Enemies
{
    public class OrcMage : ActionPatternEnemy
    {
        private const int BoundingBoxHeightValue = 50;
        private const int BoundingBoxWidthValue = 55;
        private const int BoundingBoxXOffsetValue = 5;
        private const int BoundingBoxYOffsetValue = 16;
        private const int MaxHealthValue = 200;
        private const int AttackDamageValue = 30;
        private const int AttackTimeValue = 1000;
        private const float ProjectileSpeedValue = 0.2f;

        private const int ProjectileRadius = 25;
        private const int fleeTime = 2000;
        private const int wanderTime = 500;
        private const int wanderRadius = 300;
        private const int wanderPosAttempts = 20;

        private readonly Texture2D _runTex;
        private readonly Texture2D _idleTex;
        private readonly int _idleHeight;
        private readonly int _idleWidth;
        private readonly int _runHeight;
        private readonly int _runWidth;
        private readonly int _deathHeight;
        private readonly int _deathWidth;
        private readonly int _totalIdleFrames;
        private readonly int _totalRunFrames;
        private readonly int _totalDeathFrames;
        private readonly int _idleCols;
        private readonly int _runCols;
        private readonly int _deathCols;

        private bool fleeing;

        public override int BoundingBoxHeight => BoundingBoxHeightValue;
        public override int BoundingBoxWidth => BoundingBoxWidthValue;
        public override int BoundingBoxXOffset => BoundingBoxXOffsetValue;
        public override int BoundingBoxYOffset => BoundingBoxYOffsetValue;
        public sealed override int MaxHealth { get; set; }

        private readonly ProjectileManager _projectileManager = ProjectileManager.Instance;

        // Attack Pattern Code
        // Starts Wandering within circle idling every few seconds
        // When Alerted, Stay in place and shoot fireballs at player
        // When Damaged, Run away from player until hit cooldown
        // When Hit Cooldown is over, resume wandering

        private ActionPattern WanderPattern;
        private ActionPattern AttackPattern;
        private ActionPattern FleePattern;

        public OrcMage(SpriteBatch spriteBatch, Game1 game)
        {
            Texture = _idleTex = game.Content.Load<Texture2D>("OrcShamanAtlas2");
            _runTex = game.Content.Load<Texture2D>("OrcShamanRun");
            DeathTex = game.Content.Load<Texture2D>("OrcShamanDeath");
            DeathSfx = SfxEnums.OrcDeath;
            AttackSfx = SfxEnums.OrcAttack;
            DamageSfx = SfxEnums.OrcHit;
            GameObject = game;
            Speed = 0.10f;
            Rows = 1;
            Row = 0;
            Columns = _idleCols = 4;
            _runCols = 6;
            _deathCols = 7;
            Height = _idleHeight = 64;
            Width = _idleWidth = 256;
            _runHeight = 128;
            _runWidth = 768;
            _deathHeight = 128;
            _deathWidth = 896;
            TextureOffsetX = 0;
            TextureOffsetY = 0;
            CurrentFrame = 0;
            TotalFrames = _totalIdleFrames = Rows * Columns - 1;
            _totalRunFrames = 11;
            _totalDeathFrames = 6;
            CurrentDeathFrame = 0;
            SpriteBatchObject = spriteBatch;
            Position = new Vector2(600, 200);
            RangedEnemy = true;
            MaxHealth = MaxHealthValue;
            Health = MaxHealthValue;
            AttackDamage = AttackDamageValue;
            AttackTime = AttackTimeValue;
            Dying = false;
            IsAlive = true;
            IsDamaged = false;

            fleeing = false;

            SpriteColor = Color.White;
        }

        public override void Update(GameTime gameTime)
        {
            if (Dying)
                SetDeathTex();

            base.Update(gameTime);
        }

        protected override void AlertNoticeAction(Vector2 pos)
        {
            // Skip base method which tracks to player
        }

        protected override void AlertedAction(GameTime time)
        {
            if (!fleeing)
            {
                if (AttackPattern == null)
                    InitAttackActionPattern();

                SetIdleTex();
                AttackPattern.Update(time);
            }
            else
            {
                SetRunTex();
                FleePattern.Update(time);
            }

            // skip base method which moves to player
        }

        protected override void IdleNoticeAction()
        {
            if (WanderPattern == null)
                InitWanderActionPattern();

            WanderPattern.Reset();

            base.IdleNoticeAction();
        }
        protected override void IdleAction(GameTime time)
        {
            WanderPattern.Update(time);
            SetIdleTex();

            base.IdleAction(time);
        }

        private void FireAtPlayer()
        {
            Vector2 player = GetPlayerPosition();
            Vector2 self = GetPosition();

            Vector2 dir = player - self;
            dir.Normalize();

            float angle = (float)Math.Atan2(dir.Y, dir.X);

            FireProjectile(angle);
        }

        private void FireProjectile(float angle)
        {
            Vector2 pos = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * ProjectileRadius + GetPosition();

            _projectileManager.AddProjectile(new Projectile(ProjectileEnums.Fireball, SpriteBatchObject, GameObject, this, pos, angle, AttackDamageValue, ProjectileSpeedValue));
            
            SoundManager.Instance.PlaySound(AttackSfx);
        }

        // Action Pattern events
        private void FireAction(GameTime time, int time_since_action, int[] settings)
        {
            FireAtPlayer();
        }

        // Init Patterns
        private void InitWanderActionPattern()
        {
            WanderPattern = new ActionPattern();

            // choose a random position within a radius of the spawn point
            int[] mas = new int[4] { (int)Spawn.X, (int)Spawn.Y, wanderRadius, wanderPosAttempts };
            WanderPattern.AddAction( SetPathToPosInRadiusAction, SingleUseCondition, mas, null);

            // Follow Path
            WanderPattern.AddAction(FollowPathAction, FinishedPathCondition, null, null);

            // Idle at destination
            int[] time = new int[1] { wanderTime };
            WanderPattern.AddAction(IdleAction, TimeCondition, null, time);
        }
        private void InitAttackActionPattern()
        {
            AttackPattern = new ActionPattern();

            // Fire
            AttackPattern.AddAction(FireAction, SingleUseCondition, null, null);

            // Cooldown
            int[] time = new int[1] { AttackTime };
            AttackPattern.AddAction(IdleAction, TimeCondition, null, time);
        }

        private void InitFleeActionPattern()
        {
            FleePattern = new ActionPattern(FinishedFleeing);

            // Cooldown
            int[] time = new int[1] { fleeTime };
            FleePattern.AddAction(FleeAction, TimeCondition, null, time);
        }

        // Start fleeing when taken damage
        public override void TakeDamage(float dmg)
        {
            if (FleePattern == null)
                InitFleeActionPattern();
            else
                FleePattern.Reset();

            fleeing = true;

            base.TakeDamage(dmg);
        }

        // Cancel the melee attack action
        public override void Attack(IPlayer player)
        {
            //base.Attack(player);
        }

        // Stop Fleeing when done with a single iteration (Callback
        private void FinishedFleeing(int iterations)
        {
            fleeing = false;
        }

        private void SetIdleTex()
        {
            Texture = _idleTex;
            Columns = _idleCols;
            Height = _idleHeight;
            Width = _idleWidth;
            TotalFrames = _totalIdleFrames;
            TextureOffsetX = 0;
            TextureOffsetY = 0;
        }

        private void SetRunTex()
        {
            Texture = _runTex;
            Columns = _runCols;
            Height = _runHeight;
            Width = _runWidth;
            TotalFrames = _totalRunFrames;
            TextureOffsetX = 32;
            TextureOffsetY = 64;
        }

        private void SetDeathTex()
        {
            Texture = DeathTex;
            Row = 0;
            Columns = _deathCols;
            Height = _deathHeight;
            Width = _deathWidth;
            TotalFrames = _totalDeathFrames;
            TextureOffsetX = 32;
            TextureOffsetY = 64;
        }
    }
}