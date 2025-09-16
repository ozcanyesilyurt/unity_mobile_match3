using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Match3Enums;

public class LevelFactory
{
    private LevelManager levelManager;
    private SpriteManager spriteManager;



    private Level level;
    private GameObject[,] tiles; //inside level
    private GameObject[,] cells; //inside level


    public LevelFactory(LevelManager levelManager)
    {
        this.levelManager = levelManager;
        this.spriteManager = levelManager.GetComponent<SpriteManager>();
        this.spriteManager.FlushCache();
    }

    public Level CreateLevel(LevelBuildData levelData)
    {
        this.level = new Level(levelData);
    }

    public void CreateCells()
    {

    }

    public void CreateLevelBackgrounds()//use scriptable object for bg
    {


    }

    public void CreateLevelObstacles()//use scriptable object for obstacles
    {

    }
    public void CreateLevelTiles()
    {

    }
    public Tile CreateTile(Level level, TileType type, TilePower tilePower)
    {

    }

    public void TileStartPosition(Tile tile)
    {

    }

    public void SetTileSpritesDict()//set tile sprite based on level data
    {

    }

}
