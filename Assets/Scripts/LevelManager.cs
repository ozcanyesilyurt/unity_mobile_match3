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
    public GridLayoutGroup tilesContainer;// match3 tiles are children of this container

    public LevelBuildData currentLevel;

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
        BuildLevel(new Level(currentLevel));

    }



    private void BuildLevel(Level level)
    {
        ClearGrid(backgroundGrid);
        LevelFactory levelFactory = new LevelFactory(this);
        levelFactory.CreateBGO(level);
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

    public Vector3 GetTilePosition(int row, int column)// get world position of tile based on row and column in backgroundGrid
    {
        float cellWidth = backgroundGrid.cellSize.x + backgroundGrid.spacing.x;
        float cellHeight = backgroundGrid.cellSize.y + backgroundGrid.spacing.y;

        float x = (column * cellWidth) + (cellWidth / 2) - (backgroundGrid.GetComponent<RectTransform>().rect.width / 2);
        float y = -(row * cellHeight) - (cellHeight / 2) + (backgroundGrid.GetComponent<RectTransform>().rect.height / 2);

        return new Vector3(x, y, 0f);
    }

}