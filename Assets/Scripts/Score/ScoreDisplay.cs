using GameStates;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class ScoreDisplay : MonoBehaviour
{
    private TMP_Text display;
    private const string Prefix = "SCORE: ";

    private void Awake()
    {
        display = GetComponent<TMP_Text>();
        OnScoreChange(0);
    }

    private void OnEnable()
    {
        ScoreManager.OnScoreChange += OnScoreChange;
    }
    
    private void OnDisable()
    {
        ScoreManager.OnScoreChange -= OnScoreChange;
    }

    private void OnScoreChange(int score)
    {
        display.text = Prefix + score;
    }
}
