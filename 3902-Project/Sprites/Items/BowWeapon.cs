using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Sprites.Projectiles;
using System;

namespace Project.Sprites.Items;

public class BowWeapon : Item
{
    private readonly ProjectileManager _projectileManager = ProjectileManager.Instance;

    private const int TotalFrames = 3;
    private int _currentFrame;
    private readonly Game1 _game;

    public BowWeapon(SpriteBatch spriteBatch, Game1 game, ItemTypeEnums weapon = ItemTypeEnums.WoodenBow) :
        base(spriteBatch, game, weapon)
    {
        SpriteRotationPivot = new Vector2(7f*TextureSourceRectangle.Width/(TotalFrames * 8f), TextureSourceRectangle.Height / 2f);
        _game = game;
    }

    public override void Use()
    {
        // Fire an arrow if the cooldown is up
        if (ItemTimeSinceLastUsage > ItemStats.UsageTime)
        {
            var projAngle = SpriteFlip == SpriteEffects.None
                ? SpriteAngle
                : SpriteAngle + MathHelper.Pi;

            var projectile = ItemType switch
            {
                ItemTypeEnums.WoodenBow => ProjectileEnums.WoodenArrow,
                ItemTypeEnums.BoneBow => ProjectileEnums.BoneArrow,
                _ => throw new ArgumentOutOfRangeException(nameof(ItemType)),
            };

            _projectileManager.AddProjectile(new Projectile(projectile, SpriteBatchObject, GameObject, _game.Player, Position,
                projAngle, ItemStats.ProjectileDamage, ItemStats.ProjectileSpeed));

            SoundManager.Instance.PlaySound(SfxEnums.BowAttack);
        }

        // Important that base.Use() is called after, since it changes the value of ItemTimeSinceLastUsage
        base.Use();
    }

    public override void Draw()
    {
        // The item states to draw
        if (ItemState is ItemStateEnums.Equipped or ItemStateEnums.Ground or ItemStateEnums.Dropped or ItemStateEnums.Dropping)
        {

            var destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, TextureSourceRectangle.Width/3, TextureSourceRectangle.Height);
            var sourceRectangle = new Rectangle(TextureSourceRectangle.X + _currentFrame*TextureSourceRectangle.Width/3, TextureSourceRectangle.Y, TextureSourceRectangle.Width/3, TextureSourceRectangle.Height);
            BoundingRectangle = new Rectangle((int)(Position.X - SpriteRotationPivot.X), (int)(Position.Y - SpriteRotationPivot.Y), TextureSourceRectangle.Width/3, TextureSourceRectangle.Height);

            SpriteBatchObject.Begin();
            // Allow for frame scanning to animate the bow
            SpriteBatchObject.Draw(Texture, destinationRectangle, sourceRectangle, Color.White, SpriteAngle + SpriteAnimationRotation, SpriteRotationPivot, SpriteFlip, 0);
            SpriteBatchObject.End();
        }
    }

    // Allows for a pullback motion on bows
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (ItemState == ItemStateEnums.Equipped)
        {

            const int animationTime = 150; // Animation time in ms (so the animation is constant even if the bow shoots slower)
            if (ItemTimeSinceLastUsage > ItemStats.UsageTime || ItemTimeSinceLastUsage > animationTime)
            {
                _currentFrame = 0;
            }
            else
            {
                _currentFrame = (int)(TotalFrames * (1 - (float)ItemTimeSinceLastUsage / animationTime));
            }
        }
    }
}