using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Sprites.Enemies.PathFinding;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Project.Sprites.Enemies
{
    public class OrcWarrior : ActionPatternEnemy
    {
        private const int BoundingBoxHeightValue = 64;
        private const int BoundingBoxWidthValue = 44;
        private const int BoundingBoxXOffsetValue = 44;
        private const int BoundingBoxYOffsetValue = 64;
        private const int MaxHealthValue = 400;
        private const int AttackDamage = 25;
        private const int AttackTime = 400;

        private const int moveTime = 2000;

        public override int BoundingBoxHeight => BoundingBoxHeightValue;
        public override int BoundingBoxWidth => BoundingBoxWidthValue;
        public override int BoundingBoxXOffset => BoundingBoxXOffsetValue;
        public override int BoundingBoxYOffset => BoundingBoxYOffsetValue;
        public sealed override int MaxHealth { get; set; }

        private ActionPattern MoveInSquare;

        public OrcWarrior(SpriteBatch spriteBatch, Game1 game)
        {
            Texture = Texture = game.Content.Load<Texture2D>("OrcWarriorAtlas");
            DeathTex = game.Content.Load<Texture2D>("OrcWarriorDeath");
            DeathSfx = SfxEnums.OrcDeath;
            AttackSfx = SfxEnums.OrcAttack;
            DamageSfx = SfxEnums.OrcHit;
            GameObject = game;
            Speed = 0.075f;
            Rows = 1;
            Row = 0;
            Columns = 6;
            Height = 128;
            Width = 764;
            CurrentFrame = 0;
            CurrentDeathFrame = 0;
            TotalFrames = Rows * Columns - 1;
            Position = new Vector2(250, 250);
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


        public override void Update(GameTime time)
        { 
            if (Dying)
                SetDeathTex();

            base.Update(time);
        }

        // Ensures the Square Movement Action Pattern is initialized and reset
        protected override void IdleNoticeAction()
        {
            if (MoveInSquare == null)
                InitSquareActionPattern();

            MoveInSquare.Reset();

            base.IdleNoticeAction();
        }

        protected override void IdleAction(GameTime time)
        {
            MoveInSquare.Update(time);

            base.IdleAction(time);
        }

        // Action Pattern Methods
        private void InitSquareActionPattern()
        {
            // Define Action Pattern
            MoveInSquare = new ActionPattern();

            // start by making sure it's at the starting position
            int[] mas = new int[2] { (int)Spawn.X, (int)Spawn.Y };
            MoveInSquare.AddAction(SetPathToPosAction, SingleUseCondition, mas, null);
            MoveInSquare.AddAction(FollowPathAction, FinishedPathCondition, null, null);

            // Square Movement Condition and actions
            int[] time = new int[1] { moveTime };

            MoveInSquare.AddAction(MoveAction, TimeCondition, new int[2] { 1, 0 }, time);
            MoveInSquare.AddAction(MoveAction, TimeCondition, new int[2] { 0, 1 }, time);
            MoveInSquare.AddAction(MoveAction, TimeCondition, new int[2] { -1, 0 }, time);
            MoveInSquare.AddAction(MoveAction, TimeCondition, new int[2] { 0, -1 }, time);
        }

        // Texture Helpers
        private void SetDeathTex()
        {
            Texture = DeathTex;
            Row = 0;
            Columns = 6;
            Height = 160;
            Width = 1152;
            TotalFrames = 5;
            TextureOffsetX = 32;
            TextureOffsetY = 32;
        }
    }
}