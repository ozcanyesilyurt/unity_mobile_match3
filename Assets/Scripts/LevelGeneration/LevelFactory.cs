using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Match3Enums;

public class LevelFactory
{
    private LevelManager levelManager;

    public LevelFactory(LevelManager levelManager)
    {
        this.levelManager = levelManager;
    }

    public Level CreateLevel(LevelBuildData levelData)
    {

    }

    public Level CreateLevelBackgrounds(Level level)//use scriptable object for bg
    {

    }

    public Level CreateLevelObstacles(Level level)//use scriptable object for obstacles
    {

    }
    public Level CreateLevelTiles(Level level)
    {
        return level;
    }
    public Tile CreateTile(Level level, TileType type, TilePower tilePower)
    {

    }

    public void TileStartPosition(Tile tile)
    {
        
    }

    public void SetTileSpritesDict(Level level)//set tile sprite based on level data
    {

    }
}
