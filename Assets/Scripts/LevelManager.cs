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
    private int _lockCount = 0;
    public bool IsLocked => _lockCount > 0;
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
            FindMatches(!awardScore); // during level creation (awardScore == false) include extra rows

            if (_matchedTiles.Count == 0)
                break;

            if (awardScore)
            {
                float stepScore = ScoreTiles();
                foreach (var tile in _matchedTiles)// debug log each matched tile
                {
                    if (tile != null)
                    {
                        Debug.Log($"Matched Tile at ({tile.row},{tile.column}) Type={tile.type} ScoreValue={tile.scoreValue}");
                    }
                }
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
            // During level creation (awardScore == false) keep using FillEmptyTiles
            // During gameplay cascades (awardScore == true) use gravity-based FallDownTiles
            if (awardScore == false)
            {
                FillEmptyTiles(matchedInfos);
            }
            else
            {
                FallDownTiles();
                return;
            }
        }

        Debug.Log("MakeMatch: no more cascades.");
        if (awardScore)
        {
            UnlockInteraction();
        }

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



    // TryMatch called after swap animation; this overload receives originals so it can CancelSwap



    // swap did not produce a match, swap back (overload with original coords)


    public void RequestSwap(Tile fromTile, Vector2Int dir)
    {
        if (IsLocked) return;
        LockInteraction();

        Debug.Log($"RequestSwap from ({fromTile.row},{fromTile.column}) dir={dir}");
        Tile toTile = GetTileAt(fromTile.row + dir.y, fromTile.column + dir.x);
        if (toTile == null)
        {
            Debug.Log("RequestSwap: target tile is null or out of bounds.");
            return;
        }
        TrySwap(fromTile, toTile);

    }
    public void TrySwap(Tile a, Tile b)
    {
        tilesAndObstacles[a.row, a.column] = b;
        tilesAndObstacles[b.row, b.column] = a;

        // Store original coords in case we need to swap back
        int aRow = a.row, aCol = a.column;
        int bRow = b.row, bCol = b.column;

        // Swap visually (start both tweens and join them)
        var tweenA = a.Move(bRow, bCol);
        var tweenB = b.Move(aRow, aCol);
        DOTween.Sequence()
            .Join(tweenA)
            .Join(tweenB)
            .OnComplete(() => TryMatch(a, b, aRow, aCol, bRow, bCol));
    }
    private void TryMatch(Tile a, Tile b, int aRow, int aCol, int bRow, int bCol)
    {
        FindMatches();
        if (_matchedTiles.Count == 0)
        {
            Debug.Log("TryMatch: no match, cancelling swap.");
            CancelSwap(a, b, aRow, aCol, bRow, bCol);
        }
        else
        {
            Debug.Log($"TryMatch: match found with {_matchedTiles.Count} tiles, resolving.");
            MakeMatch(awardScore: true);
        }
    }
    public void CancelSwap(Tile a, Tile b, int aRow, int aCol, int bRow, int bCol)
    {
        // Restore logical positions
        tilesAndObstacles[aRow, aCol] = a;
        tilesAndObstacles[bRow, bCol] = b;

        // Swap visually back (start both tweens and join them)
        var tweenA = a.Move(aRow, aCol);
        var tweenB = b.Move(bRow, bCol);
        DOTween.Sequence()
            .Join(tweenA)
            .Join(tweenB)
            .OnComplete(() =>
            {
                Debug.Log("CancelSwap: swap back complete.");
                Debug.Log("CancelSwap: swap back complete.");
                UnlockInteraction();
            });
    }
    private void FallDownTiles(bool delayBeforeMatch = true)
    {
        int rows = currentLevel.rowCount + extraRows;
        int cols = currentLevel.columnCount;
        var moveTweens = new List<Tween>();
        bool anyMoved = false;

        // pack / pull-down pass (your existing logic)
        for (int c = 0; c < cols; c++)
        {
            for (int r = rows - 1; r >= 0; r--)
            {
                if (tilesAndObstacles[r, c] != null) continue;

                for (int rr = r - 1; rr >= 0; rr--)
                {
                    var obj = tilesAndObstacles[rr, c];
                    if (obj == null) continue;

                    tilesAndObstacles[r, c] = obj;
                    tilesAndObstacles[rr, c] = null;

                    if (obj is Tile t)
                    {
                        var tw = t.Move(r, c);
                        if (tw != null) moveTweens.Add(tw);
                    }
                    else if (obj is Obstacle o)
                    {
                        var tw = o.Move(r, c);
                        if (tw != null) moveTweens.Add(tw);
                    }
                    anyMoved = true;
                    break;
                }
            }
        }

        float postFallDelay = 0.5f; // only used at the very end

        void FinishOrRepeat()
        {
            // fill only the top buffer
            FillExtraRowsWithNewTiles();

            // if ANY null remains anywhere (or just in visible rows if you prefer), repeat immediately
            if (HasEmptyCells())
            {
                // keep repeating WITHOUT adding delay each time
                FallDownTiles(delayBeforeMatch);
                return;
            }

            // settled -> now (and only now) add the nice pause before scoring/cascading
            if (delayBeforeMatch)
            {
                DOTween.Sequence()
                       .AppendInterval(postFallDelay)
                       .OnComplete(() => MakeMatch(awardScore: true));
            }
            else
            {
                MakeMatch(awardScore: true);
            }
        }

        if (anyMoved && moveTweens.Count > 0)
        {
            var seq = DOTween.Sequence();
            foreach (var tw in moveTweens) seq.Join(tw);
            seq.OnComplete(FinishOrRepeat); // no interval here
        }
        else if (anyMoved)
        {
            // moved but no tweens returned
            FinishOrRepeat();
        }
        else
        {
            // nothing moved, still try to top-fill and loop
            FinishOrRepeat();
        }
    }

    private bool HasEmptyCells()
    {
        int rows = currentLevel.rowCount + extraRows;
        int cols = currentLevel.columnCount;
        for (int r = 0; r < rows; r++)
            for (int c = 0; c < cols; c++)
                if (tilesAndObstacles[r, c] == null) return true;
        return false;
    }

    // add helper coroutine near class bottom
    private System.Collections.IEnumerator WaitThenMatchRealtime(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Debug.Log("FallDownTiles: fallback realtime delay complete, checking for matches.");
        MakeMatch(awardScore: true);
    }

    private void FillExtraRowsWithNewTiles()
    {
        if (currentLevel == null || levelFactory == null || tilesAndObstacles == null) return;
        int cols = currentLevel.columnCount;
        for (int r = 0; r < extraRows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                if (tilesAndObstacles[r, c] == null)
                {
                    // Build allowed types and pick a random one
                    var allowedTypes = new List<TileType>(currentLevel.tileTypesAllowed);
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

                    Debug.Log($"FillExtraRowsWithNewTiles: Creating tile at ({r},{c}) of type {chosenType}");
                    GameObject newTileObj = levelFactory.CreateTile(r, c, chosenType);
                    levelFactory.PlaceIPoolableInLevel(newTileObj.GetComponent<IPoolable>());
                    tilesAndObstacles[r, c] = newTileObj.GetComponent<Tile>();
                }
            }
        }
    }
    public void LockInteraction()
    {
        _lockCount++;
        SetRaycastsEnabled(false);
    }
    public void UnlockInteraction()
    {
        _lockCount = Mathf.Max(0, _lockCount - 1);
        if (_lockCount == 0) SetRaycastsEnabled(true);
    }
    private void SetRaycastsEnabled(bool enabled)
    {
        if (tilesContainer == null) return;
        var cg = tilesContainer.GetComponent<CanvasGroup>();
        if (!cg) cg = tilesContainer.gameObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = enabled;
        cg.interactable = enabled; // optional, if you use Selectables
    }
}