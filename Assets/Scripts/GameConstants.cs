public enum MapSize
{
    TINY,
    SMALL,
    MEDIUM,
    LARGE
}

public static class GameConstants
{
    public const int TINY_SIZE = 16;
    public const int SMALL_SIZE = 24;
    public const int MEDIUM_SIZE = 32;
    public const int LARGE_SIZE = 48;
    public const string MAP_FOLDER_NAME = "MapFiles";
    public const int SCREEN_BORDER_THICKNESS = 3;

    public static int MapSizeToInt(MapSize size){
        
        if (size == MapSize.SMALL)
        {
            return SMALL_SIZE;
        }
        else if (size == MapSize.MEDIUM)
        {
            return MEDIUM_SIZE;
        }
        else if (size == MapSize.LARGE)
        {
            return LARGE_SIZE;
        }
        else if (size == MapSize.TINY)
        {
            return TINY_SIZE;
        }
        return 0;
    }
}
