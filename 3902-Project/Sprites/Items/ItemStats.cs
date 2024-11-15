namespace Project.Sprites.Items;

// This class has the stats for every type of item.
// No item uses all of the stats. Unused stats are set to 0.
// This is to make loading from a json viable.
public class ItemStats
{
    public ItemStats(int usageTime, int meleeDamage = 0, string projectileName = "Arrow", int projectileDamage = 0, float projectileSpeed = 0.5f, int effectMagnitude = 0, int blockChance = 0, int blockPercent = 0)
    {
        UsageTime = usageTime;
        MeleeDamage = meleeDamage;
        ProjectileName = projectileName;
        ProjectileDamage = projectileDamage;
        ProjectileSpeed = projectileSpeed;
        EffectMagnitude = effectMagnitude;
        BlockChance = blockChance;
        BlockPercent = blockPercent;
    }

    public int UsageTime { get; set; } // Time between uses in ms
    public int MeleeDamage { get; set; } // (Melee) Direct hit damage
    public string ProjectileName { get; set; } // (Bow, Wand, Scepter) Type of the fired projectile
    public int ProjectileDamage { get; set; } // (Bow, Wand, Scepter) Projectile damage
    public float ProjectileSpeed { get; set; } // (Bow, Wand, Scepter) Speed of the fired projectile
    public int EffectMagnitude { get; set; } // (Potion, Key) Effect size of the potion (as a percent)
    public int BlockChance { get; set; } // (Shield) Percent chance to block
    public int BlockPercent { get; set; } // (Shield) Percent of damage blocked when a block succeeds
}