using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.Controllers;
using Project.Sprites;
using Project.Sprites.Players;
using Project.Sprites.Items;
using System;
using System.ComponentModel;
using System.Linq;
using Project.Sprites.Environment;

namespace Project.App
{
    public class Game1 : Game
    {
#pragma warning disable IDE0052 // Remove unread private members
        private readonly GraphicsDeviceManager _graphics;
#pragma warning restore IDE0052 // Remove unread private members

        private SpriteBatch _spriteBatch;
        private readonly ProjectileManager _projectileManager = ProjectileManager.Instance;
        private Dictionary<GameStateEnums, IController> _keyboardControllers;
        private Dictionary<GameStateEnums, IController> _mouseControllers;
        private Level _currentLevel;
        private bool _doneInitializing;
        private GameStateEnums _gameState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        public SpriteFont Font { get; private set; }

        public Level CurrentLevel
        {
            get => _currentLevel;
            set
            {
                if (!DiscoveredRooms.Contains(value) && _currentLevel != null)
                {
                    DiscoveredRooms.Add(value);
                }

                var potentialPath = Tuple.Create(_currentLevel, value);

                if (!DiscoveredTransitions.Contains(potentialPath) && _doneInitializing)
                {
                    DiscoveredTransitions.Add(potentialPath);
                }


                _currentLevel = value;
            }
        }

        public List<Level> DiscoveredRooms { get; private set; }

        public List<Tuple<Level, Level>> DiscoveredTransitions { get; private set; }

        public List<Tuple<Level, Level>> AllTransitions { get; private set; }

        public int CurrentLevelIndex { get; set; }

        public Level OldLevel { get; set; }

        public GameStateEnums GameState
        {
            get => _gameState;
            set
            {
                switch (value)
                {
                    case GameStateEnums.Dead:
                        //SoundManager.Instance.PlaySound(SfxEnums.PlayerDeath);
                        SoundManager.Instance.PlaySound(SfxEnums.GameOver);
                        SoundManager.MusicPause();
                        break;
                    case GameStateEnums.Main:
                    case GameStateEnums.Paused:
                    case GameStateEnums.Running:
                    case GameStateEnums.Transition:
                    case GameStateEnums.Victory:
                        break;
                    default:
                        throw new InvalidEnumArgumentException();
                }

                _gameState = value;
            }
        }

        public List<Tuple<Level, Vector2>> FullMap { get; set; }

        public IPlayer Player { get; set; }

        public List<IPlayer> AvailablePlayers { get; set; }

        private ISprite MainScreen { get; set; }

        private ISprite PausedScreen { get; set; }

        private ISprite TransitionScreen { get; set; }

        private ISprite VictoryScreen { get; set; }

        private ISprite DeadScreen { get; set; }

        public Vector2 NewPlayerPosition { get; set; }

        public IInventory Inventory { get; set; }

        public List<IItem> InventoryItems {  get; set; }

        public ISprite Hud {  get; set; }

        public void ResetGame()
        {
            _projectileManager.ClearProjectiles();
            SoundManager.Instance.ClearSounds();
            CurrentLevel = null;
            _doneInitializing = false;
            FullMap.Clear();
            Initialize();
            LoadContent();
        }

        protected override void Initialize()
        {
            _doneInitializing = false;

            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 1264;
            _graphics.PreferredBackBufferHeight = 850;
            _graphics.ApplyChanges();

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Initialize item stats
            ItemStatSheet.Initialize(this);

            InventoryItems = new List<IItem>();
            Inventory = new Inventory(_spriteBatch, this)
            {
                InventoryList = InventoryItems
            };

            Hud = new Hud(_spriteBatch, this, Inventory);

            ItemStatSheet.Initialize(this);

            AvailablePlayers = new List<IPlayer>();
            LoadPlayers();

            // Initialize Level 
            DiscoveredRooms = new List<Level>();
            DiscoveredTransitions = new List<Tuple<Level, Level>>();
            AllTransitions = new List<Tuple<Level, Level>>();
            CurrentLevel = new Level(this, _spriteBatch, Inventory);
            FullMap = new List<Tuple<Level, Vector2>>();
            
            GenerateMap();

            InitializeControllers();

            GameState = GameStateEnums.Main;
            MainScreen = new MainScreen(_spriteBatch, this);
            PausedScreen = new PausedScreen(_spriteBatch, this, Inventory);
            TransitionScreen = new TransitionScreen(_spriteBatch, this);
            DeadScreen = new DeadScreen(_spriteBatch, this);
            VictoryScreen = new VictoryScreen(_spriteBatch, this);

            SoundManager.Instance.Initialize(this);
            SoundManager.Instance.MusicPlay();

            base.Initialize();
        }

