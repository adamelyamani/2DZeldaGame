using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Sprites.Enemies.PathFinding;
using System;
using System.Diagnostics;
using System.Net.Mime;

namespace Project.Sprites.Enemies
{
    public class OrcRogue : ActionPatternEnemy
    {
        private const int BoundingBoxHeightValue = 64;
        private const int BoundingBoxWidthValue = 40;
        private const int BoundingBoxXOffsetValue = 44;
        private const int BoundingBoxYOffsetValue = 64;
        private const int MaxHealthValue = 90;
        private const int AttackDamageValue = 5;
        private const int AttackTimeValue = 125;

        private readonly Texture2D _idleTex;
        private readonly Texture2D _runTex;
        private readonly int _idleHeight;
        private readonly int _idleWidth;
        private readonly int _runHeight;
        private readonly int _runWidth;
        private readonly int _totalIdleFrames;
        private readonly int _totalRunFrames;
        private readonly int _idleCols;
        private readonly int _runCols;

        public override int BoundingBoxHeight => BoundingBoxHeightValue;
        public override int BoundingBoxWidth => BoundingBoxWidthValue;
        public override int BoundingBoxXOffset => BoundingBoxXOffsetValue;
        public override int BoundingBoxYOffset => BoundingBoxYOffsetValue;

        public sealed override int MaxHealth { get; set; }

        // Action Pattern Code
        ActionPattern runBackAndForth;

        public OrcRogue(SpriteBatch spriteBatch, Game1 game)
        {
            Texture = _runTex = game.Content.Load<Texture2D>("OrcRogueRun");
            _idleTex = game.Content.Load<Texture2D>("OrcRogueIdle");
            DeathTex = game.Content.Load<Texture2D>("OrcRogueDeath");
            DeathSfx = SfxEnums.OrcDeath;
            AttackSfx = SfxEnums.OrcAttack;
            DamageSfx = SfxEnums.OrcHit;
            GameObject = game;
            Speed = 0.125f;
            Rows = 2;
            Row = 0;
            Columns = _idleCols = 4;
            _runCols = 6;
            _idleHeight = 128;
            _runHeight = 256;
            Width = 764;
            _idleWidth = 256;
            _runWidth = 768;
            CurrentFrame = 0;
            CurrentDeathFrame = 0;
            _totalIdleFrames = 7;
            _totalRunFrames = 11;
            SpriteBatchObject = spriteBatch;
            Position = new Vector2(100, 200);
            RangedEnemy = false;
            MaxHealth = MaxHealthValue;
            Health = MaxHealthValue;
            AttackDamage = AttackDamageValue;
            AttackTime = AttackTimeValue;
            IsAlive = true;
            IsDamaged = false;
            SpriteColor = Color.White;

            // Define Action Pattern
            runBackAndForth = new ActionPattern();

            int[] mcas = new int[2] { 1, 0 };
            int[] mccs = new int[1] { 1500 };
            runBackAndForth.AddAction(MoveAction, TimeCondition, mcas, mccs);

            int[] mcas2 = new int[2] { -1, 0 };
            int[] mccs2 = new int[1] { 1500};
            runBackAndForth.AddAction(MoveAction, TimeCondition, mcas2, mccs2);
        }


        public override void Update(GameTime time)
        {
            int elapsedTime = time.ElapsedGameTime.Milliseconds;

            if (Dying)
                SetDeathTex();

            base.Update(time);
        }

        // Override Enemy Methods
        protected override void AlertedAction(GameTime time)
        {
            SetRunTex();

            base.AlertedAction(time);
        }

        protected override void IdleAction(GameTime time)
        {
            runBackAndForth.Update(time);

            base.IdleAction(time);
        }

        // Action Pattern Related Methods
        // protected void MoveTo

        // Action Pattern Overrides
        protected override void MoveAction(GameTime time, int time_since_action, int[] settings)
        {
            base.MoveAction(time, time_since_action, settings);

            // Change Texture
            SetRunTex();
        }

        protected override void IdleAction(GameTime time, int time_since_action, int[] settings)
        {
            base.IdleAction(time, time_since_action, settings);

            // Change Texture
            SetIdleTex();
        }

        private void SetIdleTex()
        {
            Texture = _idleTex;
            Columns = _idleCols;
            Height = _idleHeight;
            Width = _idleWidth;
            TotalFrames = _totalIdleFrames;
            TextureOffsetX = -32;
            TextureOffsetY = -64;
        }

        private void SetRunTex()
        {
            Texture = _runTex;
            Columns = _runCols;
            Height = _runHeight;
            Width = _runWidth;
            TotalFrames = _totalRunFrames;
            TextureOffsetX = 0;
            TextureOffsetY = 0;
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
                TextureOffsetX = 0;
                TextureOffsetY = 0;
            }
        }
    }
}