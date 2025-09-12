using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public GameObject obstaclePrefab;
    public GameObject tilePrefab;

    public RectTransform backgroundCanvas; // Reference to Match3_Canvas_BG
    public GridLayoutGroup backgroundGrid; // Reference to Match3_Board_BG
    public RectTransform tilesCanvas;      // Reference to match3_Canvas_Matches
    public GridLayoutGroup tilesGrid;      // Reference to Match3_Board_Matches


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
}