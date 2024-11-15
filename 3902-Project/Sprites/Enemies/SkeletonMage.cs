using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Sprites.Enemies.PathFinding;
using Project.Sprites.Projectiles;

namespace Project.Sprites.Enemies
{
    public class SkeletonMage : ActionPatternEnemy
    {
        private const int BoundingBoxHeightValue = 64;
        private const int BoundingBoxWidthValue = 36;
        private const int BoundingBoxXOffsetValue = 16;
        private const int BoundingBoxYOffsetValue = 0;
        private const int MaxHealthValue = 80;
        private new const int AttackDamage = 20;
        private new const int AttackTime = 250;
        private const float ProjectileSpeed = 0.2f;
        private const int ProjectileRadius = 25;
        private const int moveTime = 500;
        private const int squareMoveOffsetX = 50;
        private const int squareMoveOffsetY = 50;

        private readonly ProjectileManager _projectileManager = ProjectileManager.Instance;

        private ActionPattern MovePattern;
        private ActionPattern AttackPattern;

        public SkeletonMage(SpriteBatch spriteBatch, Game1 game)
        {
            Texture = Texture = game.Content.Load<Texture2D>("SkeletonMageAtlas");
            DeathTex = game.Content.Load<Texture2D>("SkeletonMageDeath");
            DeathSfx = SfxEnums.SkeletonDeath;
            AttackSfx = SfxEnums.SkeletonAttack;
            DamageSfx = SfxEnums.SkeletonHit;
            Speed = 0.10f;
            Rows = 2;
            Row = 0;
            Columns = 4;
            Height = 128;
            Width = 256;
            CurrentFrame = 0;
            CurrentDeathFrame = 0;
            TotalFrames = Rows / 2 * Columns - 1;
            SpriteBatchObject = spriteBatch;
            GameObject = game;
            Position = new Vector2(200, 200);
            RangedEnemy = true;
            MaxHealth = MaxHealthValue;
            Health = MaxHealthValue;
            base.AttackDamage = AttackDamage;
            base.AttackTime = AttackTime;
            IsAlive = true;
            IsDamaged = false;
            SpriteColor = Color.White;
        }

        public override int BoundingBoxHeight => BoundingBoxHeightValue;

        public override int BoundingBoxWidth => BoundingBoxWidthValue;

        public override int BoundingBoxXOffset => BoundingBoxXOffsetValue;

        public override int BoundingBoxYOffset => BoundingBoxYOffsetValue;

        public sealed override int MaxHealth { get; set; }

        public override void Update(GameTime gameTime)
        {
            if (Dying)
                SetDeathTex();
            else
            {
                if (MovePattern == null)
                    InitMoveActionPattern();

                MovePattern.Update(gameTime);
            }

            base.Update(gameTime);
        }

        protected override void AlertNoticeAction(Vector2 pos)
        {
            return;
        }
        protected override void AlertedAction(GameTime time)
        {
            if (AttackPattern == null)
                InitAttackActionPattern();

            AttackPattern.Update(time);
        }

        private void FireProjectile(float angle)
        {
            Vector2 pos = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * ProjectileRadius + GetPosition();

            _projectileManager.AddProjectile(new Projectile(ProjectileEnums.Fireball, SpriteBatchObject, GameObject, this, pos, angle, AttackDamage, ProjectileSpeed));

            SoundManager.Instance.PlaySound(AttackSfx);
        }

        // Action Pattern events
        private void FireAction(GameTime time, int time_since_action, int[] settings)
        {
            Vector2 player = GetPlayerPosition();
            Vector2 self = GetPosition();

            Vector2 dir = player - self;
            dir.Normalize();

            float angle = (float)Math.Atan2(dir.Y, dir.X);

            FireProjectile(angle);
        }

        // Init Action Patterns
        private void InitAttackActionPattern()
        {
            AttackPattern = new ActionPattern();

            // Fire
            AttackPattern.AddAction(FireAction, SingleUseCondition, null, null);

            // Cooldown
            int[] time = new int[1] { AttackTime };
            AttackPattern.AddAction(IdleAction, TimeCondition, null, time);
        }

        private void InitMoveActionPattern()
        {
            MovePattern = new ActionPattern();

            // Move to starting position
            int[] mas = new int[2] { (int)Spawn.X, (int)Spawn.Y};
            MovePattern.AddAction(SetPathToPosAction, SingleUseCondition, mas, null);
            MovePattern.AddAction(FollowPathAction, FinishedPathCondition, null, null);

            // Square Movement Condition and actions
            int[] time = new int[1] { moveTime };

            MovePattern.AddAction(MoveAction, TimeCondition, new int[2] { 1, 0 }, time);
            MovePattern.AddAction(MoveAction, TimeCondition, new int[2] { 0, 1 }, time);
            MovePattern.AddAction(MoveAction, TimeCondition, new int[2] { -1, 0 }, time);
            MovePattern.AddAction(MoveAction, TimeCondition, new int[2] { 0, -1 }, time);
        }

        private void SetDeathTex()
        {
            Texture = DeathTex;
            Row = 0;
            Rows = 1;
            Columns = 6;
            Height = 128;
            Width = 768;
            TotalFrames = 5;
            TextureOffsetX = 16;
            TextureOffsetY = 64;
        }
    }
}