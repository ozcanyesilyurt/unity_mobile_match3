using UnityEngine;
using UnityEngine.UI;
using static Match3Enums;

public class Background : MonoBehaviour, IPoolable
{
    [field: SerializeField] public int row { get; set; }
    [field: SerializeField] public int column { get; set; }
    [field: SerializeField] public Sprite sprite { get; set; }
    [field: SerializeField] public BackgroundType type { get; set; }

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