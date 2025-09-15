using UnityEngine;
using UnityEngine.UI;
using static Match3Enums;
using DG.Tweening;

public class Tile : MonoBehaviour, IPoolable
{
    public int row;
    public int column;
    public TilePower power;
    public TileType type;
    public Sprite sprite;
    public float speed = 0.2f; //speed to move tile one row or column

    public void ResetForPool()
    {
        row = -1;
        column = -1;
        power = TilePower.Normal;
        type = TileType.None;
        sprite = null;
        GetComponent<Image>().sprite = null;
        StopAllCoroutines();
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        transform.DOKill();
    }

    public void Move(int newRow, int newColumn)//tween to new position
    {
        NewCoordinates(newRow, newColumn);

        // Calculate new position using the same logic as TileStartPosition
        GridLayoutGroup backgroundGrid = LevelManager.Instance.backgroundGrid;

        float cellWidth = backgroundGrid.cellSize.x;
        float cellHeight = backgroundGrid.cellSize.y;
        float spacingX = backgroundGrid.spacing.x;
        float spacingY = backgroundGrid.spacing.y;

        int columns = backgroundGrid.constraintCount;
        int rows = backgroundGrid.transform.childCount / columns;

        float totalWidth = (columns * cellWidth) + ((columns - 1) * spacingX);
        float totalHeight = (rows * cellHeight) + ((rows - 1) * spacingY);

        // Calculate target position
        float targetX = (newColumn * (cellWidth + spacingX)) - (totalWidth * 0.5f) + (cellWidth * 0.5f);
        float targetY = (totalHeight * 0.5f) - (newRow * (cellHeight + spacingY)) - (cellHeight * 0.5f);

        Vector3 targetPosition = new Vector3(targetX, targetY, 0f);

        // Kill any existing tween and animate to new position
        transform.DOKill();
        transform.DOLocalMove(targetPosition, speed).SetEase(Ease.OutQuad);
    }

    public void NewCoordinates(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

}