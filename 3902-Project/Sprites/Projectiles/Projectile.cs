using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework;
using Project.App;

namespace Project.Sprites.Projectiles
{
    public class Projectile : IProjectile
    {
        private Vector2 _position;
        private readonly SfxEnums _hitSfx;

        public Projectile(ProjectileEnums type, SpriteBatch spriteBatchObject, Game game, ISprite owner, Vector2 startingPosition, float angle, int projDamage, float projSpeed)
        {
            SpriteBatchObject = spriteBatchObject;
            GameObject = game;
            Owner = owner;
            Angle = angle;
            Damage = projDamage;
            Speed = projSpeed;

            // Load texture based on the enum name as a string
            Texture = game.Content.Load<Texture2D>(type.ToString());

            // Select arrow SFX or Fireball SFX
            _hitSfx = type is ProjectileEnums.WoodenArrow or ProjectileEnums.BoneArrow ? SfxEnums.ArrowHit : SfxEnums.FireballHit;

            Position = startingPosition;
            RotationPivot = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
        }

        public Game GameObject { get; protected set; }

        public int Damage { get; set; }

        public float Speed { get; set; }

        //In radians
        public float Angle { get; set; }

        public bool Live { get; set; } = true;

        public ISprite Owner { get; set; }

        public Texture2D Texture { get; set; }

        protected Vector2 RotationPivot { get; set; }

        public SpriteBatch SpriteBatchObject { get; protected set; }

        public Vector2 Position
        {
            get => _position;
            set
            {
                BoundingRectangle = new Rectangle((int)value.X - Texture.Width / 2,
                    (int)value.Y - Texture.Height / 2,
                    Texture.Width,
                    Texture.Height);
                _position = value;
            }
        }

        public Rectangle BoundingRectangle { get; set; }

        public RotatedRectangle RotatedRectangle => new RotatedRectangle(BoundingRectangle, RotationPivot, Angle);

        public void Update(GameTime gameTime)
        {
            if (!Live)
            {
                return;
            }

            Position = new Vector2(Position.X + (float)Math.Cos(Angle) * gameTime.ElapsedGameTime.Milliseconds * Speed, Position.Y + (float)Math.Sin(Angle) * gameTime.ElapsedGameTime.Milliseconds * Speed);

            if (Position.X > GameObject.GraphicsDevice.PresentationParameters.BackBufferWidth || Position.X < 0 || Position.Y > GameObject.GraphicsDevice.PresentationParameters.BackBufferHeight || Position.Y < 0)
            {
                Live = false;
            }
        }

        public void Draw()
        {
            if (!Live)
            {
                return;
            }

            Rectangle sourceRectangle = new(0, 0, Texture.Width, Texture.Height);
            Rectangle destinationRectangle = new((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

            SpriteBatchObject.Begin();
            SpriteBatchObject.Draw(Texture, destinationRectangle, sourceRectangle, Color.White, Angle, RotationPivot, SpriteEffects.None, 0);
            SpriteBatchObject.End();
        }

        public void Hit()
        {
            SoundManager.Instance.PlaySound(_hitSfx);
        }
    }
}
