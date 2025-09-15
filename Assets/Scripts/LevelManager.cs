using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public GameObject obstaclePrefab;
    public GameObject tilePrefab;

    public RectTransform backgroundCanvas; // backgroundGrid is on this canvas
    public GridLayoutGroup backgroundGrid; // Board backgrounds and obstacles are children of this grid
    public RectTransform tilesCanvas;      // tilesContainer is on this canvas
    public GameObject tilesContainer;      // match3 tiles are children of this container

    public LevelBuildData currentLevelData;
    public Level currentLevel;
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
        DebugTilesArray();
    }



    public void CreateLevel(LevelBuildData levelData)
    {
        ClearGrid(backgroundGrid);
        currentLevel = new LevelFactory(this)
            .CreateLevel(levelData);
    }
    private void ClearGrid(GridLayoutGroup grid)
    {
        var toReturn = new List<GameObject>();
        foreach (Transform child in grid.transform)
        {
            toReturn.Add(child.gameObject);
        }
        for (int i = 0; i < toReturn.Count; i++)
        {
            toReturn[i].GetComponent<IPoolable>().ResetForPool();
            ObjectPoolManager.ReturnObjectToPool(toReturn[i]);
        }
    }

    public Vector3 GetCellPosition(int row, int column)
    {
        Transform cellTransform = backgroundGrid.transform.GetChild(row * currentLevel.columnCount + column);



        return cellTransform.position;
    }

    public void DebugTilesArray()
    {
        foreach (var tile in currentLevel.tiles)
        {
            if (tile != null)
                Debug.Log($"Tile at ({tile.GetComponent<Tile>().row}, {tile.GetComponent<Tile>().column}): {tile.GetComponent<Tile>().type}");
            else
                Debug.Log("Null tile found");
        }
    }
}