        protected override void LoadContent()
        {

            Font = Content.Load<SpriteFont>("Header");

            _doneInitializing = true;
        }

        protected override void Update(GameTime gameTime)
        {
            _keyboardControllers[GameState].Update();
            _mouseControllers[GameState].Update();

            switch (GameState)
            {
                //todo add sound manager for different game states
                case GameStateEnums.Main:
                    MainScreen.Update(gameTime);
                    break;
                case GameStateEnums.Dead:
                    DeadScreen.Update(gameTime);
                    break;
                case GameStateEnums.Paused:
                    PausedScreen.Update(gameTime);
                    break;
                case GameStateEnums.Running:
                    Player.Update(gameTime);
                    CurrentLevel.Update(gameTime);
                    break;
                case GameStateEnums.Transition:
                    TransitionScreen.Update(gameTime);
                    break;
                case GameStateEnums.Victory:
                    VictoryScreen.Update(gameTime);
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }

            SoundManager.Instance.Update();
            
            base.Update(gameTime);
        }
         
        protected override void Draw(GameTime gameTime)
        {
            //GraphicsDevice.Clear(new Color(43, 48, 55));
            GraphicsDevice.Clear(Color.Black);

            switch (GameState)
            {
                case GameStateEnums.Main:
                    MainScreen.Draw();
                    break;
                case GameStateEnums.Dead:
                    DeadScreen.Draw();
                    break;
                case GameStateEnums.Paused:
                    PausedScreen.Draw();
                    break;
                case GameStateEnums.Running:
                    CurrentLevel.Draw();
                    Player.Draw();
                    break;
                case GameStateEnums.Transition:
                    TransitionScreen.Draw();
                    break;
                case GameStateEnums.Victory:
                    VictoryScreen.Draw();
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }

            base.Draw(gameTime);
        }

        private void InitializeControllers()
        {
            _keyboardControllers = new Dictionary<GameStateEnums, IController>
            {
                { GameStateEnums.Running, new RunningKeyboardController(this) },
                { GameStateEnums.Main, new MainKeyboardController(this) },
                { GameStateEnums.Paused, new PausedKeyboardController(this) },
                { GameStateEnums.Transition, new NullController() },
                { GameStateEnums.Dead,  new NullController()},
                { GameStateEnums.Victory, new NullController()}
            };

            _mouseControllers = new Dictionary<GameStateEnums, IController>
            {
                { GameStateEnums.Running, new MainMouseController(this) },
                { GameStateEnums.Main, new MainMouseController(this) },
                { GameStateEnums.Paused, new PauseMouseController(this, Inventory) },
                { GameStateEnums.Transition, new MainMouseController(this) },
                { GameStateEnums.Dead, new SelectionScreenMouseController(this)},
                { GameStateEnums.Victory, new SelectionScreenMouseController(this)}
            };
        }

        private void LoadPlayers()
        {
            AvailablePlayers.Add(new Knight(_spriteBatch, this));
            AvailablePlayers.Add(new Rogue(_spriteBatch, this));
            AvailablePlayers.Add(new Wizard(_spriteBatch, this));

            Player = AvailablePlayers[0];
        }

