using DG.Tweening;
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
    [field: SerializeField] public float speed { get; set; } = 0.2f;
    public void ResetForPool()
    {
        row = -10;
        column = -10;
        hp = maxHP;
        sprite = null;
        GetComponent<Image>().sprite = null;
        StopAllCoroutines();
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
    }
    public Tween Move(int newRow, int newColumn)
    {
        var rt = GetComponent<RectTransform>();
        if (rt == null || LevelManager.Instance == null || LevelManager.Instance.currentLevel == null)
        {
            // fallback: just set coordinates and return a completed tween
            NewCoordinates(newRow, newColumn);
            return DOVirtual.DelayedCall(0f, () => { });
        }

        // Compute target anchored position via LevelManager helper
        Vector3 target = LevelManager.Instance.GetAnchoredPositionFor(newRow, newColumn);

        // update logical coords immediately so other logic reads consistent state while animation runs
        NewCoordinates(newRow, newColumn);

        // animate anchored position (3D) — uses tile.speed as duration
        return rt.DOAnchorPos3D(target, speed).SetEase(Ease.InOutSine);
    }

    public void NewCoordinates(int row, int column)
    {
        this.row = row;
        this.column = column;
    }
}