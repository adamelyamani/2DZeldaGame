using Project.Sprites.Projectiles;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Project.Sprites.Enemies;
using Project.Sprites.Environment;
using Project.Sprites.Items;
using System.Text.Json;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Commands;
using System;
using System.Linq;
using Microsoft.Xna.Framework.Input;


namespace Project.Sprites
{
    public class Level : ISprite
    {
        private readonly List<IEnvironment> _environmentNonCollidable = new();
        public readonly List<IEnvironment> EnvironmentCollidable = new();
        public readonly List<IEnvironment> Traps = new();
        private readonly List<IEnemy> _enemies = new(); // List of Enemies in the level
        private readonly List<IEnemy> _hitEnemies = new(); // List of Enemies hit by a single sweep of a player (to avoid more than 1 hit per swing)

        private readonly Game1 _game;
        private readonly SpriteBatch _spriteBatch;
        private readonly RenderTarget2D _coverScreen;
        private bool _keyDrop = false;
        private int _inventoryMaxCount = 25;
        private readonly IInventory _inventory;
        private int _startingEnemyCount = 0;
        public bool DarkRoom = false;
        private readonly ProjectileManager _projectileManager = ProjectileManager.Instance;

        private readonly List<IEnemy> _enemiesToAdd = new();

        public readonly List<IItem> Items = new();
        private static List<IProjectile> Projectiles => ProjectileManager.Instance.GetProjectiles();

        public Level(Game1 game, SpriteBatch spriteBatch, IInventory inventory)
        { 
            _game = game;
            _spriteBatch = spriteBatch;
            _inventory = inventory;
            _coverScreen = new RenderTarget2D(_spriteBatch.GraphicsDevice, _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferWidth, _spriteBatch.GraphicsDevice.PresentationParameters.BackBufferHeight);
        }

        // Note: this may be moved into the constructor in the future, depending on how we decide to use levels in Game1.cs
        public void LoadLevel(string levelName)
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                WriteIndented = true
            };

            //Wipe out all the previous data
            _environmentNonCollidable.Clear();
            EnvironmentCollidable.Clear();
            _enemies.Clear();
            Items.Clear();

