public class match3Enums
{
    public enum TilePower
    {
        Normal
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