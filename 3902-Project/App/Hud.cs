using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Sprites;
using Project.Sprites.Items;

namespace Project.App
{
    public class Hud : ISprite
    {
        private const int RoomOffsetInPixels = 15;
        private const int PathWidth = 3;

        private Texture2D _whitePixel;
        private readonly IInventory _inventory;
        private readonly Vector2 _itemTextPos;
        private readonly Vector2 _mapTextPos;
        private readonly Vector2 _itemPos;
        private readonly Point _roomStartLocation;
        private readonly Rectangle _hudInner;
        private readonly Rectangle _hudOuter;
        private readonly Rectangle _itemBoxOuter;
        private readonly Rectangle _itemBoxInner;
        private readonly Rectangle _mapBoxOuter;
        private readonly Rectangle _mapBoxInner;
        private readonly Rectangle _mapRoomSize;

        private int _hudWidth;
        private int _hudHeight;
        private int _edgeOffset;
        private readonly int boxOffset;
        private readonly int boxDimension;
        private readonly int itemXOffset;
        private readonly int itemYOffset;
        private readonly int _bowAdjust;

        public Hud(SpriteBatch spriteBatch, Game1 game, IInventory inventory)
        {
            GameObject = game;
            SpriteBatch = spriteBatch;
            _inventory = inventory;

            // Sizing variables
            _hudWidth = 1264;
            _hudHeight = 122;
            _edgeOffset = 5;
            boxOffset = 3;
            boxDimension = 95;
            itemXOffset = 20;
            itemYOffset = 1;
            _bowAdjust = 3;

            var hudPos = new Vector2(0, 728);
            _itemTextPos = new Vector2(50, 762);
            _mapTextPos = new Vector2(700, 762);
            var itemBoxPos = new Vector2(300, 741);
            var mapBoxPos = new Vector2(900, 741);
            _itemPos = new Vector2(itemBoxPos.X + itemXOffset + boxOffset, itemBoxPos.Y + itemYOffset + boxOffset);
            _roomStartLocation = new Point(915, 810);

            _hudOuter = new Rectangle((int)hudPos.X, (int)hudPos.Y, _hudWidth, _hudHeight);
            _hudInner = new Rectangle((int)hudPos.X + _edgeOffset, (int)hudPos.Y + _edgeOffset, _hudWidth - 2 * _edgeOffset, _hudHeight- 2 * _edgeOffset);
            _itemBoxOuter = new Rectangle((int)itemBoxPos.X, (int)itemBoxPos.Y, boxDimension, boxDimension);
            _itemBoxInner = new Rectangle((int)itemBoxPos.X + boxOffset, (int)itemBoxPos.Y + boxOffset, boxDimension - 2 * boxOffset, boxDimension - 2 * boxOffset);
            _mapBoxOuter = new Rectangle((int)mapBoxPos.X, (int)mapBoxPos.Y, boxDimension, boxDimension);
            _mapBoxInner = new Rectangle((int)mapBoxPos.X + boxOffset, (int)mapBoxPos.Y + boxOffset, boxDimension - 2 * boxOffset, boxDimension - 2 * boxOffset);
            var width = SpriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth / 6;
            _mapRoomSize = new Rectangle(width / 20, width / 20, width / 20, width / 20);
        }

        public Game1 GameObject { get; set; }

        public SpriteBatch SpriteBatch { get; set; }

        private Texture2D WhitePixel
        {
            get
            {
                if (_whitePixel == null)
                {
                    _whitePixel = new Texture2D(GameObject.GraphicsDevice, 1, 1);
                    _whitePixel.SetData(new Color[] { Color.White });
                }

                return _whitePixel;
            }
        }

        public void Draw()
        {
            SpriteBatch.Begin();

            // Hud Outline
            SpriteBatch.Draw(WhitePixel,_hudOuter,Color.Maroon);
            SpriteBatch.Draw(WhitePixel,_hudInner, new Color(43, 48, 55));

            // Inventory part of the Hud
            SpriteBatch.Draw(WhitePixel, _itemBoxOuter, Color.Gold);
            SpriteBatch.Draw(WhitePixel, _itemBoxInner, Color.Black);
            SpriteBatch.DrawString(GameObject.Font, "Held Item:", _itemTextPos, Color.DarkSlateGray);

            if (_inventory.LeftHand != null)
            {
                if (_inventory.LeftHand is BowWeapon)
                {
                    Rectangle sourceRectangle = new Rectangle(_inventory.LeftHand.Texture.Bounds.X + _inventory.LeftHand.Texture.Width / _bowAdjust, _inventory.LeftHand.Texture.Bounds.Y, _inventory.LeftHand.Texture.Width / _bowAdjust, _inventory.LeftHand.Texture.Height);
                    SpriteBatch.Draw(_inventory.LeftHand.Texture, _itemPos, sourceRectangle, Color.White);
                } else
                    SpriteBatch.Draw(_inventory.LeftHand.Texture, _itemPos, Color.White);
            }

            // Map part of the Hud
            SpriteBatch.Draw(WhitePixel, _mapBoxOuter, Color.Gold);
            SpriteBatch.Draw(WhitePixel, _mapBoxInner, Color.Black);
            SpriteBatch.DrawString(GameObject.Font, "Map:", _mapTextPos, Color.DarkSlateGray);

            SpriteBatch.End();
            DrawMap();
        }

