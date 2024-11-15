using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Project.App;

namespace Project.Controllers
{
    internal class PauseMouseController : IController
    {
        protected MouseState NewMouse;
        public MouseState OldMouse;

        private readonly Game1 _game;
        private readonly IInventory _inventory;

        private readonly Vector2 _boxVector;
        private readonly Vector2 _borderPosition;

        private int _gridSize;
        private int _borderHeight;
        private int _boxWidth;
        private int _boxHeight;
        private int _boxWidthOffset;
        private int _boxHeightOffset;
        private int _borderOffset;
        private int _lesserOffset;

        public PauseMouseController(Game1 game, IInventory inventory) 
        {
            _game = game;
            _inventory = inventory;

            _gridSize = 5;
            _boxWidth = 92;
            _boxHeight = 92;
            _boxWidthOffset = 6;
            _boxHeightOffset = 6;
            _borderOffset = 4;
            _lesserOffset = 2;

            var _borderPosition = new Vector2(50, 150);
            _borderHeight = 100;
            _boxVector = new Vector2(_borderPosition.X + _borderOffset * _lesserOffset, _borderPosition.Y + _borderHeight + _borderOffset * _lesserOffset);
        }

        public void Update()
        {
            NewMouse = Mouse.GetState();

            var xBound = _game.GraphicsDevice.PresentationParameters.BackBufferWidth;
            var yBound = _game.GraphicsDevice.PresentationParameters.BackBufferHeight;

            if (NewMouse.X < 0 || NewMouse.Y < 0 || NewMouse.X > xBound || NewMouse.Y > yBound)
            {
                return;
            }

            for (int i = 0; i < _game.InventoryItems.Count; i++)
            {
                // Determine the rectangle the item at index 'i' sits in
                Rectangle slot = new Rectangle((int)_boxVector.X + (_boxWidth + _boxWidthOffset) * (i % _gridSize),
                    (int)_boxVector.Y + (_boxHeight + _boxHeightOffset) * (i / _gridSize), _boxWidth, _boxHeight);

                if (slot.Contains(NewMouse.Position))
                {
                    // Click left MB to hold item
                    if (LeftClick()) 
                        _inventory.Equip(_game.InventoryItems[i]);
                    // Click right MB to drop item
                    if (RightClick())
                    {
                        _inventory.Drop(_game.InventoryItems[i]);
                    }
                }
            }

            OldMouse = NewMouse;
        }

        public bool LeftClick()
        {
            if (NewMouse.LeftButton == ButtonState.Pressed && OldMouse.LeftButton == ButtonState.Released)
                return true;

            return false;
        }

        public bool RightClick()
        {
            if (NewMouse.RightButton == ButtonState.Pressed && OldMouse.RightButton == ButtonState.Released)
                return true;

            return false;
        }
    }
}
