using System;
using Microsoft.Xna.Framework.Graphics;
using Project.App;

namespace Project.Interfaces;

internal abstract class Factory
{
    protected readonly Game1 GameObject;
    protected readonly SpriteBatch SpriteBatchObject;
    
    protected Factory(Game1 game, SpriteBatch spriteBatch)
    {
        GameObject = game;
        SpriteBatchObject = spriteBatch;
    }

    public abstract object Create(Enum type);
};