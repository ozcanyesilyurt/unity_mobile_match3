using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Random;
using static Match3Enums;

public class LevelFactory
{
    private LevelManager levelManager;
    private SpriteManager spriteManager;
    RectTransform tilesContainer;
    RectTransform playArea;
    private float baseCellSizeX, baseCellSizeY, baseSpacingX, baseSpacingY;

    private Level level;
    public IPoolable[,] tilesAndObstacles; // for match3 tiles and obstacles inside level

    private int extraRows; // extra rows above the visible play area for spawning tiles. No extra columns needed

    public LevelFactory(LevelManager levelManager)
    {
        this.extraRows = levelManager.extraRows;
        this.levelManager = levelManager;
        this.tilesContainer = levelManager.tilesContainer;
        this.playArea = levelManager.playArea;
        this.spriteManager = levelManager.GetComponent<SpriteManager>();
        this.spriteManager.FlushCache();
    }

    public Level CreateLevel(LevelBuildData levelData)
    {
        tilesAndObstacles = new IPoolable[levelData.rowCount + extraRows, levelData.columnCount];
        this.level = new Level(levelData);
        baseCellSizeX = level.cellSizeX;
        baseCellSizeY = level.cellSizeY;
        baseSpacingX = level.spacingX;
        baseSpacingY = level.spacingY;
        ConfigureLayoutContainers();
        CreateLevelBackgrounds();
        CreateLevelObstacles();
        CreateLevelTiles();
        return this.level;
    }

    public void CreateLevelBackgrounds()//use scriptable object for bg
    {
        for (int r = 0; r < level.rowCount + extraRows; r++)
        {
            for (int c = 0; c < level.columnCount; c++)
            {
                GameObject bgObj = ObjectPoolManager.SpawnObject(levelManager.backgroundPrefab, tilesContainer);
                BackgroundType bgType = level.backgroundTypesAllowed[Range(0, level.backgroundTypesAllowed.Length)];
                Image bgImage = bgObj.GetComponent<Image>();
                bgImage.sprite = spriteManager.GetRandomSprite(bgType);
                bgObj.GetComponent<Background>().type = bgType;

                Background bg = bgObj.GetComponent<Background>();
                bg.row = r;
                bg.column = c;
                bg.sprite = bgImage.sprite;
                bg.type = bgType;

                PlaceIPoolableInLevel(bgObj.GetComponent<IPoolable>());
            }
        }
    }

    public void CreateLevelObstacles() // use scriptable object for obstacles
    {
        int capacity = level.rowCount * level.columnCount;
        int totalObstacles = Mathf.Min(level.obstacleCount, capacity);

        int placedObstacles = 0;
        int safety = 0, safetyMax = capacity * 4;

        while (placedObstacles < totalObstacles && safety++ < safetyMax)
        {
            int obstacleRow = Random.Range(extraRows, level.rowCount + extraRows);
            int obstacleColumn = Random.Range(0, level.columnCount);

            if (tilesAndObstacles[obstacleRow, obstacleColumn] != null)
                continue; // already occupied, try another cell

            int maxHP = 3;
            GameObject obstacleObj = CreateObstacle(obstacleRow, obstacleColumn, maxHP);
            PlaceIPoolableInLevel(obstacleObj.GetComponent<IPoolable>());
            tilesAndObstacles[obstacleRow, obstacleColumn] = obstacleObj.GetComponent<IPoolable>();
            placedObstacles++;
        }

        if (placedObstacles < totalObstacles)
            Debug.LogWarning($"Placed {placedObstacles}/{totalObstacles} obstacles (ran out of attempts).");
    }

    public void CreateLevelTiles()
    {
        for (int r = 0; r < level.rowCount + extraRows; r++)
        {
            for (int c = 0; c < level.columnCount; c++)
            {
                if (tilesAndObstacles[r, c] != null)
                    continue; // already occupied by an obstacle

                GameObject tileObj = CreateTile(r, c);
                PlaceIPoolableInLevel(tileObj.GetComponent<Tile>());
                tilesAndObstacles[r, c] = tileObj.GetComponent<Tile>();
            }
        }
    }

    public GameObject CreateTile(int row, int column, TileType? tileType = null, TilePower tilePower = TilePower.Normal)
    {
        GameObject tileObj = ObjectPoolManager.SpawnObject(levelManager.tilePrefab, tilesContainer);
        Tile tile = tileObj.GetComponent<Tile>();
        tile.row = row;
        tile.column = column;
        tile.power = tilePower;

        // If tileType is not specified, pick a random allowed type
        TileType finalType = tileType ?? level.tileTypesAllowed[Range(0, level.tileTypesAllowed.Length)];
        Sprite sprite;
        if (tilePower != TilePower.Normal)
        {
            sprite = spriteManager.GetRandomSprite(finalType, true);
        }
        else
        {
            sprite = spriteManager.GetRandomSprite(finalType);
        }

        tile.sprite = sprite;
        tile.type = finalType;

        tileObj.GetComponent<Image>().sprite = sprite;
        return tileObj;
    }

    public GameObject CreateObstacle(int row, int column, int maxHP)
    {
        GameObject obstacleObj = ObjectPoolManager.SpawnObject(levelManager.obstaclePrefab, tilesContainer);
        Obstacle obstacle = obstacleObj.GetComponent<Obstacle>();
        obstacle.row = row;
        obstacle.column = column;
        ObstacleType obstacleType = level.obstacleTypesAllowed[Range(0, level.obstacleTypesAllowed.Length)];
        Sprite sprite = spriteManager.GetRandomSprite(obstacleType);
        obstacle.sprite = sprite;
        obstacle.type = obstacleType;
        obstacle.maxHP = maxHP;

        obstacleObj.GetComponent<Image>().sprite = sprite;
        return obstacleObj;
    }

