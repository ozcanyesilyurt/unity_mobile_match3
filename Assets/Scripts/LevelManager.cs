using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        FindMatches();
    }



    public void CreateLevel(LevelBuildData levelData)
    {
        levelFactory = new LevelFactory(this);
        currentLevel = levelFactory.CreateLevel(levelData);
        this.tilesAndObstacles = levelFactory.tilesAndObstacles;
    }

    public void MakeMatch()
    {
        Debug.Log("MakeMatch");
        // find matches
        // remove matched tiles
        // drop tiles above
        // fill empty spaces with new tiles
        // check for new matches
        // repeat if new matches found
        // if no new matches, wait for player input
    }
    public void FindMatches()
    {
        if (currentLevel == null || tilesContainer == null || tilesAndObstacles == null)
        {
            Debug.LogWarning("FindMatches: Level, tilesContainer or board is null.");
            return;
        }
        _matchedTiles.Clear();

        int rowsStart = extraRows;
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
        _matchedTiles.Clear();
    }
    
    private Tile GetTileAt(int row, int col)
    {
        if (tilesAndObstacles == null) return null;
        if (row < 0 || col < 0 ||
            row >= tilesAndObstacles.GetLength(0) ||
            col >= tilesAndObstacles.GetLength(1)) return null;

        return tilesAndObstacles[row, col] as Tile; // obstacles/backgrounds => null
    }
}