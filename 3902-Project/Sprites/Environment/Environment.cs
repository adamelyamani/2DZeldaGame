using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Project.Sprites.Environment
{
    public abstract class Environment : IEnvironment
    {
        private const int DefaultRows = 1;
        private const int DefaultColumns = 1;

        public int TotalFrames;
        public int CurrentFrame;
        private Vector2 _position;

        protected Environment(SpriteBatch spriteBatch, Game game, string textureString)
        {
            SpriteBatchObject = spriteBatch; 
            Texture = game.Content.Load<Texture2D>(textureString);

            //Default values
            Rows = DefaultRows;
            Columns = DefaultColumns;
            Width = Texture.Width;
            Height = Texture.Height;
            IsCollidable = false;
            Position = Vector2.Zero;

            TotalFrames = Rows * Columns;
            CurrentFrame = 0;
        }

        public bool IsCollidable { get; set; }

        public int Rows { get; set; }

        public int Columns { get; set; } 

        public int Width { get; protected set; }

        public int Height { get; protected set; }

        public Texture2D Texture { get; set; }

        public virtual Vector2 Position
        {
            get => _position;

            set
            {
                BoundingRectangle = new Rectangle((int)value.X, (int)value.Y, Texture.Width, Texture.Height);
                _position = value;
            }
        }

        public Rectangle BoundingRectangle { get; set; }

        public SpriteBatch SpriteBatchObject { get; protected set; }

        public virtual void Draw()
        {
            int width = Texture.Width / Columns;
            int height = Texture.Height / Rows;
            int row = CurrentFrame / Columns;
            int column = CurrentFrame % Columns;

            var sourceRectangle = new Rectangle(width * column, height * row, width, height);
            var destinationRectangle = new Rectangle((int)Position.X, (int)Position.Y, width, height);

            SpriteBatchObject.Begin();
            SpriteBatchObject.Draw(Texture, destinationRectangle, sourceRectangle, Color.White);
            SpriteBatchObject.End();
        }

    public abstract void Update(GameTime gameTime);

    }
}
