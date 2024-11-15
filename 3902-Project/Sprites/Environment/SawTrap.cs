using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Sprites.Environment
{
    public class SawTrap : Trap
    {
        public const int SawTrapTextureWidth = 96;
        public const int SawTrapTextureHeight = 32;
        public const int SawTrapTextureRows = 1;
        public const int SawTrapTextureColumns = 3;
        private Vector2 _position;
        private int _counter = 0;

        public SawTrap(SpriteBatch spriteBatch, Game game) : base(spriteBatch, game, EnvironmentTypeEnums.SawTrap.ToString())
        {
            Active = true;
            Damage = 1;
            Width = SawTrapTextureWidth;
            Height = SawTrapTextureHeight;
            Rows = SawTrapTextureRows;
            Columns = SawTrapTextureColumns;
            TotalFrames = Rows * Columns;
        }
        public override Vector2 Position
        {
            get => _position;
            set
            {
                BoundingRectangle = new Rectangle((int)value.X, (int)value.Y, SawTrapTextureWidth / SawTrapTextureColumns, SawTrapTextureHeight / SawTrapTextureRows);
                _position = value;
            }
        }

        public override void Update(GameTime gameTime)
        {
            _counter++;
            if (_counter > 5)
            {
                CurrentFrame++;
                if (CurrentFrame == TotalFrames)
                {
                    CurrentFrame = 0;
                }
                _counter = 0;
            }

        }
    }

    
}
