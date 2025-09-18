using UnityEngine;
using UnityEngine.UI;
using static Match3Enums;

public class Obstacle : MonoBehaviour, IPoolable
{
    [field: SerializeField] public int row { get; set; }
    [field: SerializeField] public int column { get; set; }
    [field: SerializeField] public float maxHP { get; set; }
    [field: SerializeField] public float hp { get; set; }
    [field: SerializeField] public Sprite sprite { get; set; }
    [field: SerializeField] public ObstacleType type { get; set; }
    public void ResetForPool()
    {
        row = -1;
        column = -1;
        hp = maxHP;
        sprite = null;
        GetComponent<Image>().sprite = null;
        StopAllCoroutines();
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
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