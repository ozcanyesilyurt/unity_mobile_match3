using System.Collections.Generic;
using DG.Tweening;
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
    public Vector3 GetAnchoredPositionFor(int row, int column)
    {
        if (currentLevel == null) return Vector3.zero;

        float stepX = currentLevel.cellSizeX + currentLevel.spacingX;
        float stepY = currentLevel.cellSizeY + currentLevel.spacingY;

        int cols = currentLevel.columnCount;
        float totalGridWidth = cols * currentLevel.cellSizeX + (cols - 1) * currentLevel.spacingX;

        float x = -totalGridWidth / 2f + (currentLevel.cellSizeX / 2f) + column * stepX;
        float y = -(currentLevel.cellSizeY / 2f) - row * stepY;

        return new Vector3(x, y, 0f);
    }
    public void TrySwap(Tile a, Tile b)    {
    if (a == null || b == null) return;

    // adjacency
    int dr = Mathf.Abs(a.row - b.row);
    int dc = Mathf.Abs(a.column - b.column);
    if (!((dr == 1 && dc == 0) || (dr == 0 && dc == 1))) return;

    // store originals
    int aRowOrig = a.row, aColOrig = a.column;
    int bRowOrig = b.row, bColOrig = b.column;

    // DEBUG: dump the pair + surrounding row/col so you can see what's actually on the board
    Debug.Log($"TrySwap: swapping A({aRowOrig},{aColOrig}) type={a.type}  <->  B({bRowOrig},{bColOrig}) type={b.type}");
    if (tilesAndObstacles != null)
    {
        Tile tAonBoard = tilesAndObstacles[aRowOrig, aColOrig] as Tile;
        Tile tBonBoard = tilesAndObstacles[bRowOrig, bColOrig] as Tile;
        Debug.Log($"Board before swap: at A => { (tAonBoard!=null ? tAonBoard.type.ToString() : "null") }, at B => { (tBonBoard!=null ? tBonBoard.type.ToString() : "null") }");
    }

    // swap in the board array
    tilesAndObstacles[aRowOrig, aColOrig] = b;
    tilesAndObstacles[bRowOrig, bColOrig] = a;

    // update logical coords (so FindMatches reads the swapped state after animation)
    a.NewCoordinates(bRowOrig, bColOrig);
    b.NewCoordinates(aRowOrig, aColOrig);

    // DEBUG: verify board after swap
    if (tilesAndObstacles != null)
    {
        Tile tAonBoard = tilesAndObstacles[bRowOrig, bColOrig] as Tile; // should be 'a' now
        Tile tBonBoard = tilesAndObstacles[aRowOrig, aColOrig] as Tile; // should be 'b' now
        Debug.Log($"Board after swap: at new Apos({bRowOrig},{bColOrig}) => { (tAonBoard!=null ? tAonBoard.type.ToString() : "null") }, at new Bpos({aRowOrig},{aColOrig}) => { (tBonBoard!=null ? tBonBoard.type.ToString() : "null") }");
    }

    Tween ta = a.Move(bRowOrig, bColOrig);
    Tween tb = b.Move(aRowOrig, aColOrig);

    Sequence seq = DOTween.Sequence();
    seq.Join(ta);
    seq.Join(tb);
    seq.OnComplete(() =>
    {
        // DEBUG: right before TryMatch, dump the two tiles
        Debug.Log($"TrySwap.OnComplete: A now at ({a.row},{a.column}) type={a.type}  B now at ({b.row},{b.column}) type={b.type}");
        TryMatch(a, b, aRowOrig, aColOrig, bRowOrig, bColOrig);
    });
}


    // TryMatch called after swap animation; this overload receives originals so it can CancelSwap
    private void TryMatch(Tile a, Tile b, int aRowOrig, int aColOrig, int bRowOrig, int bColOrig)
    {
        // detect matches on the current board (swapped state)
        FindMatches(includeExtraRows: false);

        if (_matchedTiles.Count > 0)
        {
            // we have matches: resolve them (award score + cascades)
            MakeMatch(awardScore: true);
            // optionally notify listeners:
            Match3Events.OnSwapSuccess?.Invoke();
        }
        else
        {
            // no match: revert the swap visually and in board data
            CancelSwap(a, b, aRowOrig, aColOrig, bRowOrig, bColOrig);
            Match3Events.OnSwapCancel?.Invoke();
        }
    }

    // keep original signature as a convenience (calls overload)
    public void TryMatch(Tile a, Tile b)
    {
        // fallback: call FindMatches and resolve — no revert possible here
        FindMatches(includeExtraRows: false);
        if (_matchedTiles.Count > 0) MakeMatch(awardScore: true);
    }

    // swap did not produce a match, swap back (overload with original coords)
    public void CancelSwap(Tile a, Tile b, int aRowOrig, int aColOrig, int bRowOrig, int bColOrig)
    {
        if (a == null || b == null) return;

        // Place objects back in the board array at their original positions
        tilesAndObstacles[aRowOrig, aColOrig] = a;
        tilesAndObstacles[bRowOrig, bColOrig] = b;

        // animate tiles back to their original positions and update their logical coords
        a.NewCoordinates(aRowOrig, aColOrig);
        b.NewCoordinates(bRowOrig, bColOrig);

        Tween ta = a.Move(aRowOrig, aColOrig);
        Tween tb = b.Move(bRowOrig, bColOrig);

        Sequence seq = DOTween.Sequence();
        seq.Join(ta);
        seq.Join(tb);
        // optional: on complete, you can allow player input again or fire an event
    }

    // keep original CancelSwap signature (no-op wrapper)
    public void CancelSwap(Tile a, Tile b)
    {
        // no original positions known — just log
        Debug.LogWarning("CancelSwap called without original positions. No action taken.");
    }

    public void RequestSwap(Tile fromTile, Vector2Int dir)
    {
        Debug.Log($"RequestSwap from ({fromTile.row},{fromTile.column}) dir={dir}");
        if (fromTile == null) return;

        // NOTE: screen-space Y and board-row Y have opposite sign.
        // Screen: up = +Y, down = -Y. Board rows increase downward.
        // So convert by subtracting dir.y when computing the neighbor row.
        int neighborRow = fromTile.row - dir.y;
        int neighborCol = fromTile.column + dir.x;

        Tile neighbor = GetTileAt(neighborRow, neighborCol);
        if (neighbor == null)
        {
            Debug.Log("RequestSwap: neighbor not valid (out of bounds or not a tile).");
            return;
        }

        // forward to TrySwap -> animation -> TryMatch/CancelSwap flow
        TrySwap(fromTile, neighbor);
        Match3Events.OnTrySwap?.Invoke();
    }
}