        public void DrawMap()
        {
            DrawPaths();
            DrawRooms();
        }

        public void Update(GameTime gameTime) { }

        private void DrawPaths()
        {
            SpriteBatch.Begin();

            var drawnTransitions = GameObject.DiscoveredTransitions;

            if (HasMap())
            {
                drawnTransitions = GameObject.AllTransitions;
            }

            foreach (var path in drawnTransitions)
            {
                var room1 = path.Item1;
                var room2 = path.Item2;

                //Get the locations of each room
                var room1Location = Vector2.Zero;
                var room2Location = Vector2.Zero;

                foreach (var pair in GameObject.FullMap)
                {
                    if (pair.Item1 == room1)
                    {
                        room1Location = pair.Item2;
                    }

                    if (pair.Item1 == room2)
                    {
                        room2Location = pair.Item2;
                    }
                }

                //Start at room 1, and draw rectangle towards room 2
                var direction = room2Location - room1Location;

                Point room1MapLocation = _roomStartLocation + new Point((int)(RoomOffsetInPixels * room1Location.X),
                    -(int)(RoomOffsetInPixels * room1Location.Y));

                Point room2MapLocation = _roomStartLocation + new Point((int)(RoomOffsetInPixels * room2Location.X),
                    -(int)(RoomOffsetInPixels * room2Location.Y));

                if (direction.X > 0)
                {
                    SpriteBatch.Draw(WhitePixel, new Rectangle(room1MapLocation.X, room1MapLocation.Y + _mapRoomSize.Y / 2 - PathWidth / 2, RoomOffsetInPixels + _mapRoomSize.X, PathWidth), Color.Maroon);
                }

                if (direction.X < 0)
                {
                    SpriteBatch.Draw(WhitePixel, new Rectangle(room2MapLocation.X, room2MapLocation.Y + _mapRoomSize.Y / 2 - PathWidth / 2, RoomOffsetInPixels + _mapRoomSize.X, PathWidth), Color.Maroon);
                }

                if (direction.Y > 0)
                {
                    SpriteBatch.Draw(WhitePixel, new Rectangle(room2MapLocation.X + _mapRoomSize.X / 2 - PathWidth / 2, room2MapLocation.Y, PathWidth, RoomOffsetInPixels + _mapRoomSize.Y), Color.Maroon);
                }

                if (direction.Y < 0)
                {
                    SpriteBatch.Draw(WhitePixel, new Rectangle(room1MapLocation.X + _mapRoomSize.X / 2 - PathWidth / 2, room1MapLocation.Y, PathWidth, RoomOffsetInPixels + _mapRoomSize.Y), Color.Maroon);
                }
            }

            SpriteBatch.End();
        }

        private void DrawRooms()
        {
            SpriteBatch.Begin();

            var drawnMaps = GameObject.DiscoveredRooms;

            if (HasMap())
            {
                drawnMaps = GameObject.FullMap.Select(level => level.Item1).ToList();
            }

            foreach (var room in drawnMaps)
            {
                //Calculate the X and Y positions of the room
                Vector2 location = Vector2.Zero;

                foreach (var pair in GameObject.FullMap)
                {
                    if (pair.Item1 == room)
                    {
                        location = pair.Item2;
                    }
                }

                Point newLocation = _roomStartLocation + new Point((int)(RoomOffsetInPixels * location.X),
                    -(int)(RoomOffsetInPixels * location.Y));

                //Draw the rooms
                SpriteBatch.Draw(WhitePixel, new Rectangle(newLocation.X, newLocation.Y, _mapRoomSize.Width, _mapRoomSize.Height), Color.Gold);

                //If this is the current room, draw a tiny square in it
                if (room == GameObject.CurrentLevel)
                {
                    SpriteBatch.Draw(WhitePixel, new Rectangle(newLocation.X + _mapRoomSize.X / 2 - _mapRoomSize.Width / 8, newLocation.Y + _mapRoomSize.Y / 2 - _mapRoomSize.Height / 8, _mapRoomSize.Width / 4, _mapRoomSize.Height / 4), Color.Red);
                }
            }

            SpriteBatch.End();
        }

        private bool HasMap()
        {
            foreach (var item in _inventory.InventoryList)
            {
                if (item is Map)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
