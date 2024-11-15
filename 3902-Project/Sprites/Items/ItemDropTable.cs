using System;
using System.Collections.Generic;
namespace Project.Sprites.Items;

public static class ItemDropTable
{
    private static readonly List<Tuple<ItemTypeEnums, int>> WeightList = new();
    private static readonly int TotalWeight = 0;
    private static readonly Random Rnd = new Random();

    static ItemDropTable()
    {
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.WoodenGreatsword, 200));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.WoodenMallet, 100));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.WoodenDagger, 100));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.WoodenSpear, 100));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.WoodenBow, 130));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.WoodenScepter, 130));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.WoodenWand, 130));

        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.BoneGreatsword, 30));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.BoneMallet, 30));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.BoneSpear, 30));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.BoneDagger, 30));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.BoneBow, 50));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.BoneScepter, 50));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.BoneWand, 50));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.BoneRake, 10));

        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.LargeHealthPotion, 300));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.SmallHealthPotion, 600));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.LargeShieldPotion, 200));
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.SmallShieldPotion, 500));

        // Chance of dropping nothing
        WeightList.Add(new Tuple<ItemTypeEnums, int>(ItemTypeEnums.None, 2000));


        foreach (var item in WeightList)
        {
            TotalWeight += item.Item2;
        }
    }

    public static ItemTypeEnums Generate()
    {
        var stopWeight = Rnd.Next(0,TotalWeight);

        foreach (var item in WeightList)
        {
            stopWeight -= item.Item2;
            if (stopWeight < 0)
            {
                return item.Item1;
            }
        }

        // This should never be called 
        return ItemTypeEnums.None;
    }
}