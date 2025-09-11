using UnityEngine;
using UnityEngine.UI;
using static Match3Enums;

public class Tile : MonoBehaviour, IPoolable
{
    public int row;
    public int column;
    public TilePower power;
    public TileType type;
    public Sprite sprite;

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
    }
}