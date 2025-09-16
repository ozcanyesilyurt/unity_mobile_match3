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

    }

    public void NewCoordinates(int row, int column)
    {
        this.row = row;
        this.column = column;
    }

}