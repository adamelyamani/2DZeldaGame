using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;

namespace Project.Sprites.Items;

public class Item : IItem
{
    private readonly Vector2 _defaultPosition = new(225, 125);

    public Item(SpriteBatch spriteBatch, Game1 game, ItemTypeEnums item = ItemTypeEnums.WoodenGreatsword)
    {
        Position = _defaultPosition;
        SpriteBatchObject = spriteBatch;
        GameObject = game;

        // Item is on the ground by default
        ItemState = ItemStateEnums.Ground;
        IsCollidable = true;
        SpriteAnimationRotation = 0;

        // Take item type
        ItemType = item;
        ItemStats = ItemStatSheet.GetStats(ItemType);
        
        // Prevent Swinging upon pickup
        ItemTimeSinceLastUsage = ItemStats.UsageTime + 1;

        // Load texture
        Texture = game.Content.Load<Texture2D>(ItemType.ToString());
        TextureSourceRectangle = Texture.Bounds;
        SpriteRotationPivot = new Vector2(TextureSourceRectangle.Width / 2f, TextureSourceRectangle.Height / 2f);
    }

    //todo: review which of these need to be public and which can be protected
    public Game1 GameObject { get; set; }

    public bool IsCollidable { get; set; } //todo: ensure this is always false when the item is equipped or in inventory

    public Texture2D Texture { get; set; }

    public Rectangle TextureSourceRectangle { get; set; }

    public Vector2 Position { get; set; }

    public SpriteBatch SpriteBatchObject { get; protected set; }

    public ItemStateEnums ItemState { get; set; }

    public ItemTypeEnums ItemType { get; protected set; } // Type of item

    public ItemStats ItemStats { get; protected set; } // Stats of item

    public int ItemTimeSinceLastUsage { get; protected set; } // Time in ms since last item usage

    public Vector2 SpriteRotationPivot { get; set; }

    public bool InUse => ItemTimeSinceLastUsage <= ItemStats.UsageTime;

    public float SpriteAnimationRotation { get; protected set; } // Animated rotation of the item

    public float SpriteAngle { get; set; } // Item angle in radians

    public SpriteEffects SpriteFlip { get; set; } // Whether or not the item is vertically mirrored (player is facing left) //Possible values are: SpriteEffects.None, SpriteEffects.FlipHorizontally

    public Rectangle BoundingRectangle { get; set; }

    public virtual void PickUp()
    {
        //Called if the player collides with an item on the ground
        ItemState = ItemStateEnums.Inventory;
        IsCollidable = false;
    }

    public virtual void Equip()
    {
        ItemState = ItemStateEnums.Equipped;
    }

    public virtual void UnEquip()
    {
        ItemState = ItemStateEnums.Inventory;
    }

    public virtual void Use()
    {
        //Whenever the item is "used" in the player's inventory 
        Equip();  // Ensure that it is equipped

        // Use the item
        if (ItemTimeSinceLastUsage > ItemStats.UsageTime)
        {
            ItemTimeSinceLastUsage = 0;
        }
    }

    public void Drop()
    {
        IsCollidable = true;
        ItemState = ItemStateEnums.Dropping;
        SpriteAngle = 0;
        SpriteAnimationRotation = 0;
    }

    public virtual void Draw()
    {
        // The item states to draw
        if (ItemState is not (ItemStateEnums.Equipped or ItemStateEnums.Ground or ItemStateEnums.Dropped or ItemStateEnums.Dropping))
        {
            return;
        }

        var destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, TextureSourceRectangle.Width, TextureSourceRectangle.Height);
        BoundingRectangle = new Rectangle((int)(Position.X - SpriteRotationPivot.X), (int)(Position.Y - SpriteRotationPivot.Y), TextureSourceRectangle.Width, TextureSourceRectangle.Height);

        SpriteBatchObject.Begin();

        // Rotation here allows for swinging animation. Flip allows for the player to be facing left or right
        SpriteBatchObject.Draw(Texture, destinationRectangle, TextureSourceRectangle, Color.White, SpriteAngle + SpriteAnimationRotation, SpriteRotationPivot, SpriteFlip, 0);

        SpriteBatchObject.End();
    }

    // Default spinning animation
    public virtual void Update(GameTime gameTime)
    {   
        // Update item cooldown only if it is equipped
        if (ItemState == ItemStateEnums.Equipped)
        {
            ItemTimeSinceLastUsage += gameTime.ElapsedGameTime.Milliseconds;
        }
    }
}