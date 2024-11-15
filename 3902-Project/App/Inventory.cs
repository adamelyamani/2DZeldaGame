using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Sprites.Items;
using System.Collections.Generic;

namespace Project.App
{
    public class Inventory : IInventory
    {
        private Texture2D _whitePixel;
        private readonly Rectangle _leftHandSlot;
        private readonly Rectangle _inventoryInner;
        private readonly Vector2 _boxVector;
        private readonly Vector2 _heldSlot;
        private Vector2 _titlePos;
        private Color _bkgdColor = new Color(43, 48, 55);

        private readonly int _gridSize;
        private readonly int _boxWidth;
        private readonly int _boxHeight;
        private readonly int _boxWidthOffset;
        private readonly int _boxHeightOffset;
        private readonly int _extraOffset;
        private readonly int _invMax;
        private int _bowAdjust;

        public Inventory(SpriteBatch spriteBatch, Game1 game) 
        {
            SpriteBatch = spriteBatch;
            GameObject = game;

            _gridSize = 5;
            _boxWidth = 92;
            _boxHeight = 92;
            _boxWidthOffset = 6;
            _boxHeightOffset = 6;
            var borderOffset = 4;
            _extraOffset = 30;
            _invMax = 25;
            _bowAdjust = 3;

            var borderPosition = new Vector2(50, 150);
            var borderHeight = 100;
            var borderWidth = 500;
            var inventoryHeight = _gridSize * borderHeight;
            _leftHandSlot = new Rectangle((int)borderPosition.X + (int)(borderHeight * 3.5), (int)borderPosition.Y + 3, borderHeight - borderOffset, borderHeight - borderOffset);
            _inventoryInner = new Rectangle((int)borderPosition.X + 2, (int)borderPosition.Y + borderHeight + 2, borderWidth - borderOffset, inventoryHeight - borderOffset);
            _boxVector = new Vector2(borderPosition.X + borderOffset * 2, borderPosition.Y + borderHeight + borderOffset * 2);
            _titlePos = new Vector2(130, 175);
            _heldSlot = new Vector2(398,152);
        }

        public Game1 GameObject { get; set; }

        public SpriteBatch SpriteBatch { get; set; }

        public List<IItem> InventoryList { get; set; }

        public IItem LeftHand { get; set; }

        private Texture2D WhitePixel
        {
            get
            {
                if (_whitePixel == null)
                {
                    _whitePixel = new Texture2D(GameObject.GraphicsDevice, 1, 1);
                    _whitePixel.SetData(new[] { Color.White });
                }

                return _whitePixel;
            }
        }

        public void Draw()
        {
            SpriteBatch.Begin();

            SpriteBatch.Draw(WhitePixel, _leftHandSlot, Color.Black);
            SpriteBatch.Draw(WhitePixel, _inventoryInner, Color.Navy);

            SpriteBatch.DrawString(GameObject.Font, "Held Item:", _titlePos, Color.Red);

            for (int i = 0; i < _gridSize; i++)
            {
                for (int j = 0; j < _gridSize; j++)
                {
                    Rectangle box = new Rectangle((int)_boxVector.X + (_boxWidth + _boxWidthOffset)*j, (int)_boxVector.Y + (_boxHeight + _boxHeightOffset)*i, _boxWidth, _boxHeight);
                    SpriteBatch.Draw(WhitePixel, box, _bkgdColor);
                }
            }
            // Draw the item textures in the inventory
            for (int i = 0; i < InventoryList.Count; i++)
            {
                Vector2 tex = new Vector2(
                    (int)_boxVector.X + _extraOffset + (_boxWidth + _boxWidthOffset) * (i % _gridSize),
                    (int)_boxVector.Y + (_boxHeight + _boxHeightOffset) * (i / _gridSize));

                if (InventoryList[i] is BowWeapon)
                {
                    Rectangle sourceRectangle = new Rectangle(InventoryList[i].Texture.Bounds.X + InventoryList[i].Texture.Width / _bowAdjust, InventoryList[i].Texture.Bounds.Y, InventoryList[i].Texture.Width / _bowAdjust, InventoryList[i].Texture.Height);
                    SpriteBatch.Draw(InventoryList[i].Texture, tex, sourceRectangle, Color.White);
                }
                else
                {
                    SpriteBatch.Draw(InventoryList[i].Texture, tex, Color.White);
                }
                    
            }
            // Draw the held item, if it exists
            if (LeftHand != null)
            {

                if (LeftHand is BowWeapon)
                {
                    Rectangle sourceRectangle = new Rectangle(LeftHand.Texture.Bounds.X + LeftHand.Texture.Width / _bowAdjust, LeftHand.Texture.Bounds.Y, LeftHand.Texture.Width / _bowAdjust, LeftHand.Texture.Height);
                    SpriteBatch.Draw(LeftHand.Texture, new Vector2(_heldSlot.X + _extraOffset, _heldSlot.Y), sourceRectangle, Color.White);
                }
                else
                {
                    SpriteBatch.Draw(LeftHand.Texture, new Vector2(_heldSlot.X + _extraOffset, _heldSlot.Y), Color.White);
                }
            }

            SpriteBatch.End();
        }

        public void Update(GameTime gameTime) 
        {
            LeftHand?.Equip();
            GameObject.Player.LeftHand.HeldItem = LeftHand;
        }

        public void Equip(IItem item)
        {
            // Put 'item' texture in held slot
            LeftHand = item;
            SoundManager.Instance.PlaySound(SfxEnums.Equip);
        }

        public void Collect(IItem item) 
        {
            // Can't pick up more than 25 items
            if (InventoryList.Count < _invMax && !InventoryList.Contains(item))
            {
                InventoryList.Add(item);
            }
        }

        public void Drop(IItem item) 
        { 
            // Only drop item if not currently held
            if (!item.Equals(LeftHand))
            {
                InventoryList.Remove(item);
                item.Drop();
                item.Position = GameObject.Player.Position;
                GameObject.CurrentLevel.Items.Add(item);
            }
        }
    }
}
