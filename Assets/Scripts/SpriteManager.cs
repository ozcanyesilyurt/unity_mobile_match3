using UnityEngine;
using System.Collections.Generic;

public class SpriteManager
{
    private Dictionary<Match3Enums.TileType, Sprite[]> tileSprites;
    private Dictionary<Match3Enums.TileType, Sprite[]> tilePowerSprites;

    public SpriteManager()
    {
        InitializeDictionaries();
    }


    // Tile Sprites
    public Sprite[] AllRedSprites;
    public Sprite[] AllPinkSprites;
    public Sprite[] AllBlueSprites;
    public Sprite[] AllGreenSprites;
    public Sprite[] AllYellowSprites;
    public Sprite[] AllPurpleSprites;
    public Sprite[] AllBrownSprites;

    // TilePower Sprites
    public Sprite[] AllRedPowerSprites;
    public Sprite[] AllPinkPowerSprites;
    public Sprite[] AllBluePowerSprites;
    public Sprite[] AllGreenPowerSprites;
    public Sprite[] AllYellowPowerSprites;
    public Sprite[] AllPurplePowerSprites;
    public Sprite[] AllBrownPowerSprites;

    // Background Sprites
    public Sprite[] AllWhiteBackgroundSprites;
    public Sprite[] AllBlackBackgroundSprites;

    // Obstacle Sprites
    public Sprite[] AllRockObstacleSprites;
    public Sprite[] AllWoodObstacleSprites;
    public Sprite[] AllIceObstacleSprites;
    public Sprite[] AllMetalObstacleSprites;

    private Dictionary<Match3Enums.TileType, Sprite[]> AlltileSprites;
    private Dictionary<Match3Enums.TileType, Sprite[]> AlltilePowerSprites;
    private Dictionary<Match3Enums.BackgroundType, Sprite[]> AllbackgroundSprites;
    private Dictionary<Match3Enums.ObstacleType, Sprite[]> AllobstacleSprites;

    private readonly Dictionary<string, Sprite> _spriteCache = new Dictionary<string, Sprite>();
    private void InitializeDictionaries()
    {
        // Initialize your main sprite dictionaries
        tileSprites = new Dictionary<Match3Enums.TileType, Sprite[]>
        {
            { Match3Enums.TileType.Red, AllRedSprites },
            { Match3Enums.TileType.Pink, AllPinkSprites },
            { Match3Enums.TileType.Blue, AllBlueSprites },
            { Match3Enums.TileType.Green, AllGreenSprites },
            { Match3Enums.TileType.Yellow, AllYellowSprites },
            { Match3Enums.TileType.Purple, AllPurpleSprites },
            { Match3Enums.TileType.Brown, AllBrownSprites }
        };

        tilePowerSprites = new Dictionary<Match3Enums.TileType, Sprite[]>
        {
            { Match3Enums.TileType.Red, AllRedPowerSprites },
            { Match3Enums.TileType.Pink, AllPinkPowerSprites },
            { Match3Enums.TileType.Blue, AllBluePowerSprites },
            { Match3Enums.TileType.Green, AllGreenPowerSprites },
            { Match3Enums.TileType.Yellow, AllYellowPowerSprites },
            { Match3Enums.TileType.Purple, AllPurplePowerSprites },
            { Match3Enums.TileType.Brown, AllBrownPowerSprites }
        };

        AllbackgroundSprites = new Dictionary<Match3Enums.BackgroundType, Sprite[]>
        {
            { Match3Enums.BackgroundType.White, AllWhiteBackgroundSprites },
            { Match3Enums.BackgroundType.Black, AllBlackBackgroundSprites }
        };

        AllobstacleSprites = new Dictionary<Match3Enums.ObstacleType, Sprite[]>
        {
            { Match3Enums.ObstacleType.Rock, AllRockObstacleSprites },
            { Match3Enums.ObstacleType.Wood, AllWoodObstacleSprites },
            { Match3Enums.ObstacleType.Ice, AllIceObstacleSprites },
            { Match3Enums.ObstacleType.Metal, AllMetalObstacleSprites }
        };
    }
    public Sprite GetSprite(System.Enum enumValue, bool hasPower = false)
    {
        // Create a unique key for caching based on enum type, value and power status
        string cacheKey = $"{enumValue.GetType().Name}_{enumValue}_{hasPower}";

        // Return cached sprite if it exists
        if (_spriteCache.ContainsKey(cacheKey))
        {
            return _spriteCache[cacheKey];
        }

        // Get appropriate sprite array based on the enum type
        Sprite[] spriteArray = null;

        if (enumValue is Match3Enums.TileType tileType)
        {
            spriteArray = hasPower ? tilePowerSprites[tileType] : tileSprites[tileType];
        }
        else if (enumValue is Match3Enums.BackgroundType bgType)
        {
            spriteArray = AllbackgroundSprites[bgType];
        }
        else if (enumValue is Match3Enums.ObstacleType obstacleType)
        {
            spriteArray = AllobstacleSprites[obstacleType];
        }

        // If no sprite array found or it's empty, return null
        if (spriteArray == null || spriteArray.Length == 0)
        {
            Debug.LogWarning($"No sprites found for {enumValue}");
            return null;
        }

        // Get a random sprite from the array
        Sprite randomSprite = spriteArray[Random.Range(0, spriteArray.Length)];

        // Cache the sprite for future use
        _spriteCache[cacheKey] = randomSprite;

        return randomSprite;
    }
}