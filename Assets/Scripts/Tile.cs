using UnityEngine;
using UnityEngine.UI;
using static Match3Enums;
using DG.Tweening;

public class Tile : MonoBehaviour, IPoolable
{
    [field: SerializeField] public int row { get; set; }
    [field: SerializeField] public int column { get; set; }
    [field: SerializeField] public TilePower power { get; set; }
    [field: SerializeField] public TileType type { get; set; }
    [field: SerializeField] public Sprite sprite { get; set; }
    [field: SerializeField] public float speed { get; set; } = 0.2f; //speed to move tile one row or column

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