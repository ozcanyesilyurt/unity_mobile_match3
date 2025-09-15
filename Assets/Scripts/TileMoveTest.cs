using UnityEngine;
using System.Collections.Generic;

public class TileMoveTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LevelManager levelManager;
    
    [Header("Settings")]
    [SerializeField] private float moveInterval = 5f;
    [SerializeField] private bool randomizeMoves = true;
    
    [Header("Runtime Properties")]
    [SerializeField] private Tile currentTile;
    [SerializeField] private int targetRow;
    [SerializeField] private int targetColumn;
    
    private float timer;
    private List<Tile> availableTiles = new List<Tile>();
    
    private void Start()
    {
        if (levelManager == null)
            levelManager = LevelManager.Instance;
            
        // Wait a moment for level to initialize
        Invoke("CollectAvailableTiles", 1f);
    }
    
    private void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= moveInterval && availableTiles.Count > 0)
        {
            timer = 0f;
            PerformRandomMove();
        }
    }
    
    private void CollectAvailableTiles()
    {
        availableTiles.Clear();
        
        if (levelManager.currentLevel == null || levelManager.currentLevel.tiles == null)
        {
            Debug.LogError("Level not initialized or no tiles available");
            return;
        }
        
        foreach (var tileObj in levelManager.currentLevel.tiles)
        {
            if (tileObj != null)
            {
                Tile tile = tileObj.GetComponent<Tile>();
                if (tile != null)
                {
                    availableTiles.Add(tile);
                }
            }
        }
        
        Debug.Log($"Found {availableTiles.Count} available tiles");
    }
    
    private void PerformRandomMove()
    {
        if (availableTiles.Count == 0) return;
        
        // Select a random tile
        currentTile = availableTiles[Random.Range(0, availableTiles.Count)];
        
        // Generate a valid target position
        if (randomizeMoves)
        {
            targetRow = Random.Range(0, levelManager.currentLevel.rowCount);
            targetColumn = Random.Range(0, levelManager.currentLevel.columnCount);
        }
        else
        {
            // Simple movement: move one step in a random direction
            targetRow = currentTile.row;
            targetColumn = currentTile.column;
            
            int direction = Random.Range(0, 4);
            switch (direction)
            {
                case 0: targetRow += 1; break;    // Down
                case 1: targetRow -= 1; break;    // Up
                case 2: targetColumn += 1; break; // Right
                case 3: targetColumn -= 1; break; // Left
            }
            
            // Ensure we stay within boundaries
            targetRow = Mathf.Clamp(targetRow, 0, levelManager.currentLevel.rowCount - 1);
            targetColumn = Mathf.Clamp(targetColumn, 0, levelManager.currentLevel.columnCount - 1);
        }
        
        // Perform the move
        Debug.Log($"Moving tile from ({currentTile.row}, {currentTile.column}) to ({targetRow}, {targetColumn})");
        currentTile.Move(targetRow, targetColumn);
    }
    
    // Optional: You can call this when the level changes to refresh tile list
    public void RefreshTiles()
    {
        CollectAvailableTiles();
    }
}