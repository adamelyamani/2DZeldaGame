using Microsoft.Xna.Framework.Graphics;
using Project.App;

namespace Project.Sprites.Items
{
    public class Map : Item
    {
        public Map(SpriteBatch spriteBatch, Game1 game) :
            base(spriteBatch, game, ItemTypeEnums.Map)
        {
        }

    }
}
