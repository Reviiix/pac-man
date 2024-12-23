using System;
using Abstractions;
using GridSystem;
using UnityEngine;

namespace GameStates
{
    [ExecuteInEditMode]
    public class ScoreManager : Singleton<ScoreManager>
    {
        public static Action<int> OnScoreChange;
        private int totalPellets;
        private int score;
        private bool gameWon;
        [SerializeField] private Sprite[] bonusPickUpSprites;

        public void Initialise()
        {
            ResetPelletsAndPoints();
        }

        public void IncrementPelletsHit()
        {
            score++;
            gameWon = CheckGameWon();
            OnScoreChange?.Invoke(score);

            if (gameWon)
            {
                Debug.Log("You Won!");
            }
        }
        
        private void ResetPelletsAndPoints()
        {
            totalPellets = PelletManager.Instance.numberOfPellets;
            score = 0;
        }

        private bool CheckGameWon()
        {
            return score >= totalPellets;
        }
        
        public void BonusItemHit(BonusItem.BonusItemType type)
        {
            score += type switch
            {
                BonusItem.BonusItemType.Cherry => 10,
                BonusItem.BonusItemType.Strawberry => 20,
                BonusItem.BonusItemType.Orange => 30,
                BonusItem.BonusItemType.Apple => 40,
                BonusItem.BonusItemType.Melon => 50,
                BonusItem.BonusItemType.Galaxian => 60,
                BonusItem.BonusItemType.Bell => 70,
                BonusItem.BonusItemType.Key => 80,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, $"{type} not accounted for in switch statement.")
            };
            OnScoreChange?.Invoke(score);
        }
        
        public Sprite GetBonusPickUpGraphic(BonusItem.BonusItemType type)
        {
            return type switch
            {
                BonusItem.BonusItemType.Cherry => bonusPickUpSprites[0],
                BonusItem.BonusItemType.Strawberry => bonusPickUpSprites[1],
                BonusItem.BonusItemType.Orange => bonusPickUpSprites[2],
                BonusItem.BonusItemType.Apple => bonusPickUpSprites[3],
                BonusItem.BonusItemType.Melon => bonusPickUpSprites[4],
                BonusItem.BonusItemType.Galaxian => bonusPickUpSprites[5],
                BonusItem.BonusItemType.Bell => bonusPickUpSprites[6],
                BonusItem.BonusItemType.Key => bonusPickUpSprites[7],
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, $"{type} not accounted for in switch statement.")
            };
        }
    }
}