using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Random;
using static Match3Enums;

public class LevelFactory
{
    private LevelManager levelManager;
    private SpriteManager spriteManager;
    RectTransform backgroundContainer;
    RectTransform tilesContainer;
    RectTransform playArea;

    private int rowCountTilesandObstacles;//more than backgrounds to allow for tiles to fall in
    private int columnCountTilesandObstacles;
    private int rowCountBackgroundsONLY;
    private int columnCountBackgroundsONLY;
    private Level level;
    private IPoolable[,] tilesAndObstacles; // for match3 tiles and obstacles inside level
    private IPoolable[,] Backgrounds; //playable area inside level


    public LevelFactory(LevelManager levelManager)
    {
        this.levelManager = levelManager;
        this.backgroundContainer = levelManager.backgroundContainer;
        this.tilesContainer = levelManager.tilesContainer;
        this.playArea = levelManager.playArea;
        rowCountBackgroundsONLY = levelManager.currentLevelData.rowCount;
        columnCountBackgroundsONLY = levelManager.currentLevelData.columnCount;
        rowCountTilesandObstacles = rowCountBackgroundsONLY + 4; //more than backgrounds to allow for tiles to fall in
        columnCountTilesandObstacles = columnCountBackgroundsONLY;
        this.spriteManager = levelManager.GetComponent<SpriteManager>();
        this.spriteManager.FlushCache();
    }

    public Level CreateLevel(LevelBuildData levelData)
    {
        this.level = new Level(levelData);
        ConfigureLayout(topOffset: 970f); // or measure blue line distance
    }

    public void CreateCells()
    {
        Backgrounds = new IPoolable[rowCountBackgroundsONLY, columnCountBackgroundsONLY];
        tilesAndObstacles = new IPoolable[rowCountTilesandObstacles, columnCountTilesandObstacles];

    }

    public void CreateLevelBackgrounds()//use scriptable object for bg
    {


    }

    public void CreateLevelObstacles()//use scriptable object for obstacles
    {
        for (int i = 0; i < level.obstacleCount; i++)
        {
            Obstacle obstacle = new Obstacle();
            obstacle.NewCoordinates(Random.Range(0, rowCountTilesandObstacles), Random.Range(0, columnCountTilesandObstacles));
            tilesAndObstacles[obstacle.row, obstacle.column] = obstacle;
            ObstacleType type = level.obstacleTypesAllowed[Random.Range(0, level.obstacleTypesAllowed.Length)];
            obstacle.type = type;
            obstacle.sprite = spriteManager.GetChosenSprite(type, type.ToString());
            GameObject obstacleGO = ObjectPoolManager.SpawnObject(levelManager.obstaclePrefab, levelManager.tilesContainer);
            Util.ObjectToGameObject(obstacle, obstacleGO, level);// set sprite and gameobject recttransform size

        }

    }

    public void CreateLevelTiles()
    {

    }

    public Tile CreateTile(Level level, TileType type, TilePower tilePower)
    {

    }
    public void PlaceIPoolableInLevel(IPoolable obj)
    {

    }

    #region Layout Containers Configuration

    public void ConfigureLayout(float topOffset)
    {
        if (level == null)
        {
            Debug.LogError("Level not created before ConfigureLayout.");
            return;
        }

        SetupContainerSizes();            // compute sizeDelta first
        ApplyAnchorsAndPositions(topOffset);
    }

    private void ApplyAnchorsAndPositions(float topOffset)
    {
        Vector2 topAnchor = new Vector2(0.5f, 1f);

        // Background container & Play Area share same top & height
        SetTopLocked(backgroundContainer, topAnchor, topOffset);
        SetTopLocked(playArea, topAnchor, topOffset);
        playArea.sizeDelta = backgroundContainer.sizeDelta; // ensure identical

        // Tiles container (extra hidden rows on top)
        int extraRows = rowCountTilesandObstacles - rowCountBackgroundsONLY;
        float perRow = level.cellSizeY + level.spacingY;
        float extraPixels = extraRows * perRow;

        SetTopLocked(tilesContainer, topAnchor, topOffset - extraPixels);
        // note: subtract extraPixels here so anchoredPosition becomes
        // (-topOffset + extraPixels) which equals (-topOffset) + extraRows*perRow
    }

    private void SetTopLocked(RectTransform rt, Vector2 anchor, float topOffset)
    {
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, -topOffset);
        rt.localScale = Vector3.one;
    }

    private void SetupContainerSizes()
    {
        // Calculate total width/height including spacing between cells
        float totalWidth = (columnCountBackgroundsONLY * level.cellSizeX) + ((columnCountBackgroundsONLY - 1) * level.spacingX);
        float bgHeight = (rowCountBackgroundsONLY * level.cellSizeY) + ((rowCountBackgroundsONLY - 1) * level.spacingY);
        float tilesHeight = (rowCountTilesandObstacles * level.cellSizeY) + ((rowCountTilesandObstacles - 1) * level.spacingY);

        backgroundContainer.sizeDelta = new Vector2(totalWidth, bgHeight);
        playArea.sizeDelta = new Vector2(totalWidth, bgHeight);
        tilesContainer.sizeDelta = new Vector2(totalWidth, tilesHeight);
    }
    #endregion

}
