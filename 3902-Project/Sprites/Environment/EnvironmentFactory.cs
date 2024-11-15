using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Interfaces;

namespace Project.Sprites.Environment;
internal class EnvironmentFactory : ImportFactory
{
    public EnvironmentFactory(Game1 game, SpriteBatch spriteBatch) : base(game, spriteBatch) { }

    public override IEnvironment Create(Enum type)
    {
        // cracked switch statement to create objects based on text
        return type switch
        {
            EnvironmentTypeEnums.WoodCrateBreakable => new Box(SpriteBatchObject, GameObject, EnvironmentTypeEnums.WoodCrateBreakable),
            EnvironmentTypeEnums.WoodBarrelBreakable => new Box(SpriteBatchObject, GameObject, EnvironmentTypeEnums.WoodBarrelBreakable),
            EnvironmentTypeEnums.WoodCrateLarge => new LargeObstacle(SpriteBatchObject, GameObject, EnvironmentTypeEnums.WoodCrateLarge),
            EnvironmentTypeEnums.DoorClosedN => new Door(SpriteBatchObject, GameObject, EnvironmentTypeEnums.DoorClosedN, DirectionEnums.North),
            EnvironmentTypeEnums.DoorClosedE => new Door(SpriteBatchObject, GameObject, EnvironmentTypeEnums.DoorClosedE, DirectionEnums.East),
            EnvironmentTypeEnums.DoorClosedS => new Door(SpriteBatchObject, GameObject, EnvironmentTypeEnums.DoorClosedS, DirectionEnums.South),
            EnvironmentTypeEnums.DoorClosedW => new Door(SpriteBatchObject, GameObject, EnvironmentTypeEnums.DoorClosedW, DirectionEnums.West),
            EnvironmentTypeEnums.DoorOpenN => new Door(SpriteBatchObject, GameObject, EnvironmentTypeEnums.DoorOpenN, DirectionEnums.North),
            EnvironmentTypeEnums.DoorOpenE => new Door(SpriteBatchObject, GameObject, EnvironmentTypeEnums.DoorOpenE, DirectionEnums.East),
            EnvironmentTypeEnums.DoorOpenS => new Door(SpriteBatchObject, GameObject, EnvironmentTypeEnums.DoorOpenS, DirectionEnums.South),
            EnvironmentTypeEnums.DoorOpenW => new Door(SpriteBatchObject, GameObject, EnvironmentTypeEnums.DoorOpenW, DirectionEnums.West),
            EnvironmentTypeEnums.LargeFloorDungeon1 => new LargeBlock(SpriteBatchObject, GameObject, EnvironmentTypeEnums.LargeFloorDungeon1),
            EnvironmentTypeEnums.LargeWoodFloor1 => new LargeBlock(SpriteBatchObject, GameObject, EnvironmentTypeEnums.LargeWoodFloor1),
            EnvironmentTypeEnums.LargeWoodFloor2 => new LargeBlock(SpriteBatchObject, GameObject, EnvironmentTypeEnums.LargeWoodFloor2),
            EnvironmentTypeEnums.BackWall => new LargeBlock(SpriteBatchObject, GameObject, EnvironmentTypeEnums.BackWall),
            EnvironmentTypeEnums.GenericTiles => new LargeBlock(SpriteBatchObject, GameObject, EnvironmentTypeEnums.GenericTiles),
            EnvironmentTypeEnums.FullFloor => new LargeBlock(SpriteBatchObject, GameObject, EnvironmentTypeEnums.FullFloor),
            EnvironmentTypeEnums.FullFloor2 => new LargeBlock(SpriteBatchObject, GameObject, EnvironmentTypeEnums.FullFloor2),
            EnvironmentTypeEnums.BackWallPillar => new Pillar(SpriteBatchObject, GameObject, EnvironmentTypeEnums.BackWallPillar),
            EnvironmentTypeEnums.SmallBlock1 => new SmallBlock(SpriteBatchObject, GameObject, EnvironmentTypeEnums.SmallBlock1),
            EnvironmentTypeEnums.SmallBlock2 => new SmallBlock(SpriteBatchObject, GameObject, EnvironmentTypeEnums.SmallBlock2),
            EnvironmentTypeEnums.SmallBlock3 => new SmallBlock(SpriteBatchObject, GameObject, EnvironmentTypeEnums.SmallBlock3),
            EnvironmentTypeEnums.SmallBlock4 => new SmallBlock(SpriteBatchObject, GameObject, EnvironmentTypeEnums.SmallBlock4),
            EnvironmentTypeEnums.SmallBlock5 => new SmallBlock(SpriteBatchObject, GameObject, EnvironmentTypeEnums.SmallBlock5),
            EnvironmentTypeEnums.SmallBlock6 => new SmallBlock(SpriteBatchObject, GameObject, EnvironmentTypeEnums.SmallBlock6),
            EnvironmentTypeEnums.SmallBlock7 => new SmallBlock(SpriteBatchObject, GameObject, EnvironmentTypeEnums.SmallBlock7),
            EnvironmentTypeEnums.DungeonWindow => new Window(SpriteBatchObject, GameObject, EnvironmentTypeEnums.DungeonWindow),
            EnvironmentTypeEnums.WoodWindow    => new Window(SpriteBatchObject, GameObject, EnvironmentTypeEnums.WoodWindow),
            EnvironmentTypeEnums.WallTopLeftCorner => new Wall(SpriteBatchObject, GameObject, EnvironmentTypeEnums.WallTopLeftCorner),
            EnvironmentTypeEnums.WallTopRightCorner => new Wall(SpriteBatchObject, GameObject, EnvironmentTypeEnums.WallTopRightCorner),
            EnvironmentTypeEnums.WallBottomLeftCorner => new Wall(SpriteBatchObject, GameObject, EnvironmentTypeEnums.WallBottomLeftCorner),
            EnvironmentTypeEnums.WallBottomRightCorner => new Wall(SpriteBatchObject, GameObject, EnvironmentTypeEnums.WallBottomRightCorner),
            EnvironmentTypeEnums.WallStraightVertical => new Wall(SpriteBatchObject, GameObject, EnvironmentTypeEnums.WallStraightVertical),
            EnvironmentTypeEnums.WallStraightHorizontal => new Wall(SpriteBatchObject, GameObject, EnvironmentTypeEnums.WallStraightHorizontal),
            EnvironmentTypeEnums.SawTrap => new SawTrap(SpriteBatchObject, GameObject),
            EnvironmentTypeEnums.SpikeTrap => new SpikeTrap(SpriteBatchObject, GameObject),
            _ => throw new NotImplementedException("Unsupported Environment Type: \"" + (EnvironmentTypeEnums)type + "\""),  
        };
    }
    public override IEnvironment Create(LevelObjectData levelObjectData)
    {
        // Cast levelObjectData to EnvironmentObjectLevelData
        var rawEnvironmentData = levelObjectData as EnvironmentObjectLevelData ?? throw new ArgumentException($"\"{levelObjectData.Type}\" does not have type EnvironmentObjectLevelData.");

        if (!Enum.TryParse(rawEnvironmentData.Type, out EnvironmentTypeEnums parsedEnvironmentType))
        {
            throw new NotImplementedException("Environment String Type: \"" + rawEnvironmentData.Type + "\" cannot be parsed into EnvironmentTypeEnums");
        }

        // Create the environment object
        var newEnvironment = Create(parsedEnvironmentType);

        // Set object-specific parameters based on the raw objectData
        newEnvironment.Position = new Vector2(rawEnvironmentData.X, rawEnvironmentData.Y);
        newEnvironment.IsCollidable = rawEnvironmentData.IsCollidable;

        return newEnvironment;
    }
}