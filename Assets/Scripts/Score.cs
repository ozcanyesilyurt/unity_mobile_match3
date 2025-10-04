using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreLabel;
    [SerializeField] private string prefix = "SCORE : ";

    public int CurrentScore { get; private set; }

    private void Awake()
    {
        if (!scoreLabel)
        {
            var t = transform.Find("Score");
            if (t) scoreLabel = t.GetComponent<TextMeshProUGUI>();
            if (!scoreLabel) scoreLabel = GetComponentInChildren<TextMeshProUGUI>(true);
        }
        SetScore(0);
    }

    private void OnEnable()
    {
        Match3Events.OnScoreAdded += HandleScoreAdded;   // changed
        Match3Events.OnScoreReset += HandleScoreReset;
    }

    private void OnDisable()
    {
        Match3Events.OnScoreAdded -= HandleScoreAdded;   // changed
        Match3Events.OnScoreReset -= HandleScoreReset;
    }

    private void HandleScoreAdded(int delta)
    {
        AddScore(delta);
    }

    private void HandleScoreReset()
    {
        SetScore(0);
    }

    private void AddScore(int delta)
    {
        SetScore(CurrentScore + delta);
    }

    private void SetScore(int value)
    {
        CurrentScore = Mathf.Max(0, value);
        if (scoreLabel) scoreLabel.text = $"{prefix}{CurrentScore}";
    }
}