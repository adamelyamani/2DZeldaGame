using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Project.App;

namespace Project.Sprites.Items;

public class ItemStatSheet
{
    private static Dictionary<ItemTypeEnums, ItemStats> _statList;

    // Loads the stats of every item from ItemStats.json file in Content
    public static void Initialize(Game1 game)
    {
        var options = new JsonSerializerOptions
        {
            IncludeFields = true,
            WriteIndented = true
        };

        using (FileStream jsonStream = new(Path.Combine(game.Content.RootDirectory, "ItemStats.json"), FileMode.Open))
        {
            _statList = JsonSerializer.Deserialize<Dictionary<ItemTypeEnums, ItemStats>>(jsonStream, options: options);
        }
    }

    public static ItemStats GetStats(ItemTypeEnums item)
    {
        return _statList[item];
    }
}
