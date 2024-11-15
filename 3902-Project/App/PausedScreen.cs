using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Sprites;
using Project.Sprites.Items;

namespace Project.App
{
    public class PausedScreen : ISprite
    {
        private const int RoomOffsetInPixels = 30;
        private const int PathWidth = 5;

        private readonly SpriteBatch _spriteBatch;
        private readonly Game1 _game;
        private Texture2D _whitePixel;
        private Rectangle _mapSize;
        private Rectangle _mapRoomSize;
        private readonly IInventory _inventory;
        private Point _roomStartLocation;

        public PausedScreen(SpriteBatch spriteBatch, Game1 game, IInventory _inventory)
        {
            _spriteBatch = spriteBatch;
            _game = game;
            this._inventory = _inventory;
            InitializePauseScreenSprites();
        }

        private void InitializePauseScreenSprites()
        { 
            //Initalize the Map here
            _mapSize = new Rectangle(_spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth / 2, 
                _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight / 3, 
                _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth / 3,
                _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight / 3);
            _mapRoomSize = new Rectangle(_mapSize.Width/20, _mapSize.Width/20, _mapSize.Width/20, _mapSize.Width/20);

            _roomStartLocation = new Point(_mapSize.X + _mapSize.Width / 2 - _mapRoomSize.X, _mapSize.Y + _mapSize.Height - _mapRoomSize.Y - RoomOffsetInPixels);
        }

        private Texture2D WhitePixel
        {
            get
            {
                if (_whitePixel == null)
                {
                    _whitePixel = new Texture2D(_game.GraphicsDevice, 1, 1);
                    _whitePixel.SetData(new[] { Color.White });
                }

                return _whitePixel;
            }
        }

        public void Update(GameTime gameTime)
        {
            //This is needed to get the hands to be draw on start screen. Not sure but it is a work around that works.
            _inventory.Update(gameTime);
        }

        public void Draw()
        {
            //Draw the inventory. Allow for swapping of item positions
            _inventory.Draw();

            //Draw background of map
            _spriteBatch.Begin();
            _spriteBatch.Draw(WhitePixel, _mapSize, Color.BurlyWood);
            _spriteBatch.DrawString(_game.Font, "Map", new Vector2(800, 200), Color.Red);
            _spriteBatch.End();

            DrawPaths();
            DrawRooms();
        }

        private void DrawPaths()
        {
            //Draw the paths
            _spriteBatch.Begin();

            var drawnTransitions = _game.DiscoveredTransitions;

            if (HasMap())
            {
                drawnTransitions = _game.AllTransitions;
            }

            foreach (var path in drawnTransitions)
            {
                var room1 = path.Item1;
                var room2 = path.Item2;

                //Get the locations of each room
                var room1Location = Vector2.Zero;
                var room2Location = Vector2.Zero;

                foreach (var pair in _game.FullMap)
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
                    _spriteBatch.Draw(WhitePixel, new Rectangle(room1MapLocation.X, room1MapLocation.Y + _mapRoomSize.Y / 2 - PathWidth / 2, RoomOffsetInPixels + _mapRoomSize.X, PathWidth), Color.Black);
                }

                if (direction.X < 0)
                {
                    _spriteBatch.Draw(WhitePixel, new Rectangle(room2MapLocation.X, room1MapLocation.Y + _mapRoomSize.Y / 2 - PathWidth / 2, RoomOffsetInPixels + _mapRoomSize.X, PathWidth), Color.Black);
                }

                if (direction.Y > 0)
                {
                    _spriteBatch.Draw(WhitePixel, new Rectangle(room2MapLocation.X + _mapRoomSize.X / 2 - PathWidth / 2, room2MapLocation.Y, PathWidth, RoomOffsetInPixels + _mapRoomSize.Y), Color.Black);
                }

                if (direction.Y < 0)
                {
                    _spriteBatch.Draw(WhitePixel, new Rectangle(room1MapLocation.X + _mapRoomSize.X / 2 - PathWidth / 2, room1MapLocation.Y, PathWidth, RoomOffsetInPixels + _mapRoomSize.Y), Color.Black);
                }
            }

            _spriteBatch.End();
        }

        private void DrawRooms()
        {
            _spriteBatch.Begin();

            var drawnMaps = _game.DiscoveredRooms;

            if (HasMap())
            {
                drawnMaps = _game.FullMap.Select(level => level.Item1).ToList();
            }

            foreach (var room in drawnMaps)
            {
                //Calculate the X and Y positions of the room
                Vector2 location = Vector2.Zero;

                foreach (var pair in _game.FullMap)
                {
                    if (pair.Item1 == room)
                    {
                        location = pair.Item2;
                    }
                }

                Point newLocation = _roomStartLocation + new Point((int)(RoomOffsetInPixels * location.X),
                    -(int)(RoomOffsetInPixels * location.Y));

                //Draw the rooms
                _spriteBatch.Draw(WhitePixel, new Rectangle(newLocation.X, newLocation.Y, _mapRoomSize.Width, _mapRoomSize.Height), Color.Black);

                //If this is the current room, draw a tiny square in it
                if (room == _game.CurrentLevel)
                {
                    _spriteBatch.Draw(WhitePixel, new Rectangle(newLocation.X + _mapRoomSize.X / 2 - _mapRoomSize.Width / 8, newLocation.Y + _mapRoomSize.Y / 2 - _mapRoomSize.Height / 8, _mapRoomSize.Width / 4, _mapRoomSize.Height / 4), Color.Red);
                }
            }

            _spriteBatch.End();
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
