public class Match3Enums
{
    public enum TilePower
    {
        Normal,
        HorizontalClear,
        VerticalClear,
        Bomb,
        ColorClear
    }

    public enum TileType
    {
        Red = 0,
        Pink = 1,
        Blue = 2,
        Green = 3,
        Yellow = 4,
        Purple = 5,
        Brown = 6,
        None = 7
    }
    public enum ObstacleType
    {
        Rock,
        Wood,
        Ice,
        Metal,
        None
    }
    public enum BackgroundType
    {
        White,
        Black,
        None
    }

    public enum GameState//implement game states later
    {
        Waiting,
        Swapping,
        Matching,
        Dropping,
        Refilling,
        Shuffling,
        LevelEnd

    }
}