        private void GenerateMap()
        {
            // These are the levels categorized by their doors.
            var levelListFiles = new Tuple<Levels, string>[]
            {
                new Tuple<Levels, string>(Levels.Level1, "level1.json"),
                new Tuple<Levels, string>(Levels.Level3, "level3.json"),
                new Tuple<Levels, string>(Levels.Level4, "level4.json"),
                new Tuple<Levels, string>(Levels.Level5, "level5.json"),
                new Tuple<Levels, string>(Levels.Level6, "level6.json"),
                new Tuple<Levels, string>(Levels.Level7, "level7.json"),
                new Tuple<Levels, string>(Levels.Level8, "level8.json"),
                new Tuple<Levels, string>(Levels.Level9, "level9.json"),
                new Tuple<Levels, string>(Levels.Level10, "level10.json")
            };

            // Starting Coords
            var x = 2;
            var y = 2;
            var numberOfRooms = 0;
            var random = new Random();
            var depthTowardsBoss = 5;

            var directionNumber = random.Next(0, levelListFiles.Length);
            var currentLevelFile = levelListFiles[directionNumber].Item2;

            //Put first room in levels
            FullMap.Insert(numberOfRooms, new Tuple<Level, Vector2>(new Level(this, _spriteBatch, Inventory), new Vector2(x, y)));
            FullMap[numberOfRooms].Item1.LoadLevel("level0.json");

            //Add a door in a random direction.
            var door = GetRandomDoor();
            FullMap[numberOfRooms].Item1.SummonDoor(door);

            //Set up for next room
            UpdateCoordsBasedOnDirection(door.Direction);
            numberOfRooms++;

            for (int i = 0; i < depthTowardsBoss; i++)
            {
                //Get another random level.
                directionNumber = random.Next(0, levelListFiles.Length);
                currentLevelFile = levelListFiles[directionNumber].Item2;

                //Add new level to map.
                FullMap.Insert(numberOfRooms, new Tuple<Level, Vector2>(new Level(this, _spriteBatch, Inventory), new Vector2(x, y)));
                FullMap[numberOfRooms].Item1.LoadLevel(currentLevelFile);

                //Link old map to new map.
                AllTransitions.Add(new Tuple<Level, Level>(FullMap[numberOfRooms - 1].Item1, FullMap[numberOfRooms].Item1));

                //Add a door to the new level to branch back towards the old one.
                var oldDirection = door.Direction;
                switch (oldDirection)
                {
                    case DirectionEnums.North:
                        door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedS, DirectionEnums.South);
                        break;
                    case DirectionEnums.East:
                        door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedW, DirectionEnums.West);
                        break;
                    case DirectionEnums.South:
                        door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedN, DirectionEnums.North);
                        break;
                    case DirectionEnums.West:
                        door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedE, DirectionEnums.East);
                        break;
                }

                FullMap[numberOfRooms].Item1.SummonDoor(door);

                //Add a new door that is different than the old door
                //Also need to make sure that the door wouldn't lead to an existing room

                Door newDoor;
                var notSafe = true;
                do
                {
                    newDoor = GetRandomDoor();
                    notSafe = newDoor.Direction switch
                    {
                        DirectionEnums.North => DoesMapContainRoomAtOrOutOfBounds(new Vector2(x, y + 1)),
                        DirectionEnums.East => DoesMapContainRoomAtOrOutOfBounds(new Vector2(x + 1, y)),
                        DirectionEnums.South => DoesMapContainRoomAtOrOutOfBounds(new Vector2(x, y - 1)),
                        DirectionEnums.West => DoesMapContainRoomAtOrOutOfBounds(new Vector2(x - 1, y)),
                        _ => notSafe
                    };

                } while (door.Direction == newDoor.Direction || notSafe);

                // This is to make the loop work
                door = newDoor;

                FullMap[numberOfRooms].Item1.SummonDoor(door);

                //Signal start of next room
                UpdateCoordsBasedOnDirection(door.Direction);
                numberOfRooms++;
            }

            // Add boss room to end of this trail.
            FullMap.Insert(numberOfRooms, new Tuple<Level, Vector2>(new Level(this, _spriteBatch, Inventory), new Vector2(x, y)));
            FullMap[numberOfRooms].Item1.LoadLevel("bosslevel.json");
            AllTransitions.Add(new Tuple<Level, Level>(FullMap[numberOfRooms - 1].Item1, FullMap[numberOfRooms].Item1));

            //Add a door to the new level to branch back towards the old one.
            switch (door.Direction)
            {
                case DirectionEnums.North:
                    door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedS, DirectionEnums.South);
                    break;
                case DirectionEnums.East:
                    door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedW, DirectionEnums.West);
                    break;
                case DirectionEnums.South:
                    door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedN, DirectionEnums.North);
                    break;
                case DirectionEnums.West:
                    door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedE, DirectionEnums.East);
                    break;
            }

            FullMap[numberOfRooms].Item1.SummonDoor(door);
            numberOfRooms++;

            //Now that the path to the boss is guaranteed, we can go back along the path and side rooms
            //We will do this by choosing one of the rooms, and we will branch to a depth of 2 along the path, using the same algorithm as before
            
            for (int secondaryBranchIndex = 0; secondaryBranchIndex < depthTowardsBoss; secondaryBranchIndex++)
            {
                x = (int)FullMap[secondaryBranchIndex].Item2.X;
                y = (int)FullMap[secondaryBranchIndex].Item2.Y;
                var secondaryDepth = random.Next(1, 4);

                //Check if the room has any open safe spots
                if (DoesMapContainRoomAtOrOutOfBounds(new Vector2(x, y + 1)) &&
                    DoesMapContainRoomAtOrOutOfBounds(new Vector2(x + 1, y)) &&
                    DoesMapContainRoomAtOrOutOfBounds(new Vector2(x, y - 1)) &&
                    DoesMapContainRoomAtOrOutOfBounds(new Vector2(x - 1, y)))
                {
                    //No safe locations, continue to next room.
                    continue;
                }

                //Add the door to the room in a safe spot.
                Door newDoor;
                var notSafe = true;
                do
                {
                    newDoor = GetRandomDoor();
                    notSafe = newDoor.Direction switch
                    {
                        DirectionEnums.North => DoesMapContainRoomAtOrOutOfBounds(new Vector2(x, y + 1)),
                        DirectionEnums.East => DoesMapContainRoomAtOrOutOfBounds(new Vector2(x + 1, y)),
                        DirectionEnums.South => DoesMapContainRoomAtOrOutOfBounds(new Vector2(x, y - 1)),
                        DirectionEnums.West => DoesMapContainRoomAtOrOutOfBounds(new Vector2(x - 1, y)),
                        _ => true
                    };

                } while (notSafe);

                door = newDoor;
                FullMap[secondaryBranchIndex].Item1.SummonDoor(door);
                var oldPosition = new Vector2(x, y);
                UpdateCoordsBasedOnDirection(door.Direction);

                for (int i = 0; i < secondaryDepth; i++)
                {
                    //Get another random level.
                    directionNumber = random.Next(0, levelListFiles.Length);
                    currentLevelFile = levelListFiles[directionNumber].Item2;

                    //Add new level to map.
                    FullMap.Insert(numberOfRooms, new Tuple<Level, Vector2>(new Level(this, _spriteBatch, Inventory), new Vector2(x, y)));
                    FullMap[numberOfRooms].Item1.LoadLevel(currentLevelFile);

                    //Find the level associated with the oldX and Y and link it to the new room
                    var oldLevel = new Level(this, _spriteBatch, Inventory);
                    foreach (var level in FullMap)
                    {
                        if (level.Item2 == oldPosition)
                        {
                            oldLevel = level.Item1;
                        }
                    }
                    AllTransitions.Add(new Tuple<Level, Level>(oldLevel, FullMap[numberOfRooms].Item1));

                    //Add a door to the new level to branch back towards the old one.
                    var oldDirection = door.Direction;
                    switch (oldDirection)
                    {
                        case DirectionEnums.North:
                            door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedS, DirectionEnums.South);
                            break;
                        case DirectionEnums.East:
                            door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedW, DirectionEnums.West);
                            break;
                        case DirectionEnums.South:
                            door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedN, DirectionEnums.North);
                            break;
                        case DirectionEnums.West:
                            door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedE, DirectionEnums.East);
                            break;
                    }

                    FullMap[numberOfRooms].Item1.SummonDoor(door);

                    //If this is the last room in the path, do not add a door to no where.
                    if (i == secondaryDepth - 1)
                    {
                        numberOfRooms++;
                        continue;
                    }

                    //Add a new door that is different than the old door
                    //Also need to make sure that the door wouldn't lead to an existing room
                    notSafe = true;
                    var noSafeLocations = false;
                    do
                    {
                        //If we get to a spot where there are no rooms, we must break out of the loop
                        if (DoesMapContainRoomAtOrOutOfBounds(new Vector2(x, y + 1)) &&
                            DoesMapContainRoomAtOrOutOfBounds(new Vector2(x + 1, y)) &&
                            DoesMapContainRoomAtOrOutOfBounds(new Vector2(x, y - 1)) &&
                            DoesMapContainRoomAtOrOutOfBounds(new Vector2(x - 1, y)))
                        {
                            noSafeLocations = true;
                            break;
                        }

                        newDoor = GetRandomDoor();
                        notSafe = newDoor.Direction switch
                        {
                            DirectionEnums.North => DoesMapContainRoomAtOrOutOfBounds(new Vector2(x, y + 1)),
                            DirectionEnums.East => DoesMapContainRoomAtOrOutOfBounds(new Vector2(x + 1, y)),
                            DirectionEnums.South => DoesMapContainRoomAtOrOutOfBounds(new Vector2(x, y - 1)),
                            DirectionEnums.West => DoesMapContainRoomAtOrOutOfBounds(new Vector2(x - 1, y)),
                            _ => notSafe
                        };

                    } while (door.Direction == newDoor.Direction || notSafe);

                    if (noSafeLocations)
                    {
                        break;
                    }

                    // This is to make the loop work
                    door = newDoor;
                    FullMap[numberOfRooms].Item1.SummonDoor(door);

                    //Signal start of next room
                    oldPosition = new Vector2(x, y);
                    UpdateCoordsBasedOnDirection(door.Direction);
                    numberOfRooms++;
                }
            }

            for(var i = 0; i < numberOfRooms; i++)
            {
                //Randomly assign rooms to be dark
                if(random.Next(0, 4) == 0)
                {
                    FullMap[i].Item1.DarkRoom = true;
                }
            }

            CurrentLevel = FullMap[0].Item1;
            
            return;

            void UpdateCoordsBasedOnDirection(DirectionEnums direction)
            {
                switch (direction)
                {
                    case DirectionEnums.North:
                        y++;
                        break;
                    case DirectionEnums.South:
                        y--;
                        break;
                    case DirectionEnums.East:
                        x++;
                        break;
                    case DirectionEnums.West:
                        x--;
                        break;
                }
            }

            bool DoesMapContainRoomAtOrOutOfBounds(Vector2 location)
            {
                if (location.X is > 4 or < 0)
                {
                    return true;
                }

                if (location.Y is > 4 or < 0)
                {
                    return true;
                }

                return FullMap.Any(pair => pair.Item2 == location);
            }
        }

        private Door GetRandomDoor()
        {
            var random = new Random();

            var directionNumber = random.Next(1, 5);
            var door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedN, DirectionEnums.North);
            switch (directionNumber)
            {
                case 1:
                    door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedN, DirectionEnums.North);
                    break;
                case 2:
                    door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedE, DirectionEnums.East);
                    break;
                case 3:
                    door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedS, DirectionEnums.South);
                    break;
                case 4:
                    door = new Door(_spriteBatch, this, EnvironmentTypeEnums.DoorClosedW, DirectionEnums.West);
                    break;
            }

            return door;
        }

        private enum Levels
        {
            [Description("level0.json")] Level0,
            [Description("level1.json")] Level1, 
            [Description("level2.json")] Test, 
            [Description("level3.json")] Level3, 
            [Description("level4.json")] Level4,
            [Description("level5.json")] Level5, 
            [Description("level6.json")] Level6, 
            [Description("level7.json")] Level7, 
            [Description("level8.json")] Level8, 
            [Description("level9.json")] Level9,
            [Description("level10.json")] Level10, 
            [Description("bosslevel.json")] Boss
        }
    }
}