            using (FileStream jsonStream = new(Path.Combine(_game.Content.RootDirectory, levelName), FileMode.Open))
            {
                var levelData = JsonSerializer.Deserialize<LevelData>(jsonStream, options);
                ParseLevelData(levelData);
            }
        }

        public void ParseLevelData(LevelData levelData)
        {
            EnvironmentFactory environmentFactory = new(_game, _spriteBatch);
            EnemyFactory enemyFactory = new(_game, _spriteBatch);
            ItemFactory itemFactory = new(_game, _spriteBatch);

            // Parse environment into level
            foreach (var env in levelData.Environment)
            {
                if (env.IsCollidable)
                {
                    EnvironmentCollidable.Add(environmentFactory.Create(env));
                }
                else
                {
                    var createdEnv = environmentFactory.Create(env);
                    if(createdEnv is Trap)
                    {
                        Traps.Add(createdEnv);
                    }
                    else
                    {
                        _environmentNonCollidable.Add(createdEnv);
                    }
                    
                }
            }

            // Parse enemies into level
            foreach (var env in levelData.Enemies)
            {
                _enemies.Add(enemyFactory.Create(env));
                _startingEnemyCount++;
            }

            //Parse items into level
            foreach (var env in levelData.Items)
            {
                Items.Add(itemFactory.Create(env));
            }

            _game.Player.Position = levelData.PlayerStartingPosition;
        }

        public void Draw()
        {
            if (DarkRoom)
            {
                var lightCircle = _game.Content.Load<Texture2D>("LightCircle");
                var lightPath = _game.Content.Load<Texture2D>("LightPath");
                _spriteBatch.GraphicsDevice.SetRenderTarget(_coverScreen);
                //This last value adjusts the intensity of the darkness
                _spriteBatch.GraphicsDevice.Clear(new Color(0, 0, 0, 240));

                var blend = new BlendState
                {
                    AlphaBlendFunction = BlendFunction.ReverseSubtract,
                    AlphaSourceBlend = Blend.One,
                    AlphaDestinationBlend = Blend.One,
                };

                _spriteBatch.Begin(blendState: blend);
                var circleRect = new Rectangle((int)_game.Player.Position.X - 3 * _game.Player.BoundingRectangle.Width, (int)_game.Player.Position.Y - 2 * _game.Player.BoundingRectangle.Height, 200, 200);
                _spriteBatch.Draw(lightCircle, circleRect, Color.White);
                var pathRect = new Rectangle((int)_game.Player.Position.X, (int)_game.Player.Position.Y, lightPath.Width, lightPath.Height);
                var mouseState = Mouse.GetState();
                var mousePosition = new Vector2(mouseState.X, mouseState.Y);    
                var dPos = -_game.Player.Position + mousePosition;
                var angle = (float)Math.Atan2(dPos.Y, dPos.X);
                var rotatePoint = new Vector2(0, lightPath.Height / 2f);

                _spriteBatch.Draw(lightPath, pathRect, null, Color.White, angle, rotatePoint, SpriteEffects.None, 0);
                _spriteBatch.End();

                _spriteBatch.GraphicsDevice.SetRenderTarget(null);
                _spriteBatch.GraphicsDevice.Clear(Color.Black);
            }

            // Draw environment non-collidable first (background)
            foreach (var env in _environmentNonCollidable)
            {
                env.Draw();
            }

            //Draw collidable environment
            foreach (var env in EnvironmentCollidable)
            {
                env.Draw();
            }

            //Draw traps
            foreach (var env in Traps)
            {
                env.Draw();
            }

            // Draw enemies
            foreach (var enemy in _enemies)
            {
                enemy.Draw();
            }

            //Draw items
            foreach (var item in Items)
            {
                item.Draw();
            }

            //Draw projectiles
            _projectileManager.Draw();

            //Draw "darkness"
            if (DarkRoom)
            {
                _spriteBatch.Begin();
                _spriteBatch.Draw(_coverScreen, Vector2.Zero, Color.White);
                _spriteBatch.End();
            }

            _game.Hud.Draw();
        }

        public void Update(GameTime gameTime)
        {
            // Update environment
            foreach (var env in _environmentNonCollidable)
            {
                env.Update(gameTime);
            }

            foreach (var env in EnvironmentCollidable)
            {
                env.Update(gameTime);
            }

            foreach (var env in Traps)
            {
                env.Update(gameTime);
            }

            // Add any enemies waiting
            if (_enemiesToAdd.Count > 0)
            {
                foreach (var enemy in _enemiesToAdd) {
                    _enemies.Add(enemy);
                }

                _enemiesToAdd.Clear();
            }

            // Update enemies
            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime);
            }
            //Update items
            foreach (var item in Items)
            { 
                
                item.Update(gameTime);
            }

            _projectileManager.Update(gameTime);
            // Remove items that have been picked up by the player
            Items.RemoveAll(item => (item.ItemState != ItemStateEnums.Ground && item.ItemState != ItemStateEnums.Dropped && item.ItemState != ItemStateEnums.Dropping));

            _enemies.RemoveAll(enemy => !enemy.IsAlive);
            if (!_keyDrop && _startingEnemyCount != 0)
            {
                CheckKeyDrop();
            }
            
            CheckCollisions();
        }

        private void CheckCollisions()
        {
            CheckEnemyToPlayer();
            CheckPlayerItemToEnemy();
            CheckProjectilesToEnvironment();
            CheckProjectilesToEntities();
            CheckPlayerToItems();
            CheckPlayerToTraps();
        }

        private void CheckKeyDrop()
        {
            if(_enemies.Count == 0)
            {
                IItem k = new Key(_spriteBatch, _game)
                {
                    Position = new Vector2(_game.Player.Position.X + 30, _game.Player.Position.Y + 30)
                };

                k.Draw();
                //If a collidable is found that intersects with the current key position, set the key position to the next option and break
                //Repeat until an acceptable spot is found (should always be one in one of the four options- otherwise the player is stuck somewhere)
                foreach (var env in EnvironmentCollidable)
                {
                    if (k.BoundingRectangle.Intersects(env.BoundingRectangle))
                    {
                        k.Position = new Vector2(_game.Player.Position.X + 30, _game.Player.Position.Y - 30);
                        break;
                    }
                }
                foreach (var env in EnvironmentCollidable)
                {
                    if (k.BoundingRectangle.Intersects(env.BoundingRectangle))
                    {
                        k.Position = new Vector2(_game.Player.Position.X - 60, _game.Player.Position.Y - 60);
                        break;
                    }
                }
                foreach (var env in EnvironmentCollidable)
                {
                    if (k.BoundingRectangle.Intersects(env.BoundingRectangle))
                    {
                        k.Position = new Vector2(_game.Player.Position.X - 60, _game.Player.Position.Y + 30);
                        break;
                    }
                }
                Items.Add(k);
                _keyDrop = true;
            }
        }

        public void UnlockNearbyDoors()
        {
            //Unlock "connecting" door
            foreach (var temp in EnvironmentCollidable)
            {
                if (temp is Door d)
                {
                    if (d.Position.X <= _game.Player.Position.X + 100 && d.Position.X >= _game.Player.Position.X - 100 && d.Position.Y <= _game.Player.Position.Y + 100 && d.Position.Y >= _game.Player.Position.Y - 100)
                    {
                        d.Unlock();
                    }
                }
            }
        }

        public void CheckPlayerToTraps()
        {
            foreach (var env in Traps)
            {
                if (env.BoundingRectangle.Intersects(_game.Player.BoundingRectangle))
                {
                    if (env is Trap t)
                    {
                        if (t.Active)
                        {
                            _game.Player.TakeDamage(t.Damage);
                        }
                    }
                }
            }
        }

        public bool CheckRectToEnviornment(Rectangle rect)
        {
            // Check against Level walls
            if (!this._game.GraphicsDevice.Viewport.Bounds.Contains(rect))
                return true;

            foreach (var env in EnvironmentCollidable)
            {
                if (env.BoundingRectangle.Intersects(rect))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckPlayerToEnvironment()
        {
            var collides = false;
            foreach (var env in EnvironmentCollidable)
            {
                if (env.BoundingRectangle.Intersects(_game.Player.BoundingRectangle))
                {
                    if (env is Door door)
                    {
                        var transition = new TransitionRooms(_game)
                        {
                            Direction = door.Direction
                        };

                        if(!door.Locked)
                        {
                            transition.Execute();
                            _game.NewPlayerPosition = door.Direction switch
                            {
                                DirectionEnums.North => new Vector2(_game.Player.Position.X, 720 - env.BoundingRectangle.Height - 30),
                                DirectionEnums.East => new Vector2(env.BoundingRectangle.Width + 30, _game.Player.Position.Y),
                                DirectionEnums.South => new Vector2(_game.Player.Position.X, env.BoundingRectangle.Height + 110),
                                DirectionEnums.West => new Vector2(1264 - env.BoundingRectangle.Width - 30, _game.Player.Position.Y),
                                _ => throw new NotImplementedException("Unsupported Direction: " + door.Direction + "\n"),
                            };

                            UnlockNearbyDoors();

                            return false;
                        }
                        if (door.Locked)
                        {
                            for(int i = 0; i < _inventory.InventoryList.Count; i++)
                            {
                                if(_inventory.InventoryList.ElementAt(i) is Key)
                                {
                                    door.Unlock();
                                    _inventory.InventoryList.RemoveAt(i);
                                }
                            }
                        }
                    }
                    collides = true;
                }
            }

            return collides;
        }

        public bool CheckEnemyToEnvironment(IEnemy enemy)
        {
            foreach (var env in EnvironmentCollidable)
            { 
                if (env.BoundingRectangle.Intersects(enemy.BoundingRectangle))
                {
                    return true;
                }
            }

            return false;
        }

        public void SummonEnemy(IEnemy enemy)
        {
            _enemiesToAdd.Add(enemy);
        }

        public void SummonDoor(Door door)
        {
            EnvironmentCollidable.Add(door);
        }

        private void CheckEnemyToPlayer()
        {
            foreach (var enemy in _enemies)
            {
                if(enemy.BoundingRectangle.Intersects(_game.Player.BoundingRectangle))
                {
                    if (enemy.IsAlive && !enemy.Dying)
                        enemy.Attack(_game.Player);
                }
            }
        }

        private void CheckPlayerItemToEnemy()
        {
            // Check if the weapon is being swung
            if (_game.Player.LeftHand.HeldItem != null && _game.Player.LeftHand.HeldItem.InUse)
            {
                // For each enemy check if the player's weapon hitbox overlaps the enemy hitbox
                foreach (var enemy in _enemies.Where(enemy => !enemy.IsDamaged &&
                                                              _game.Player.LeftHand.ItemBoundingRectangle.CollidesWith(enemy.BoundingRectangle) &&
                                                              !_hitEnemies.Contains(enemy)))
                {
                    enemy.TakeDamage(_game.Player.LeftHand.HeldItem.ItemStats.MeleeDamage);
                    _hitEnemies.Add(enemy);
                }
            }
            else
            {   
                // If the weapon swing is over, reset the enemies that have been hit
                _hitEnemies.Clear();
            }
        }

        private void CheckProjectilesToEnvironment()
        {
            foreach (var proj in Projectiles)
            {
                foreach (var env in EnvironmentCollidable)
                {
                    if (proj.RotatedRectangle.CollidesWith(env.BoundingRectangle))
                    {
                        proj.Live = false;
                        proj.Hit();
                    }
                }
            }
        }

        private void CheckProjectilesToEntities()
        {
            foreach (var proj in Projectiles)
            {
                foreach (var enemy in _enemies)
                {
                    if (proj.Owner.Equals(_game.Player) &&
                        !enemy.IsDamaged &&
                        proj.RotatedRectangle.CollidesWith(enemy.BoundingRectangle))
                    {
                            proj.Hit();
                            enemy.TakeDamage(proj.Damage);
                            proj.Live = false;
                    }
                }

                if (!proj.Owner.Equals(_game.Player) &&
                    !_game.Player.IsDamaged &&
                    proj.RotatedRectangle.CollidesWith(_game.Player.BoundingRectangle)) 
                {
                        proj.Hit();
                        _game.Player.TakeDamage(proj.Damage);
                        proj.Live = false;
                }
            }
        }

        private void CheckPlayerToItems()
        {
            foreach (var item in Items)
            {
                // Pick up items by walking over them.
                if (item.BoundingRectangle.Intersects(_game.Player.BoundingRectangle))
                {
                    switch (item.ItemState)
                    {
                        case ItemStateEnums.Ground:

                            if (_game.Inventory.InventoryList.Count < _inventoryMaxCount)
                            {
                                _inventory.Collect(item);
                                item.PickUp();
                            }

                            break;
                        case ItemStateEnums.Dropped:
                            item.ItemState = ItemStateEnums.Dropping;
                            break;
                    }
                }
                // Prevent the player from picking up an item they just dropped
                // This uses three states. Items go dropping->dropped->ground.
                // This prevents the player picking up an item as soon as they drop it.
                // The idea is to make sure the player isn't intersecting the item for at least 2 update cycles.
                else
                {
                    item.ItemState = item.ItemState switch
                    {
                        ItemStateEnums.Dropping => ItemStateEnums.Dropped,
                        ItemStateEnums.Dropped => ItemStateEnums.Ground,
                        _ => item.ItemState
                    };
                }
            }
        }
    }
}
