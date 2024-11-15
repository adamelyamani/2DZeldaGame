using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Sprites.Environment
{
    public class Door : Environment
    {
        public DirectionEnums Direction;
        public bool Locked { get; set; }
        private readonly Texture2D _openTex;

        public Door(SpriteBatch spriteBatch, Game game, EnvironmentTypeEnums type, DirectionEnums direction) : base(spriteBatch, game, type.ToString())
        {
            this.Direction = direction;
            Locked = type.ToString().Contains("Closed");
            if (Locked)
            {
                switch (direction)
                {
                    case DirectionEnums.North:
                        _openTex = game.Content.Load<Texture2D>("DoorOpenN");
                        Position = new Vector2(592, 0);
                        break;
                    case DirectionEnums.South:
                        _openTex = game.Content.Load<Texture2D>("DoorOpenS");
                        Position = new Vector2(592, 632);
                        break;
                    case DirectionEnums.East:
                        _openTex = game.Content.Load<Texture2D>("DoorOpenE");
                        Position = new Vector2(1200, 328);
                        break;
                    case DirectionEnums.West:
                        _openTex = game.Content.Load<Texture2D>("DoorOpenW");
                        Position = new Vector2(0, 328);
                        break;
                }
            }
        }

        public void Unlock()
        {
            this.Texture = _openTex;
            Locked = false;
        }
        public override void Update(GameTime gameTime)
        {
        }
    }
}
