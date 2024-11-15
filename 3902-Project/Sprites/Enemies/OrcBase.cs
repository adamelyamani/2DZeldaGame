using System;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Sprites.Enemies.PathFinding;
using System.Diagnostics;

namespace Project.Sprites.Enemies
{
    public class OrcBase : ActionPatternEnemy
    {
        private const int BoundingBoxHeightValue = 64;
        private const int BoundingBoxWidthValue = 44;
        private const int BoundingBoxXOffsetValue = 44;
        private const int BoundingBoxYOffsetValue = 64;
        private const int MaxHealthValue = 150;
        private const int AttackDamageValue = 15;
        private const int AttackTimeValue = 200;
        private const int alert_distance = 400;
        private const int pace_time = 500;

        private ActionPattern PacePattern;

        public override int BoundingBoxHeight => BoundingBoxHeightValue;
        public override int BoundingBoxWidth => BoundingBoxWidthValue;
        public override int BoundingBoxXOffset => BoundingBoxXOffsetValue;
        public override int BoundingBoxYOffset => BoundingBoxYOffsetValue;
        public sealed override int MaxHealth { get; set; }

        public OrcBase(SpriteBatch spriteBatch, Game1 game)
        {
            Texture = Texture = game.Content.Load<Texture2D>("OrcBaseAtlas");
            DeathTex = game.Content.Load<Texture2D>("OrcBaseDeath");
            DeathSfx = SfxEnums.OrcDeath;
            AttackSfx = SfxEnums.OrcAttack;
            DamageSfx = SfxEnums.OrcHit;
            GameObject = game;
            Speed = 0.10f;
            Rows = 2;
            Row = 0;
            Columns = 6;
            Height = 256;
            Width = 764;
            CurrentFrame = 0;
            CurrentDeathFrame = 0;
            TotalFrames = Rows / 2 * Columns - 1;
            SpriteBatchObject = spriteBatch;
            RangedEnemy = false;
            MaxHealth = MaxHealthValue;
            Health = MaxHealthValue;
            AttackDamage = AttackDamageValue;
            AttackTime = AttackTimeValue;
            IsAlive = true;
            IsDamaged = false;
            Alerted = false;
            SpriteColor = Color.White;

            alertDistance = alert_distance;

            // Define Action Pattern
            PacePattern = new ActionPattern();

            int[] mcas = new int[2] { 1, 0 };
            int[] mccs = new int[1] { pace_time };
            PacePattern.AddAction(MoveAction, TimeCondition, mcas, mccs);

            int[] mcas2 = new int[2] { -1, 0 };
            PacePattern.AddAction(MoveAction, TimeCondition, mcas2, mccs);
        }

        public override void Update(GameTime gameTime)
        {
            if (Dying)
                SetDeathTex();

            base.Update(gameTime);
        }

        protected override void IdleAction(GameTime time)
        {
            PacePattern.Update(time);

            base.IdleAction(time);
        }

        private void SetDeathTex()
        {
            // If first time in death state, set up death animation
            if (Texture != DeathTex)
            {
                CurrentFrame = 0;

                Texture = DeathTex;
                Row = 0;
                Rows = 1;
                Columns = 6;
                Height = 128;
                Width = 768;
                TotalFrames = 5;
            }
        }
    }
}