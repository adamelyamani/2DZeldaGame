using Microsoft.Xna.Framework.Graphics;
using Project.Sprites;
using Project.App;

namespace Project.Interfaces;
internal abstract class ImportFactory : Factory
{
	protected ImportFactory(Game1 game, SpriteBatch spriteBatch) : base(game, spriteBatch) { }

	// Raw objectData overload (this is what should be called)
	public abstract object Create(LevelObjectData levelObjectData);
}