    public void PlaceIPoolableInLevel(IPoolable obj)
    {
        if (obj == null) return;

        var comp = obj as Component;
        if (comp == null) return;

        var rt = comp.GetComponent<RectTransform>();
        if (rt == null) return;

        // Parent under the tiles container
        rt.SetParent(tilesContainer, worldPositionStays: false);

        // Ensure expected anchors/pivot for "top-origin" math
        rt.anchorMin = new Vector2(0.5f, 1f);   // Top-Center
        rt.anchorMax = new Vector2(0.5f, 1f);   // Top-Center
        rt.pivot = new Vector2(0.5f, 0.5f); // Center pivot is fine for centering in a cell

        // Normalize size/scale
        rt.localScale = Vector3.one;
        rt.sizeDelta = new Vector2(level.cellSizeX, level.cellSizeY);

        float cw = level.cellSizeX;
        float ch = level.cellSizeY;

        // Make the white background smaller than the cell,
        // and the icon slightly larger (or equal) than the background.
        if (comp.TryGetComponent<Background>(out _))
        {
            rt.sizeDelta = new Vector2(cw * levelManager.backgroundFill,
                                       ch * levelManager.backgroundFill);
        }
        else if (comp.TryGetComponent<Tile>(out _)
              || comp.TryGetComponent<Obstacle>(out _))
        {
            rt.sizeDelta = new Vector2(cw * levelManager.iconFill,
                                       ch * levelManager.iconFill);
        }

        // (anchors/pivot/top-origin position stay the same as you already set)

        // Grid step sizes
        float stepX = level.cellSizeX + level.spacingX;
        float stepY = level.cellSizeY + level.spacingY;

        // Center grid horizontally within the tiles container
        int cols = level.columnCount;
        float totalGridWidth = cols * level.cellSizeX + (cols - 1) * level.spacingX;

        // Compute anchored position (top-origin: down is negative Y)
        float x = -totalGridWidth / 2f + (level.cellSizeX / 2f) + obj.column * stepX;
        float y = -(level.cellSizeY / 2f) - obj.row * stepY;

        rt.anchoredPosition3D = new Vector3(x, y, 0f);
    }

    #region Layout Containers Configuration

    /// <summary>
    /// Fits Match3_Play_Area to the designed board size while staying centered inside
    /// Match3_Max_Play_Area, and then makes Match3_Board_Tiles taller by extraRows only
    /// on the TOP side (so bottoms line up).
    /// </summary>
    public void ConfigureLayoutContainers()
    {
        if (level == null || playArea == null || tilesContainer == null) return;

        RectTransform maxPlayArea = playArea.parent as RectTransform; // Match3_Max_Play_Area

        // ----- base (unscaled) board geometry -----
        float boardWidth = level.columnCount * baseCellSizeX + Mathf.Max(0, level.columnCount - 1) * baseSpacingX;
        float boardHeight = level.rowCount * baseCellSizeY + Mathf.Max(0, level.rowCount - 1) * baseSpacingY;

        // ----- available area (optionally shrink by margin) -----
        float areaW = Mathf.Max(0, maxPlayArea.rect.width - 2f * levelManager.boardMarginPx);
        float areaH = Mathf.Max(0, maxPlayArea.rect.height - 2f * levelManager.boardMarginPx);

        // Scale that would fill the area
        float fillScale = Mathf.Min(areaW / boardWidth, areaH / boardHeight);

        // What cell size would that produce?
        float cellSizeIfFilled = baseCellSizeX * fillScale; // assume square cells

        // Clamp cell size so boards with fewer rows/cols don't become enormous
        float clampedCellSize = Mathf.Clamp(cellSizeIfFilled,
                                            levelManager.minCellSizePx,
                                            levelManager.maxCellSizePx);

        // Final scale derived from the clamped cell size
        float scale = clampedCellSize / baseCellSizeX;

        // ----- apply scaled metrics (idempotent from base) -----
        level.cellSizeX = baseCellSizeX * scale;
        level.cellSizeY = baseCellSizeY * scale;
        level.spacingX = baseSpacingX * scale;
        level.spacingY = baseSpacingY * scale;

        float playW = boardWidth * scale;
        float playH = boardHeight * scale;

        // center the play area inside Max_Play_Area
        playArea.anchorMin = playArea.anchorMax = new Vector2(0.5f, 0.5f);
        playArea.pivot = new Vector2(0.5f, 0.5f);
        playArea.sizeDelta = new Vector2(playW, playH);
        playArea.anchoredPosition = Vector2.zero;

        // extend the tiles container upward by extra rows (using scaled metrics)
        float extraH = levelManager.extraRows * level.cellSizeY
                     + (levelManager.extraRows > 0 ? levelManager.extraRows * level.spacingY : 0f);

        float boardTilesW = playW;
        float boardTilesH = playH + extraH;

        tilesContainer.anchorMin = tilesContainer.anchorMax = new Vector2(0.5f, 0.5f);
        tilesContainer.pivot = new Vector2(0.5f, 0.5f);
        tilesContainer.sizeDelta = new Vector2(boardTilesW, boardTilesH);
        tilesContainer.anchoredPosition = new Vector2(0f, extraH * 0.5f);
    }


    #endregion

}
