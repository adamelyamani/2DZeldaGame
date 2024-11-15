using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Project.Sprites.Environment
{
    public class SpikeTrap : Trap
    {
        public const int SpikeTrapTextureWidth = 96;
        public const int SpikeTrapTextureHeight = 46;
        public const int SpikeTrapTextureRows = 1;
        public const int SpikeTrapTextureColumns = 3;
        private Vector2 _position;
        private int _counter = 0;

        public SpikeTrap(SpriteBatch spriteBatch, Game game) : base(spriteBatch, game, EnvironmentTypeEnums.SpikeTrap.ToString())
        {
            Active = false;
            Damage = 2;
            Width = SpikeTrapTextureWidth;
            Height = SpikeTrapTextureHeight;
            Rows = SpikeTrapTextureRows;
            Columns = SpikeTrapTextureColumns;
            TotalFrames = Rows * Columns;
        }
        public override Vector2 Position
        {
            get => _position;
            set
            {
                BoundingRectangle = new Rectangle((int)value.X, (int)value.Y, SpikeTrapTextureWidth / SpikeTrapTextureColumns, SpikeTrapTextureHeight / SpikeTrapTextureRows);
                _position = value;
            }
        }

        public override void Update(GameTime gameTime)
        {
            _counter++;
            if(_counter > 50 && _counter <= 60)
            {
                CurrentFrame = 1;
                if(_counter == 60)
                {
                    CurrentFrame = 2;
                    Active = true;
                }
                
            }
            if(_counter > 90 && _counter <= 100)
            {
                CurrentFrame = 1;
                if(_counter == 100)
                {
                    CurrentFrame = 0;
                    Active = false;
                    _counter = 0;
                }
            }

        }
    }
}
