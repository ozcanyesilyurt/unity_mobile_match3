using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AspectRatioFitter))]
public class MiniGameAspectController : MonoBehaviour
{
    [SerializeField] private RawImage rawImage;
    private AspectRatioFitter aspectFitter;

    void Start()
    {
        aspectFitter = GetComponent<AspectRatioFitter>();
        UpdateAspectRatio();
    }

    void UpdateAspectRatio()
    {
        if (rawImage == null || rawImage.texture == null)
        {
            Debug.LogWarning("MiniGameAspectController: RawImage or texture is null");
            return;
        }

        float width = rawImage.texture.width;
        float height = rawImage.texture.height;
        float ratio = width / height;

        aspectFitter.aspectRatio = ratio;
        
        Debug.Log($"MiniGame aspect ratio set to {ratio} (texture: {width}x{height})");
    }

    // Call this if you change the render texture at runtime
    public void RefreshAspect()
    {
        UpdateAspectRatio();
    }
}