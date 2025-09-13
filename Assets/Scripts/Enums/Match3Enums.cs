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
        Red,
        Pink,
        Blue,
        Green,
        Yellow,
        Purple,
        Brown,
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