using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Sprites.Enemies.PathFinding;

namespace Project.Sprites.Enemies
{
    public class SkeletonBase : ActionPatternEnemy
    {
        private const int BoundingBoxHeightValue = 64;
        private const int BoundingBoxWidthValue = 36;
        private const int BoundingBoxXOffsetValue = 48;
        private const int BoundingBoxYOffsetValue = 64;
        private const int MaxHealthValue = 100;
        private new const int AttackDamage = 10;
        private new const int AttackTime = 150;

        private const int moveTime = 1500;

        public override int BoundingBoxHeight => BoundingBoxHeightValue;
        public override int BoundingBoxWidth => BoundingBoxWidthValue;
        public override int BoundingBoxXOffset => BoundingBoxXOffsetValue;
        public override int BoundingBoxYOffset => BoundingBoxYOffsetValue;
        public sealed override int MaxHealth { get; set; }

        private ActionPattern MovePattern;

        public SkeletonBase(SpriteBatch spriteBatch, Game1 game)
        {
            Texture = Texture = game.Content.Load<Texture2D>("SkeletonBaseAtlas");
            DeathTex = game.Content.Load<Texture2D>("SkeletonBaseDeath");
            DeathSfx = SfxEnums.SkeletonDeath;
            AttackSfx = SfxEnums.SkeletonAttack;
            DamageSfx = SfxEnums.SkeletonHit;
            GameObject = game;
            Speed = 0.10f;
            Rows = 1;
            Row = 0;
            Columns = 6;
            Height = 128;
            Width = 764;
            CurrentFrame = 0;
            CurrentDeathFrame = 0;
            TotalFrames = Rows * Columns - 1;
            Position = new Vector2(300, 200);
            SpriteBatchObject = spriteBatch;
            RangedEnemy = false;
            MaxHealth = MaxHealthValue;
            Health = MaxHealthValue;
            base.AttackDamage = AttackDamage;
            base.AttackTime = AttackTime;
            IsAlive = true;
            IsDamaged = false;
            SpriteColor = Color.White;
        }

        public override void Update(GameTime gameTime)
        {
            if(Dying)
                SetDeathTex();

            base.Update(gameTime);
        }

        protected override void IdleNoticeAction()
        {
            if (MovePattern == null)
                InitMoveActionPattern();

            MovePattern.Reset();

            base.IdleNoticeAction();
        }

        protected override void IdleAction(GameTime time)
        {
            MovePattern.Update(time);

            base.IdleAction(time);
        }

        // Action Pattern Methods
        private void InitMoveActionPattern()
        {
            // Define Action Pattern
            MovePattern = new ActionPattern();

            // start by making sure it's at the starting position
            int[] mas = new int[2] { (int)Spawn.X, (int)Spawn.Y };
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
            Columns = 8;
            Height = 128;
            Width = 1536;
            TotalFrames = 7;
            TextureOffsetX = 16;
            TextureOffsetY = 0;
        }
    }
}