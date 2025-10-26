using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestText;

    private float lastShownBest = -1f;

    private void Start()
    {
        RefreshAll();
    }

    private void Update()
    {
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.state == GameManager.GameState.Playing && scoreText != null)
        {
            scoreText.text = FormatSeconds(GameManager.Instance.score);
        }
        if (bestText !=null && Mathf.Abs(GameManager.Instance.bestScore - lastShownBest) > 0.0001f)
        {
            bestText.text = FormatSeconds(GameManager.Instance.bestScore);
            lastShownBest = GameManager.Instance.bestScore;
        }
    }

    private void RefreshAll()
    { 
    }
    private string FormatSeconds(float s)
    {
        // show 0.0s style
        return s.ToString("0.0") + "s";
    }

}
