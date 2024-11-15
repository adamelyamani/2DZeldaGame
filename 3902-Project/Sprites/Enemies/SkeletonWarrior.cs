using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Sprites.Enemies.PathFinding;
using Project.Sprites.Players;

namespace Project.Sprites.Enemies
{
    public class SkeletonWarrior : ActionPatternEnemy
    {
        private const int BoundingBoxHeightValue = 64;
        private const int BoundingBoxWidthValue = 40;
        private const int BoundingBoxXOffsetValue = 48;
        private const int BoundingBoxYOffsetValue = 64;
        private const int MaxHealthValue = 500;
        private new const int AttackDamage = 40;
        private new const int AttackTime = 500;

        private const int moveTime = 500;

        private ActionPattern MoveInEight;

        public SkeletonWarrior(SpriteBatch spriteBatch, Game1 game)
        {
            Texture = Texture = game.Content.Load<Texture2D>("WarriorAtlas");
            DeathTex = game.Content.Load<Texture2D>("SkeletonWarriorDeath");
            DeathSfx = SfxEnums.SkeletonDeath;
            AttackSfx = SfxEnums.SkeletonAttack;
            DamageSfx = SfxEnums.SkeletonHit;
            GameObject = game;
            Speed = 0.075f;
            Rows = 2;
            Row = 0;
            Columns = 6;
            Height = 256;
            Width = 764;
            CurrentFrame = 0;
            CurrentDeathFrame = 0;
            TotalFrames = Rows / 2 * Columns - 1;
            Position = new Vector2(300, 200);
            SpriteBatchObject = spriteBatch;
            Health = MaxHealthValue;
            MaxHealth = MaxHealthValue;
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

        public override void Update(GameTime time)
        {
            if (Dying)
                SetDeathTex();

            base.Update(time);
        }

        protected override void IdleNoticeAction()
        {
            if (MoveInEight == null)
                InitEightActionPattern();

            MoveInEight.Reset();

            base.IdleNoticeAction();
        }

        protected override void IdleAction(GameTime time)
        {
            MoveInEight.Update(time);

            base.IdleAction(time);
        }

        // Action Pattern Methods
        private void InitEightActionPattern()
        {
            // Define Action Pattern
            MoveInEight = new ActionPattern();

            // start by making sure it's at the starting position
            int[] mas = new int[2] { (int)Spawn.X, (int)Spawn.Y };
            MoveInEight.AddAction(SetPathToPosAction, SingleUseCondition, mas, null);
            MoveInEight.AddAction(FollowPathAction, FinishedPathCondition, null, null);

            // Eight Movement Condition and actions
            int[] time = new int[1] { moveTime };

            MoveInEight.AddAction(MoveAction, TimeCondition, new int[2] { 1, 0 }, time);
            MoveInEight.AddAction(MoveAction, TimeCondition, new int[2] { 0, 1 }, time);
            MoveInEight.AddAction(MoveAction, TimeCondition, new int[2] { -1, 0 }, time);
            MoveInEight.AddAction(MoveAction, TimeCondition, new int[2] { 0, -1 }, time);
            MoveInEight.AddAction(MoveAction, TimeCondition, new int[2] { -1, 0 }, time);
            MoveInEight.AddAction(MoveAction, TimeCondition, new int[2] { 0, 1 }, time);
            MoveInEight.AddAction(MoveAction, TimeCondition, new int[2] { 1, 0 }, time);
            MoveInEight.AddAction(MoveAction, TimeCondition, new int[2] { 0, -1 }, time);
        }

        private void SetDeathTex()
        {
            Texture = DeathTex;
            Row = 0;
            Rows = 1;
            Columns = 6;
            Height = 96;
            Width = 768;
            TotalFrames = 5;
            TextureOffsetX = -16;
            TextureOffsetY = -32;
        }
    }
}