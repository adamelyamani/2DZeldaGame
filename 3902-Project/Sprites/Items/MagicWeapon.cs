using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Sprites.Projectiles;

namespace Project.Sprites.Items;

public class MagicWeapon : Item
{
    private readonly ProjectileManager _projectileManager = ProjectileManager.Instance;
    private readonly Game1 _game;

    public MagicWeapon(SpriteBatch spriteBatch, Game1 game, ItemTypeEnums weapon = ItemTypeEnums.WoodenScepter) :
        base(spriteBatch, game, weapon)
    {
        if (weapon is ItemTypeEnums.WoodenScepter or ItemTypeEnums.BoneScepter)
        {
            // Scepter
            SpriteRotationPivot = new Vector2(TextureSourceRectangle.Width / 2f, TextureSourceRectangle.Height / 2f);
        }
        else
        {
            // Wand
            SpriteRotationPivot = new Vector2(TextureSourceRectangle.Width / 4f, TextureSourceRectangle.Height / 2f);
        }

        _game = game;
    }

    public override void Use()
    {
        // Fire a Fireball if the cooldown is over
        if (ItemTimeSinceLastUsage > ItemStats.UsageTime)
        {
            var projAngle = SpriteFlip == SpriteEffects.None
                    ? SpriteAngle
                    : SpriteAngle + MathHelper.Pi;

            var projectile = ItemType switch
            {
                ItemTypeEnums.WoodenWand => ProjectileEnums.Fireball,
                ItemTypeEnums.WoodenScepter => ProjectileEnums.FireballLarge,
                ItemTypeEnums.BoneWand => ProjectileEnums.BlueFireball,
                ItemTypeEnums.BoneScepter => ProjectileEnums.BlueFireballLarge,
                _ => throw new ArgumentOutOfRangeException(nameof(ItemType)),
            };

            _projectileManager.AddProjectile(new Projectile(projectile, SpriteBatchObject, GameObject,
                _game.Player, Position, projAngle, ItemStats.ProjectileDamage, ItemStats.ProjectileSpeed));
        }
        
        // Important that bas.Use() is called after, since it changes the value of ItemTimeSinceLastUsage
        base.Use();
    }

    // Simple swinging animation suitable for basic weapons
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (ItemState == ItemStateEnums.Equipped)
        {

            if (ItemTimeSinceLastUsage > ItemStats.UsageTime)
            {
                SpriteAnimationRotation = 0;
            }
            else
            {
                SpriteAnimationRotation = 20-20f * ItemTimeSinceLastUsage / ItemStats.UsageTime;
                SpriteAnimationRotation = MathHelper.ToRadians(SpriteAnimationRotation);
            }
        }
    }
}