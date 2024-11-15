using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Project.App;
using Project.Sprites.Items;

namespace Project.Sprites.Players;

public class Player : IPlayer
{
    private const int QuiverRightShift = -10;
    private const int QuiverLeftShift = 10;
    private const int QuiverDownShift = 5;
    private const int BoundingHeight = 48;
    private const int BoundingWidth = 36;

    private const int RegenerationRate = 3;   // Health passively regained per second
    private const int ShieldDecay = 10; // Shield passively lost per second

    private readonly SpriteBatch _spriteBatch;

    private bool _enableAnimations;
    private Animation[] _animations;
    private int _currentAnimation;
    private float _animationTimer;
    private bool _movedLastFrame;
    private Color _spriteColor;
    private int _damageFlashCount;
    private Dictionary<ItemTypeEnums, IItem> _quivers;
    private readonly Game1 _game;
    private Vector2 _position;
    private Rectangle _boundingRectangle;

    public Player(SpriteBatch spriteBatch, Game1 game, PlayerStateEnums state, float health, float speed, bool isAlive)
    {
        _spriteBatch = spriteBatch;
        _game = game;
        State = state;

        // Create animation handler
        _enableAnimations = false;

        Position = new Vector2(400, 200); // init to origin

        MaxHealth = health;
        Health = health;
        Speed = speed;
        IsAlive = isAlive;

        FacingRight = true; // default

        LeftHand = new Hand(this, game, spriteBatch, HandEnums.LeftPlayer);
        RightHand = new Hand(this, game, spriteBatch, HandEnums.RightPlayer);

        _movedLastFrame = false;
        IsDamaged = false;

        BuildQuivers();
    }

    public Rectangle BoundingRectangle
    {
        get => _boundingRectangle;
        set
        {
            _position = new Vector2(value.X + (BoundingWidth / 2f), value.Y + (BoundingHeight / 2f));
            _boundingRectangle = value;
        } 
    }

    public Vector2 Position
    {
        get => _position;

        set
        {
            _boundingRectangle = new Rectangle((int)value.X - BoundingWidth / 2,
                (int)value.Y - BoundingHeight / 2,
                BoundingWidth,
                BoundingHeight);
            
            _position = value;

            if (_position == Vector2.Zero)
            {
                Debug.WriteLine("Test");
            }
        }
    }

    public float MaxHealth { get;  }
    public float Health { get; set; }

    public float Shield { get; set; } = 200;

    public bool IsDamaged { get; set; }

    public float Speed { get; set; }

    public bool IsAlive { get; set; }

    public PlayerStateEnums State { get; set; }

    public Hand LeftHand { get; set; }

    public Hand RightHand { get; set; }

    public int CurrentFrame { get; protected set; }
    public int LastFrame { get; protected set; }

    public bool FacingRight { get; set; }

    public void InitAnimations(Animation idle, Animation moving, Animation death)
    {
        _animations = new Animation[3];
        _animations[0] = idle;
        _animations[1] = moving;
        _animations[2] = death;

        SwitchAnimation(0);

        _enableAnimations = true;
        _spriteColor = Color.White;
        _damageFlashCount = 0;
    }

    public void Update(GameTime gameTime)
    {
        LeftHand.Update(gameTime);
        RightHand.Update(gameTime);

        if (_game.GameState == GameStateEnums.Running && IsAlive)
        {
            FacingRight = Mouse.GetState().Position.X > Position.X;
        }

        if (!_enableAnimations)
        {
            return;
        }

        if (LeftHand.HeldItem is BowWeapon bow)
        {
            _quivers[bow.ItemType].Position = Position;

            if (FacingRight)
            {
                _quivers[bow.ItemType].Position += new Vector2(QuiverRightShift, QuiverDownShift);
            }
            else
            {
                _quivers[bow.ItemType].Position += new Vector2(QuiverLeftShift, QuiverDownShift);
            }

            AnimateQuiver();
        }

        // inc timer
        var delta = (float) gameTime.ElapsedGameTime.TotalMilliseconds;
        _animationTimer += delta;

        // change frames and reset anim if needed
        // Leave on last frame if it's the death animation
        if (_animationTimer > CurrentAnimation().Period && IsAlive)
        {
            _animationTimer -= CurrentAnimation().Period;
        }

        LastFrame = CurrentFrame;
        CurrentFrame = (int) (MathF.Min(_animationTimer / CurrentAnimation().Period, 0.999f) *  CurrentAnimation().FrameCount);
        if ((State == PlayerStateEnums.Moving) && (LastFrame != CurrentFrame) &&
            CurrentFrame%2 == 0)
        {
            SoundManager.Instance.PlaySound(SfxEnums.PlayerStep);
        }

        // draw player sprite red if damaged or if health is less than 1
        if (IsDamaged && _damageFlashCount < 8 || this.Health < 1)
        {
            _spriteColor = Color.Red;
            _damageFlashCount++;
        } 
        else 
        {
            IsDamaged = false;
            _damageFlashCount = 0;
            _spriteColor = Color.White;
        }

        // Death registration
        if (Health <= 0)
        {
            if (State != PlayerStateEnums.Death)
            {
                SoundManager.Instance.PlaySound(SfxEnums.PlayerDeath);
            }
            State = PlayerStateEnums.Death;
            
        }

        // Apply regeneration
        Health += RegenerationRate * gameTime.ElapsedGameTime.Milliseconds / 1000f;

        // Cap Health
        if (Health > MaxHealth)
        {
            Health = MaxHealth;
        }

        // Cap Shield
        if (Shield > MaxHealth)
        {
            Shield = MaxHealth;
        }
        // Shield decays over time
        Shield -= ShieldDecay * gameTime.ElapsedGameTime.Milliseconds / 1000f;
        if (Shield < 0)
        {
            Shield = 0;
        }

        // Decide what to do on state
        switch (State)
        {
            case PlayerStateEnums.Idle:
                {
                    if (_movedLastFrame) { 
                        State = PlayerStateEnums.Moving;
                        this.SwitchAnimation(1);
                    }

                    break;
                }
            case PlayerStateEnums.Moving:
                {
                    if (_movedLastFrame) { 
                        _movedLastFrame = false;
                    }
                    else
                    {
                        State = PlayerStateEnums.Idle;
                        this.SwitchAnimation(0);
                    }

                    break;
                }
            case PlayerStateEnums.Death:
                {
                    if (IsAlive)
                    {
                        this.SwitchAnimation(2);
                        IsAlive = false;
                    }

                    if (_animationTimer > 1000)
                    {
                        _game.GameState = GameStateEnums.Dead;
                    }

                    break;
                }
        }
    }

