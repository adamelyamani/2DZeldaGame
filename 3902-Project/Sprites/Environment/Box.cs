using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Project.Sprites.Environment
{
    public class Box : Environment
    {
        public const int BoxTextureWidth = 32;
        public const int BoxTextureHeight = 46;
        public const int BoxRows = 1;
        public const int BoxColumns = 3;
        private Vector2 _position;

        public Box(SpriteBatch spriteBatch, Game game, EnvironmentTypeEnums type) : base(spriteBatch, game, type.ToString())
        {
            Width = BoxTextureWidth;
            Height = BoxTextureHeight;
            Rows = BoxRows;
            Columns = BoxColumns;
        }

        public override Vector2 Position
        {
            get => _position;
            set
            {
                BoundingRectangle = new Rectangle((int)value.X, (int)value.Y, BoxTextureWidth, BoxTextureHeight);
                _position = value;
            }
        }
        public override void Update(GameTime gameTime)
        {
        }
    }
}
