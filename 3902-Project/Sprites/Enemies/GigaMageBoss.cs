using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Sprites.Enemies.BossAttacks;

namespace Project.Sprites.Enemies
{
    public class GigaMageBoss : Enemy, IBossEnemy
    {
        // Cull
        private const int _boundingBoxHeight = 64;
        private const int _boundingBoxWidth = 36;
        private const int _boundingBoxXOffset = 16;
        private const int _boundingBoxYOffset = 0;

        private const int _maxHealth = 5000;
        private const float _baseSpeed = 0.1f;
        private const int _attackDamage = 20;
        private const int _attackTime = 250;

        private float _timer; // Next anim frame timer

        private readonly ProjectileManager _projectileManager = ProjectileManager.Instance;

        // Boss Info
        // Constant info
        private const float idle_wait_time = 2000f;
        private const float phase0_summon_cooldown = 10000.0f;
        private const float phase0_health_line = 2500.0f;
        private const float phase1_heal_amount = 200.0f;


        // Runtime Vars
        private int phase; // current phase
        private int state; // states per phase
        private float state_timer; // Time since last state change

        IBossAttack bossAttack;

        // Note: Overrides all alert code management for fight

        // Mage boss based on Skeleton Mage
        public GigaMageBoss(SpriteBatch spriteBatch, Game1 game)
        {
            Texture = game.Content.Load<Texture2D>("SkeletonMageAtlas");
            DeathTex = game.Content.Load<Texture2D>("SkeletonMageDeath");
            DeathSfx = SfxEnums.SkeletonDeath;
            AttackSfx = SfxEnums.SkeletonAttack;
            DamageSfx = SfxEnums.SkeletonHit;
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
            Speed = _baseSpeed;
            MaxHealth = _maxHealth;
            Health = MaxHealth;
            AttackDamage = _attackDamage;
            AttackTime = _attackTime;
            IsAlive = true;
            IsDamaged = false;
            SpriteColor = Color.White;

            // boss info
            SwitchPhase(0); // Init to first phase
        }

        public override int BoundingBoxHeight => _boundingBoxHeight;

        public override int BoundingBoxWidth => _boundingBoxWidth;

        public override int BoundingBoxXOffset => _boundingBoxXOffset;

        public override int BoundingBoxYOffset => _boundingBoxYOffset;

        public sealed override int MaxHealth { get; set; }

        public override void Update(GameTime gameTime)
        {
            OldUpdate(gameTime);

            float elapsed_time = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (Dying) phase = 3; // if boss suddenly dies out of the blue

            switch (phase)
            {
                case 0:
                    UpdatePhase0(elapsed_time);
                    break;
                case 1:
                    UpdatePhase1(elapsed_time);
                    break;
                case 2:
                    Dying = true;
                    break;
            }

            base.Update(gameTime);
        }

        void UpdatePhase0(float elapsed_time)
        {
            state_timer += elapsed_time;

            switch (state)
            {
                case 0: // Sit still at entry
                    if (state_timer > idle_wait_time)
                        SetState(1);
                    break;
                case 1:
                    MoveTowardsPlayer(elapsed_time);
                    // Create new Attack if none
                    if (bossAttack == null)
                        bossAttack = new GMChargeAttack(this);
                    // Update attack if running
                    if (bossAttack.Update(elapsed_time, GetPosition(), GameObject.Player.Position))
                    {
                        bossAttack = null;
                        SetState(2);
                    }
                    break;
                case 2:
                    // Create new Attack if none
                    if (bossAttack == null) bossAttack = new GMSummonBabyAttack(this);

                    // Update attack if running
                    if (bossAttack.Update(elapsed_time, GetPosition(), GameObject.Player.Position)) {
                        bossAttack = null;
                        SetState(3);
                    }

                    break;
                case 3:
                    MoveTowardsPlayer(elapsed_time);

                    // Wait a while after summoning before switching
                    if (state_timer >= phase0_summon_cooldown) SetState(1);

                    break;
            }

            if (Health < phase0_health_line)
            {
                SwitchPhase(1);
            }
        }

        void UpdatePhase1(float elapsed_time)
        {
            state_timer += elapsed_time;

            switch (state)
            {
                case 0: // simple walk
                    MoveTowardsPlayer(elapsed_time);
                    if (state_timer > idle_wait_time)
                        SetState(1);
                    break;

                case 1:
                    // Create new Attack if none
                    if (bossAttack == null)
                        bossAttack = new GMChargeAttack(this);
                    // Update attack if running
                    if (bossAttack.Update(elapsed_time, GetPosition(), GameObject.Player.Position))
                    {
                        bossAttack = null;
                        SetState(2);
                    }

                    break;
                case 2: // summon attack and walk
                    // Create new Attack if none
                    if (bossAttack == null)
                        bossAttack = new GMSummonBabyAttack(this);
                    // Update attack if running
                    if (bossAttack.Update(elapsed_time, GetPosition(), GameObject.Player.Position))
                    {
                        bossAttack = null;
                        SetState(3);
                    }

                    MoveTowardsPlayer(elapsed_time);
                    break;
                case 3: // heal state
                    // init heal
                    if (bossAttack == null)
                        bossAttack = new GMHeal(this, phase1_heal_amount);
                    // Update attack if running
                    if (bossAttack.Update(elapsed_time, GetPosition(), GameObject.Player.Position))
                    {
                        bossAttack = null;
                    SetState(0);
                    }
                    break;
            }

            if (Health <= 0) SwitchPhase(2);
        }

        // Reset on Phase Switch
        void SwitchPhase(int phase)
        {
            this.phase = phase;
            SetState(0);
            bossAttack = null;
        }

        void MoveTowardsPlayer(float elapasedTime)
        {
            Vector2 playerDir = GameObject.Player.Position - GetPosition();

            // Normalize if not zero
            if (playerDir.LengthSquared() != 0) { playerDir.Normalize(); }

            this.Position = Position + playerDir * _baseSpeed * (elapasedTime);
        }

        void OldUpdate(GameTime gameTime)
        {
            _timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (!Dying)
            {
                if (_timer > 200)
                {
                    CurrentFrame++;

                    if (CurrentFrame > TotalFrames)
                    {
                        CurrentFrame = 0;
                    }

                    _timer = 0;
                }
            }
            else
            {
                SetDeathTex();
            }
        }

        public override void Draw()
        {
            if (bossAttack != null) bossAttack.Draw();

            base.Draw();
        }

        public override void DeathEvent()
        {
            GameObject.GameState = GameStateEnums.Victory;

            base.DeathEvent();
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

        void SetState(int state)
        {
            this.state = state;
            this.state_timer = 0;
        }

        // Overrides to prevent collisions of default code
        protected override void AlertNoticeAction(Vector2 pos) { }

        protected override void AlertedAction(GameTime time) { }
    }
}
