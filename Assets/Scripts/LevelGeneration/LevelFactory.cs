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

    public void CreateLevelBackgrounds(Level level)//use scriptable object for bg
    {

    }

    public void CreateLevelObstacles(Level level)//use scriptable object for obstacles
    {

    }
    public void CreateLevelTiles(Level level)
    {
        
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
