using UnityEngine;
using UnityEngine.UI;
using static Match3Enums;

public class Background : MonoBehaviour, IPoolable
{
    public int row;
    public int column;
    public Sprite sprite;
    public BackgroundType type;

    public void ResetForPool()
    {
        row = -1;
        column = -1;
        sprite = null;
        GetComponent<Image>().sprite = null;
        StopAllCoroutines();
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
    }
}