    public Vector2 Move(Vector2 direction)
    {
        if (!IsAlive) // return if dead
        {
            return Position;
        }

        var safePosition = Position;
        Position += direction * Speed;
        _movedLastFrame = true;

        //Check if moving would collide with anything. If so, move back to previous position.
        if (_game.CurrentLevel.CheckPlayerToEnvironment())
        {
            Position = safePosition;
            return safePosition;
        }

        return Position;
    }

    public Vector2 Dash(Vector2 direction)
    {
        if (!IsAlive) // return if dead
        {
            return Position;
        }

        var safePosition = Position;
        Position += direction * Speed * 6;
        _movedLastFrame = true;

        //Check if moving would collide with anything. If so, move back to previous position.
        if (_game.CurrentLevel.CheckPlayerToEnvironment())
        {
            Position = safePosition;
            return safePosition;
        }

        return Position;
    }

    public void Draw()
    {
        if (!_enableAnimations)
        {
            return;
        }

        if (LeftHand.HeldItem is BowWeapon bow)
        {
            _quivers[bow.ItemType].Draw();
        }

        // Draw current frame, we don't actually change the frame until the update method since animation is time based
        var x = (int) CurrentAnimation().StartPos.X + CurrentFrame * (int) CurrentAnimation().Size.X;
        var y = (int) CurrentAnimation().StartPos.Y;
        var width = (int) CurrentAnimation().Size.X;
        var height = (int) CurrentAnimation().Size.Y;

        var offsetX = (int)CurrentAnimation().Offset.X;
        var offsetY = (int)CurrentAnimation().Offset.Y;

        var sourceRectangle = new Rectangle(x, y, width, height);
        var destinationRectangle = new Rectangle((int)Position.X - width/2 + offsetX, (int)Position.Y - height/2 + offsetY, width, height);

        LeftHand.Flipped = !FacingRight;
        RightHand.Flipped = !FacingRight;
        sourceRectangle = FacingRight ? sourceRectangle : Flip(sourceRectangle);

        LeftHand.Draw();

        if (!RightHand.IsOpen)
        {
            RightHand.Draw();
        }

        _spriteBatch.Begin();
        _spriteBatch.Draw(CurrentAnimation().Tex, destinationRectangle, sourceRectangle, _spriteColor);
        _spriteBatch.End();

        if (RightHand.IsOpen)
        {
            RightHand.Draw();
        }

        if (_game.GameState is GameStateEnums.Running)
        {
            HealthBar.DrawHealthBar(_spriteBatch, BoundingRectangle.X + BoundingRectangle.Width / 2f, BoundingRectangle.Y - 10, MaxHealth, Health, Shield);
        }
    }

    public void Equip(IItem item)
    {
        SoundManager.Instance.PlaySound(SfxEnums.Equip);
    }

    public void Attack()
    {
    }

    public void TakeDamage(float damage)
    {
        IsDamaged = true;
        if (damage < Shield)
        {
            Shield -= damage;
        }
        else
        {
            damage -= Shield;
            Shield = 0;
            Health = MathF.Max(Health - damage, 0);
        }
    }

    private void SwitchAnimation(int anim)
    {
        CurrentFrame = 0;
        LastFrame = CurrentAnimation().FrameCount;
        _animationTimer = 0;
        _currentAnimation = anim;
    }

    private static Rectangle Flip(Rectangle rect)
    {
        return new Rectangle(rect.X + rect.Width, rect.Y, -1 * rect.Width, rect.Height);
    }

    private void BuildQuivers()
    {
        _quivers = new Dictionary<ItemTypeEnums, IItem>
        {
            { ItemTypeEnums.BoneBow, new Item(_spriteBatch, _game, ItemTypeEnums.BoneQuiver) },
            { ItemTypeEnums.WoodenBow, new Item(_spriteBatch, _game, ItemTypeEnums.WoodenQuiver) }
        };
    }

    private void AnimateQuiver()
    {
        if(LeftHand.HeldItem is not BowWeapon bow)
        {
            return;
        }

        var percent = GetPercentThroughAnimationFrame(CurrentFrame);

        if (percent > .5)
        {
            percent = 1 - percent;
        }

        const int distanceToMoveIdle = 3;

        switch (State)
        {
            case PlayerStateEnums.Death:
                break;

            case PlayerStateEnums.Idle:
            case PlayerStateEnums.Moving:
                _quivers[bow.ItemType].Position += new Vector2(0, distanceToMoveIdle * percent);
                break;

            default:
                throw new InvalidEnumArgumentException();
        }
    }

    public float GetPercentThroughAnimationFrame(int frame)
    {
        var totalFrames = CurrentAnimation().FrameCount;
        return (float)frame / totalFrames;
    }

    // Shorthand to access current animation
    public Animation CurrentAnimation()
    {
        return _animations[_currentAnimation]; 
    }
}