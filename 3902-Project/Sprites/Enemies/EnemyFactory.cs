using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Project.App;
using Project.Interfaces;

namespace Project.Sprites.Enemies;
internal class EnemyFactory : ImportFactory
{
    public EnemyFactory(Game1 game, SpriteBatch spriteBatch) : base(game, spriteBatch) { }

    // Create a new enemy based on an Enum value
    // Satisfies Factory
    public override IEnemy Create(Enum enemyType)
    {
        return enemyType switch
        {
            EnemyTypeEnums.SkeletonBase    => new SkeletonBase   (SpriteBatchObject, GameObject),
            EnemyTypeEnums.SkeletonMage    => new SkeletonMage   (SpriteBatchObject, GameObject),
            EnemyTypeEnums.SkeletonRogue   => new SkeletonRogue  (SpriteBatchObject, GameObject),
            EnemyTypeEnums.SkeletonWarrior => new SkeletonWarrior(SpriteBatchObject, GameObject),
            EnemyTypeEnums.OrcBase         => new OrcBase        (SpriteBatchObject, GameObject),
            EnemyTypeEnums.OrcMage         => new OrcMage        (SpriteBatchObject, GameObject),
            EnemyTypeEnums.OrcRogue        => new OrcRogue       (SpriteBatchObject, GameObject),
            EnemyTypeEnums.OrcWarrior      => new OrcWarrior     (SpriteBatchObject, GameObject),
            EnemyTypeEnums.BossGigaMage    => new GigaMageBoss   (SpriteBatchObject, GameObject),
            _ => throw new NotImplementedException("Unsupported Enemy Type: \"" + enemyType + "\""),
        };
    }

    // Converts the LevelData String identifier of an enemy into an EnemyTypeEnums value, then calls Create on it
    // Satisfies ImportFactory
    public override IEnemy Create(LevelObjectData levelObjectData)
    {
        // Cast levelObjectData to EnemyLevelObjectData
        var rawEnemyData = levelObjectData as EnemyLevelObjectData ?? throw new ArgumentException($"\"{levelObjectData.Type}\" does not have type EnemyLevelObjectData.");

        if (!Enum.TryParse(rawEnemyData.Type, out EnemyTypeEnums parsedEnemyType))
        {
            throw new NotImplementedException("Enemy String Type: \"" + rawEnemyData.Type + "\" cannot be parsed into EnemyTypeEnums");
        }

        // Create the enemy object
        var newEnemy = Create(parsedEnemyType);

        // If health is specified in the json, overwrite the default health
        if (rawEnemyData.MaxHealth > 0)
        {
            newEnemy.MaxHealth = rawEnemyData.MaxHealth;
            newEnemy.Health = rawEnemyData.MaxHealth;
        }
        // Set enemy position
        newEnemy.Spawn = new Vector2(rawEnemyData.X, rawEnemyData.Y);

        return newEnemy;
    }
}