using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Interfaces;

namespace Project.Sprites.Items;

public interface IItem : ISprite, ICollidable
{
    public Vector2 Position { get; set; }

    public float SpriteAngle { get; set; } //rotation of the item in radians

    public float SpriteAnimationRotation { get; } //animation rotation of the item in radians

    public SpriteEffects SpriteFlip { get; set; }

    public Vector2 SpriteRotationPivot { get;  set; }

    public ItemStats ItemStats { get; }
    public ItemTypeEnums ItemType { get;}

    public ItemStateEnums ItemState { get; set; }

    public Texture2D Texture { get; set; }

    public bool InUse { get; }

    public void PickUp();        // Called when an item is picked up

    public void Use();           // Called whenever an item is used

    public void Equip();         // Called when an item is moved to the usable slot in inventory

    public void UnEquip();       // Called when an item is moved out of the usable slot in inventory

    public void Drop();          // Drop an existing item on the ground
}