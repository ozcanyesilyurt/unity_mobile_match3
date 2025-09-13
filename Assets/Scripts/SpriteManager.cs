using UnityEngine;
using System.Collections.Generic;

public class SpriteManager : MonoBehaviour
{
    public static SpriteManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeDictionaries();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Tile Sprites
    public Sprite[] RedSprites;
    public Sprite[] PinkSprites;
    public Sprite[] BlueSprites;
    public Sprite[] GreenSprites;
    public Sprite[] YellowSprites;
    public Sprite[] PurpleSprites;
    public Sprite[] BrownSprites;

    // Background Sprites
    public Sprite[] WhiteBackgroundSprites;
    public Sprite[] BlackBackgroundSprites;

    // Obstacle Sprites
    public Sprite[] RockObstacleSprites;
    public Sprite[] WoodObstacleSprites;
    public Sprite[] IceObstacleSprites;
    public Sprite[] MetalObstacleSprites;

    private Dictionary<Match3Enums.TileType, Sprite[]> tileSprites;
    private Dictionary<Match3Enums.BackgroundType, Sprite[]> backgroundSprites;
    private Dictionary<Match3Enums.ObstacleType, Sprite[]> obstacleSprites;

    private void InitializeDictionaries()
    {
        tileSprites = new Dictionary<Match3Enums.TileType, Sprite[]>
        {
            { Match3Enums.TileType.Red, RedSprites },
            { Match3Enums.TileType.Pink, PinkSprites },
            { Match3Enums.TileType.Blue, BlueSprites },
            { Match3Enums.TileType.Green, GreenSprites },
            { Match3Enums.TileType.Yellow, YellowSprites },
            { Match3Enums.TileType.Purple, PurpleSprites },
            { Match3Enums.TileType.Brown, BrownSprites }
        };

        backgroundSprites = new Dictionary<Match3Enums.BackgroundType, Sprite[]>
        {
            { Match3Enums.BackgroundType.White, WhiteBackgroundSprites },
            { Match3Enums.BackgroundType.Black, BlackBackgroundSprites }
        };

        obstacleSprites = new Dictionary<Match3Enums.ObstacleType, Sprite[]>
        {
            { Match3Enums.ObstacleType.Rock, RockObstacleSprites },
            { Match3Enums.ObstacleType.Wood, WoodObstacleSprites },
            { Match3Enums.ObstacleType.Ice, IceObstacleSprites },
            { Match3Enums.ObstacleType.Metal, MetalObstacleSprites }
        };
    }

    public static T GetRepeatingElement<T>(T[] array, int index)
    {
        return array[index % array.Length];
    }

    private Sprite GetRandomSprite<T>(Dictionary<T, Sprite[]> spriteDict, T type)
    {
        if (spriteDict.TryGetValue(type, out Sprite[] selectedArray) && selectedArray != null && selectedArray.Length > 0)
        {
            return selectedArray[Random.Range(0, selectedArray.Length)];
        }
        return null;
    }

    private Sprite GetRepeatingSprite<T>(Dictionary<T, Sprite[]> spriteDict, T type, int index)
    {
        if (spriteDict.TryGetValue(type, out Sprite[] selectedArray) && selectedArray != null && selectedArray.Length > 0)
        {
            return GetRepeatingElement(selectedArray, index);
        }
        return null;
    }

    public Sprite GetRandomSpriteTileType(Match3Enums.TileType type)
    {
        return GetRandomSprite(tileSprites, type);
    }

    public Sprite GetRandomSpriteBackground(Match3Enums.BackgroundType type)
    {
        return GetRandomSprite(backgroundSprites, type);
    }

    public Sprite GetRandomSpriteObstacle(Match3Enums.ObstacleType type)
    {
        return GetRandomSprite(obstacleSprites, type);
    }

    public Sprite GetRepeatingSpriteTileType(Match3Enums.TileType type, int index)
    {
        return GetRepeatingSprite(tileSprites, type, index);
    }

    public Sprite GetRepeatingSpriteBackground(Match3Enums.BackgroundType type, int index)
    {
        return GetRepeatingSprite(backgroundSprites, type, index);
    }

    public Sprite GetRepeatingSpriteObstacle(Match3Enums.ObstacleType type, int index)
    {
        return GetRepeatingSprite(obstacleSprites, type, index);
    }
}