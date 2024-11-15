using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Sprites.Enemies.PathFinding;

namespace Project.Sprites.Enemies
{
    public class SkeletonRogue : ActionPatternEnemy
    {
        private const int BoundingBoxHeightValue = 70;
        private const int BoundingBoxWidthValue = 36;
        private const int BoundingBoxXOffsetValue = 48;
        private const int BoundingBoxYOffsetValue = 64;
        private const int MaxHealthValue = 200;
        private const int AttackDamage = 10;
        private const int AttackTime = 75;

        private const int paceTime = 1500;
        private const int stunTime = 1000;
        private const int fleeTime = 1500;


        private const float paceSpeed = 0.0625f;
        private const float approachSpeed = paceSpeed * 2.5f;
        private const float fleeSpeed = paceSpeed * 2;

        private ActionPattern PacePattern;
        private ActionPattern FleePattern;

        private bool fleeing;

        public SkeletonRogue(SpriteBatch spriteBatch, Game1 game)
        {
            Texture = Texture = game.Content.Load<Texture2D>("SkeletonRogueAtlas");
            DeathTex = game.Content.Load<Texture2D>("SkeletonRogueDeath");
            DeathSfx = SfxEnums.SkeletonDeath;
            AttackSfx = SfxEnums.SkeletonAttack;
            DamageSfx = SfxEnums.SkeletonHit;
            GameObject = game;
            Speed = 0.125f;
            Rows = 2;
            Row = 0;
            Columns = 6;
            Height = 256;
            Width = 764;
            CurrentFrame = 0;
            CurrentDeathFrame = 0;
            TotalFrames = Rows / 2 * Columns - 1;
            Position = new Vector2(200, 320);
            SpriteBatchObject = spriteBatch;
            RangedEnemy = false;
            MaxHealth = MaxHealthValue;
            Health = MaxHealthValue;
            base.AttackDamage = AttackDamage;
            base.AttackTime = AttackTime;
            IsAlive = true;
            IsDamaged = false;
            SpriteColor = Color.White;

            fleeing = false;
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

            base.Update(gameTime);
        }

        // Ensures the Square Movement Action Pattern is initialized and reset
        protected override void IdleNoticeAction()
        {
            if (PacePattern == null)
                InitPaceActionPattern();

            PacePattern.Reset();

            base.IdleNoticeAction();
        }

        protected override void IdleAction(GameTime time)
        {
            Speed = paceSpeed;
            PacePattern.Update(time);

            base.IdleAction(time);
        }

        protected override void AlertedAction(GameTime time)
        {
            // Do base of run at player
            if (!fleeing)
            {
                Speed = approachSpeed;
                base.AlertedAction(time);
            }
            else
            {
                Speed = fleeSpeed;
                FleePattern.Update(time);
            }

            // skip base method which moves to player
        }

        // Action Pattern Code
        // Custom Methods

        // Init methods
        private void InitPaceActionPattern()
        {
            PacePattern = new ActionPattern();

            int[] mcas = new int[2] { 1, 0 };
            int[] time = new int[1] { paceTime };
            PacePattern.AddAction(MoveAction, TimeCondition, mcas, time);

            int[] mcas2 = new int[2] { -1, 0 };
            PacePattern.AddAction(MoveAction, TimeCondition, mcas2, time);
        }

        private void InitFleeActionPattern()
        {
            FleePattern = new ActionPattern(FinishedFleeing);

            // Stun
            int[] time = new int[1] { stunTime };
            FleePattern.AddAction(IdleAction, TimeCondition, null, time);

            // Running
            time = new int[1] { fleeTime };
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

        // Stop Fleeing when done with a single iteration (Callback
        private void FinishedFleeing(int iterations)
        {
            fleeing = false;
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
            TextureOffsetX = -16;
            TextureOffsetY = -16;
        }
    }
}