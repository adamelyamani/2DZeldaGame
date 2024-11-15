using Project.App;
using Project.Sprites;
using System;
using Microsoft.Xna.Framework;
using System.ComponentModel;
using Project.Sprites.Environment;

namespace Project.Commands;

public class TransitionRooms : ICommand
{
    readonly Game1 _game;
    public const float Tolerance = 0.00001f;

    public DirectionEnums Direction { get; set; }
    public TransitionRooms(Game1 game) 
    {
        _game = game;
    }

    public void Execute()
    {
        _game.GameState = GameStateEnums.Transition;
        _game.OldLevel = _game.CurrentLevel;

        //Find the "coordinates" of the current level within the game's full map
        Vector2 currentLevelLocation = new Vector2();
        foreach (Tuple<Level, Vector2> pair in _game.FullMap)
        {
            if (_game.CurrentLevel == pair.Item1)
            {
                currentLevelLocation = pair.Item2;
                break;
            }
        }

        //Based on the direction of transition, find the "next" room within the game's full map and set that to the current level
        switch (Direction)
        {
            case DirectionEnums.North:
                foreach (Tuple<Level, Vector2> pair in _game.FullMap)
                {
                    if (Math.Abs(pair.Item2.Y - (currentLevelLocation.Y + 1)) < Tolerance && Math.Abs(pair.Item2.X - currentLevelLocation.X) < Tolerance)
                    {
                        _game.CurrentLevel = pair.Item1;
                        break;
                    }
                }
                break;
            case DirectionEnums.South:
                foreach (Tuple<Level, Vector2> pair in _game.FullMap)
                {
                    if (Math.Abs(pair.Item2.Y - (currentLevelLocation.Y - 1)) < Tolerance && Math.Abs(pair.Item2.X - currentLevelLocation.X) < Tolerance)
                    {
                        _game.CurrentLevel = pair.Item1;
                        break;
                    }
                }
                break;
            case DirectionEnums.East:
                foreach (Tuple<Level, Vector2> pair in _game.FullMap)
                {
                    if (Math.Abs(pair.Item2.X - (currentLevelLocation.X + 1)) < Tolerance && Math.Abs(pair.Item2.Y - currentLevelLocation.Y) < Tolerance)
                    {
                        _game.CurrentLevel = pair.Item1;
                        break;
                    }
                }
                break;
            case DirectionEnums.West:
                foreach (Tuple<Level, Vector2> pair in _game.FullMap)
                {
                    if (Math.Abs(pair.Item2.X - (currentLevelLocation.X - 1)) < Tolerance && Math.Abs(pair.Item2.Y - currentLevelLocation.Y) < Tolerance)
                    {
                        _game.CurrentLevel = pair.Item1;
                        break;
                    }
                }
                break;
            default: throw new InvalidEnumArgumentException();
        }

        //Unlock "connecting" door
        foreach (var temp in _game.CurrentLevel.EnvironmentCollidable)
        {
            if (temp is Door d)
            {
                if(Direction is DirectionEnums.South)
                {
                    if(d.Direction is DirectionEnums.North)
                    {
                        d.Unlock();
                    }
                }
                if (Direction is DirectionEnums.North)
                {
                    if (d.Direction is DirectionEnums.South)
                    {
                        d.Unlock();
                    }
                }
                if (Direction is DirectionEnums.East)
                {
                    if (d.Direction is DirectionEnums.West)
                    {
                        d.Unlock();
                    }
                }
                if (Direction is DirectionEnums.West)
                {
                    if (d.Direction is DirectionEnums.East)
                    {
                        d.Unlock();
                    }
                }
            }
        }
        //Clear all projectiles in the old room
        ProjectileManager.Instance.ClearProjectiles();
    }
}
