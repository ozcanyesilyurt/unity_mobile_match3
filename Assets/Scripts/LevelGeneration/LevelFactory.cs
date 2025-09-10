using UnityEngine;

public class LevelFactory //Factory Design Pattern
{
    public Level CreateLevelBGO(LevelBuildData levelData)//use scriptable object for bg and obstacles
    {
        return new Level(levelData);
    }
    public Level CreateLevelBGO(int rows, int columns, int obstacleCount, Sprite[] obstacles, Sprite[] tileTypes)// use parameters for bg and obstacles
    {
        return new Level(rows, columns, obstacleCount, obstacles, tileTypes);
    }

    public Level CreateLevelTiles(Level level)//TDO: implement tile creation
    {
        return null;
    }
}
