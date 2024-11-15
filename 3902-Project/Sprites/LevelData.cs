#nullable enable
using System;
using Microsoft.Xna.Framework;

namespace Project.Sprites;

public class LevelData
{
    public string Name { get; set; } = "ErrorLevelName";
    public Vector2 PlayerStartingPosition { get; set; }
    public EnvironmentObjectLevelData[] Environment { get; set; } = Array.Empty<EnvironmentObjectLevelData>();
    public EnemyLevelObjectData[] Enemies { get; set; } = Array.Empty<EnemyLevelObjectData>();
    public ItemLevelObjectData[] Items { get; set; } = Array.Empty<ItemLevelObjectData>();
}

public class LevelObjectData
{
    public string Type { get; set; } = "ErrorType";
}

public class EnvironmentObjectLevelData : LevelObjectData
{
    public float X { get; set; } = 0;
    public float Y { get; set; } = 0;
    public bool IsCollidable { get; set; } = true;
}

public class EnemyLevelObjectData : LevelObjectData
{
    public float X { get; set; } = 0;
    public float Y { get; set; } = 0;
    public int MaxHealth { get; set; } = -1;
}
public class ItemLevelObjectData : LevelObjectData
{
    public float X { get; set; } = 0;
    public float Y { get; set; } = 0;
}