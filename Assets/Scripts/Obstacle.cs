using UnityEngine;
using UnityEngine.UI;
using static Match3Enums;

public class Obstacle : MonoBehaviour, IPoolable
{
    public int row;
    public int column;
    public float maxHP;
    public float hp;
    public Sprite sprite;
    public ObstacleType type;
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