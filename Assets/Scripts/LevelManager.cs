using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Match3Enums;

public class LevelManager : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public GameObject obstaclePrefab;
    public GameObject tilePrefab;
    public RectTransform tilesCanvas;           // tilesContainer and playArea are on this canvas
    public RectTransform tilesContainer;        // match3 tiles and obstacles are children of this container
    public RectTransform playArea;              // area where tiles are visible and interactable

    public int extraRows = 2;

    public LevelBuildData currentLevelData;
    public Level currentLevel;
    public LevelFactory levelFactory;


    private IPoolable[,] tilesAndObstacles;// holds all tiles and obstacles in the level
    private readonly HashSet<Tile> _matchedTiles = new HashSet<Tile>();
    public IReadOnlyCollection<Tile> MatchedTiles => _matchedTiles;
    public static LevelManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        CreateLevel(currentLevelData);
        LevelStart();
    }
    public void LevelStart()
    {
        MakeMatch(awardScore: false);
    }



    public void CreateLevel(LevelBuildData levelData)
    {
        levelFactory = new LevelFactory(this);
        currentLevel = levelFactory.CreateLevel(levelData);
        this.tilesAndObstacles = levelFactory.tilesAndObstacles;
    }

    public void MakeMatch(bool awardScore = true)
    {
        // Repeat until no new matches are produced by fills (handles cascades)
        while (true)
        {
            FindMatches(true);

            if (_matchedTiles.Count == 0)
                break;

            if (awardScore)
            {
                float stepScore = ScoreTiles();
                Debug.Log($"MakeMatch: matched {_matchedTiles.Count} tiles, score +{stepScore}");
                // fire events here if desired (only when awardScore == true)
            }
            else
            {
                Debug.Log($"MakeMatch: resolving {_matchedTiles.Count} initial matched tiles (no score)");
            }

            var matchedInfos = new List<(int row, int column, TileType prevType)>();
            foreach (var tile in _matchedTiles)
            {
                if (tile == null) continue;
                matchedInfos.Add((tile.row, tile.column, tile.type));
            }

            RemoveMatchedTiles();
            FillEmptyTiles(matchedInfos);
        }

        Debug.Log("MakeMatch: no more cascades.");
    }

    public void FindMatches(bool includeExtraRows = false)
    {
        if (currentLevel == null || tilesContainer == null || tilesAndObstacles == null)
        {
            Debug.LogWarning("FindMatches: Level, tilesContainer or board is null.");
            return;
        }
        _matchedTiles.Clear();
        int rowsStart;
        if (includeExtraRows == false)
        {
            rowsStart = extraRows;
        }
        else
        {
            rowsStart = 0;
        }
        int rowsEnd = extraRows + currentLevel.rowCount; // exclusive
        int cols = currentLevel.columnCount;
        // Horizontal runs
        for (int r = rowsStart; r < rowsEnd; r++)
        {
            int c = 0;
            while (c < cols)
            {
                Tile start = GetTileAt(r, c);
                if (start == null)
                {
                    c++;
                    continue;
                }

                int runLen = 1;
                int cc = c + 1;
                while (cc < cols)
                {
                    Tile next = GetTileAt(r, cc);
                    if (next == null || next.type != start.type) break;
                    runLen++;
                    cc++;
                }

                if (runLen >= 3)
                {
                    for (int k = 0; k < runLen; k++)
                    {
                        Tile t = GetTileAt(r, c + k);
                        if (t != null) _matchedTiles.Add(t);
                    }
                }

                c += Mathf.Max(runLen, 1);
            }
        }

        // Vertical runs
        for (int c = 0; c < cols; c++)
        {
            int r = rowsStart;
            while (r < rowsEnd)
            {
                Tile start = GetTileAt(r, c);
                if (start == null)
                {
                    r++;
                    continue;
                }

                int runLen = 1;
                int rr = r + 1;
                while (rr < rowsEnd)
                {
                    Tile next = GetTileAt(rr, c);
                    if (next == null || next.type != start.type) break;
                    runLen++;
                    rr++;
                }

                if (runLen >= 3)
                {
                    for (int k = 0; k < runLen; k++)
                    {
                        Tile t = GetTileAt(r + k, c);
                        if (t != null) _matchedTiles.Add(t);
                    }
                }

                r += Mathf.Max(runLen, 1);
            }
        }

        Debug.Log(_matchedTiles.Count > 0
            ? $"FindMatches: found {_matchedTiles.Count} matched tiles."
            : "FindMatches: no matches.");
    }

    public void RemoveMatchedTiles()
    {
        foreach (var tile in _matchedTiles)
        {
            if (tile != null)
            {
                tilesAndObstacles[tile.row, tile.column] = null;
                ObjectPoolManager.ReturnObjectToPool(tile.gameObject);
            }
        }

    }
    // ...existing code...

    public float ScoreTiles()
    {
        float totalScore = 0;
        foreach (var tile in _matchedTiles)
        {
            if (tile != null)
            {
                totalScore += tile.scoreValue;
            }
        }
        return totalScore;
    }

    public Tile GetTileAt(int row, int col)
    {
        if (tilesAndObstacles == null) return null;
        if (row < 0 || col < 0 ||
            row >= tilesAndObstacles.GetLength(0) ||
            col >= tilesAndObstacles.GetLength(1)) return null;

        return tilesAndObstacles[row, col] as Tile; // obstacles/backgrounds => null
    }

    public void FillEmptyTiles(List<(int row, int column, TileType prevType)> positions, bool onlyExtraRows = false)
    {
        if (positions == null || positions.Count == 0) return;
        foreach (var info in positions)
        {
            int row = info.row;
            int col = info.column;
            if (onlyExtraRows && row >= extraRows) continue;

            // Build allowed types and exclude the replaced tile's type
            var allowedTypes = new List<TileType>(currentLevel.tileTypesAllowed);
            allowedTypes.Remove(info.prevType);

            // Fallback if removing left us with no options
            TileType chosenType;
            if (allowedTypes.Count == 0)
            {
                var fallback = currentLevel.tileTypesAllowed;
                chosenType = fallback[Random.Range(0, fallback.Length)];
            }
            else
            {
                chosenType = allowedTypes[Random.Range(0, allowedTypes.Count)];
            }

            Debug.Log($"FillEmptyTiles: Filling empty tile at ({row}, {col}) with {chosenType}");
            GameObject newTileObj = levelFactory.CreateTile(row, col, chosenType);
            tilesAndObstacles[row, col] = newTileObj.GetComponent<Tile>();
        }
    }
    public void TrySwap(Tile a, Tile b) // player attempted to swap two tiles
    {

    }


    public void TryMatch(Tile a, Tile b) // Tryswap calls this after swap animation
    {

    }

    public void CancelSwap(Tile a, Tile b) // swap did not produce a match, swap back
    {

    }
    public void RequestSwap(Tile fromTile, Vector2Int dir)
    {
        Debug.Log($"RequestSwap from ({fromTile.row},{fromTile.column}) dir={dir}");
        // Step 2: we'll fetch the neighbor at (row+dir.y, col+dir.x),
        // validate, animate the swap, then call TrySwap(a,b) or CancelSwap(a,b).
    }
}