using UnityEngine;

[DefaultExecutionOrder(-100)]
public class NormalizeCanvases : MonoBehaviour
{
    [SerializeField] Canvas[] canvases;
    void Awake()
    {
        foreach (var c in canvases)
            c.GetComponent<RectTransform>().localScale = Vector3.one;
    }
}
