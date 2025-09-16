using UnityEngine;
using UnityEngine.UI;

public class Background : MonoBehaviour, IPoolable
{
    public int row;
    public int column;
    public Sprite sprite;

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