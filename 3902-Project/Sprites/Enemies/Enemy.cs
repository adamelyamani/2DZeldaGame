using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Sprites.Enemies.PathFinding;
using Project.Sprites.Players;
using Project.Sprites.Items;
using System;

namespace Project.Sprites.Enemies;

public abstract class Enemy : IEnemy
{
    // Constants
    protected const float Tolerance = 0.0001f;
    private Vector2 _position;
    private Vector2 _spawn;

    private int _timeSinceLastAttack = 9999;

    private const int alert_animation_time = 2050;
    private const int alert_multiplier = 5;
    private const int alert_vertical_offset = 15;

    private const int alert_cancel_time = 2000;
    private const int alert_distance = 300;

    private const int damage_flash_time = 100;

    private const int frame_time = 200;

    // PathFinding repathing of player
    private const int repath_time = 500;

    // Objects for management
    public Game1 GameObject { get; set; }

    // Texture info
    public SpriteBatch SpriteBatchObject { get; set; }
    public Texture2D Texture { get; set; }
    public Texture2D DeathTex { get; set; }
    public int TextureOffsetX { get; set; }
    public int TextureOffsetY { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int Rows { get; set; }
    public int Columns { get; set; }
    public Color SpriteColor { get; set; }

    // Frame Rendering
    public int Row { get; set; }
    public int CurrentFrame { get; set; }
    public int CurrentDeathFrame { get; set; }
    public int TotalFrames { get; set; }

    public SfxEnums DeathSfx { get; set; }
    public SfxEnums AttackSfx { get; set; }
    public SfxEnums DamageSfx { get; set; }

    public Vector2 Position
    {
        get => _position;
        set
        {
            //basically just check here?
            var previousPosition = _position;
            var previousBounding = BoundingRectangle;
            BoundingRectangle = new Rectangle((int)value.X + BoundingBoxXOffset, (int)value.Y + BoundingBoxYOffset,
                BoundingBoxWidth, BoundingBoxHeight);
            _position = value;

            if (GameObject.CurrentLevel.CheckEnemyToEnvironment(this))
            {
                BoundingRectangle = previousBounding;
                _position = previousPosition;
            }
        }
    }

    public Vector2 Spawn
    {
        get => _spawn;
        set
        {
            Position = value;
            _spawn = new Vector2(BoundingRectangle.Center.X, BoundingRectangle.Center.Y);
        }
    }

    public Rectangle BoundingRectangle { get; set; }

    public bool RangedEnemy { get; set; }
    public float Health { get; set; }
    public abstract int MaxHealth { get; set; }
    public float Speed { get; set; }
    public int AttackDamage { get; set; }
    public int AttackTime { get; set; }

    public abstract int BoundingBoxHeight { get; }
    public abstract int BoundingBoxWidth { get; }
    public abstract int BoundingBoxXOffset { get; }
    public abstract int BoundingBoxYOffset { get; }

    // Flags and timers
    public bool Dying { get; set; }
    public bool IsAlive { get; set; }

    public bool IsDamaged { get; set; }
    private int damageFlashTimer = 0;

    private int frameTimer = 0;

    private bool firstIdle = true;

    public bool Alerted { get; set; }
    protected int alertDistance = alert_distance;
    protected int alertCancelTime = alert_cancel_time;
    protected int timeSinceAlert = 0;
    private int alertAnimationTimer = 0;

    protected PathFinder pathFinder;

    // Target to move towards
    public Vector2 target { get; set; }

    public virtual void Draw()
    {
        if (IsAlive)
        {
            // Set Color according to damage info
            SpriteColor = IsDamaged ? Color.Red : Color.White;

            // Draw Player Info
            var width = Width / Columns;
            var height = Height / Rows;
            var column = CurrentFrame % Columns;

            var sourceRectangle = new Rectangle(width * column, height * Row, width, height);
            var destinationRectangle = new Rectangle((int)Position.X - TextureOffsetX, (int)Position.Y - TextureOffsetY,
                width, height);

            // Draw the sprite
            SpriteBatchObject.Begin();
            SpriteBatchObject.Draw(Texture, destinationRectangle, sourceRectangle, SpriteColor);
            SpriteBatchObject.End();

            // Draw Alerted Effect Above player if alerted
            if (Alerted && timeSinceAlert < alert_animation_time) {
                // Centered above the player
                var pos = new Vector2(BoundingRectangle.Center.X, BoundingRectangle.Top - alert_vertical_offset);

                AlertedEffect.Draw(SpriteBatchObject, pos, Math.Min(alert_multiplier*timeSinceAlert, alert_animation_time), alert_animation_time);
            }

            // Debug draw path
            //if (pathFinder != null)
            //    pathFinder.Draw(this.GameObject, this.SpriteBatchObject);
        }

        // Until Removal, draw health
        HealthBar.DrawHealthBar(SpriteBatchObject, BoundingRectangle.X + BoundingRectangle.Width / 2f , BoundingRectangle.Y, MaxHealth, Health);
    }

    public virtual void Update(GameTime gameTime)
    {
        int elapsedTime = gameTime.ElapsedGameTime.Milliseconds;

        frameTimer += elapsedTime;

        // Death sounds
        if (Health <= 0 && !Dying)
        {
            Dying = true;
            SoundManager.Instance.PlaySound(DeathSfx);
        }

        // Update Frame info
        if (IsAlive && frameTimer > frame_time)
        {
            frameTimer = 0;
            
            CurrentFrame++;
            if (CurrentFrame > TotalFrames)
            {
                CurrentFrame = 0;

                // Run When Death Animation is complete
                if (Dying)
                    DeathEvent();
            }
        }

        // Base Update if not dying
        if (!Dying)
        {
            _timeSinceLastAttack += elapsedTime;

            // Update Damage Animation timers
            if (IsDamaged)
            {
                damageFlashTimer += elapsedTime;

                if (damageFlashTimer > damage_flash_time)
                {
                    damageFlashTimer = 0;
                    IsDamaged = false;
                }
            }

            // Update Alerted Status
            bool prev_alert_status = Alerted;
            bool in_alert_distance = Vector2.Distance(GetPosition(), GetPlayerPosition()) < alertDistance;
            bool ready_for_repath = !Alerted || timeSinceAlert > repath_time;

            if (in_alert_distance && ready_for_repath)
                Alert(GetPlayerPosition());

            // Either Idle or Alerted
            if (Alerted)
            {
                timeSinceAlert += elapsedTime;
                alertAnimationTimer += elapsedTime;

                if (timeSinceAlert > alertCancelTime)
                {
                    Alerted = false;
                    pathFinder = null;
                    IdleNoticeAction();
                }
                else
                    AlertedAction(gameTime);
            }
            else
            {
                if (firstIdle)
                {
                    firstIdle = false;
                    IdleNoticeAction();
                }

                IdleAction(gameTime);
            }
        }
    }

    protected virtual void Alert(Vector2 pos)
    {
        Alerted = true;
        timeSinceAlert = 0;

        AlertNoticeAction(pos);

        if (alertAnimationTimer > alert_animation_time || !Alerted)
            alertAnimationTimer = 0;
    }

    // Default Alert Notice Action is to generate a path to the player from PathFinder
    protected virtual void AlertNoticeAction(Vector2 pos)
    {
        // PathFind to Player
        // If there is no path, then just run towards the player
        if (!PathTo(pos))
        {
            target = pos;
        }
    }

    // Default Alert Action is to Chase Player using PathFinder
    protected virtual void AlertedAction(GameTime time)
    {
        if (pathFinder != null)
            UpdatePathing();

        Move(time);
    }

    // Called before IdleAction is Called for the first time
    protected virtual void IdleNoticeAction()
    {
        // Do nothing
        return;
    }

    // Default is do nothing
    protected virtual void IdleAction(GameTime time)
    {
        return;
    }

    // Default sets IsAlive to false
    public virtual void DeathEvent()
    {
        IsAlive = false;

        // Drop an item
        var itemFactory = new ItemFactory(GameObject, SpriteBatchObject);
        var newItem = ItemDropTable.Generate();

        // Check if an item was actually dropped
        if (newItem != ItemTypeEnums.None)
        {
            // If it was, create the item
            var droppedItem = itemFactory.Create(newItem);
            droppedItem.Position = new Vector2(BoundingRectangle.X + BoundingBoxWidth / 2f, BoundingRectangle.Y + BoundingBoxHeight / 2f);

            // Add the item to the current level
            GameObject.CurrentLevel.Items.Add(droppedItem);
        }
    }

    // Moves the Enemy towards the target variable
    public virtual void Move(GameTime time)
    {
        int elapsed = time.ElapsedGameTime.Milliseconds;

        //Move enemy towards player
        var dir = (target - GetPosition());
        if (dir.LengthSquared() != 0) { dir.Normalize(); }

        Position += dir * Speed * elapsed;

        // Set the direction of the sprite (Legacy code)
        Row = (dir.X > 0) ? 0 : 1;
    }

    // Creates a new path to target
    protected bool PathTo(Vector2 target)
    {
        pathFinder = new PathFinder(GetPosition(), target, GameObject.CurrentLevel, BoundingRectangle);
        if (!pathFinder.PathFound())
            pathFinder = null;

        return pathFinder != null;
    }

    // Update Pathing target if Pathing
    protected void UpdatePathing()
    {
        if (pathFinder != null)
            this.target = pathFinder.GetTargetAlongPath(GetPosition());
    }

    public Vector2 GetPlayerPosition()
    {
        var point = GameObject.Player.BoundingRectangle.Center;
        return new Vector2(point.X, point.Y);
    }

    public Vector2 GetPosition()
    {
        var center = BoundingRectangle.Center;
        return new Vector2(center.X, center.Y);
    }

    // Melee Attack Controller
    // Base is Melee Attack of player
    public virtual void Attack(IPlayer player)
    {
        if (_timeSinceLastAttack > AttackTime)
        {
            _timeSinceLastAttack = 0;
            if (player.IsAlive)
            {
                player.TakeDamage(AttackDamage);
                SoundManager.Instance.PlaySound(AttackSfx);
            }
        }
    }

    public virtual void TakeDamage(float damage)
    {
        Health -= damage;
        IsDamaged = true;
        damageFlashTimer = 0; // reset timer


        if (!Dying)
        {
            SoundManager.Instance.PlaySound(DamageSfx);
        }
    }
}