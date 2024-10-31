using Abstractions;
using GridSystem;
using UnityEngine;

namespace GameStates
{
    [ExecuteInEditMode]
    public class ScoreManager : Singleton<ScoreManager>
    {
        private int totalPellets;
        private int currentPelletsHit;

        private bool gameWon;
        
        public void Initialise()
        {
            ResetPelletsAndPoints();
        }

        public void IncrementPelletsHit()
        {
            currentPelletsHit++;
            gameWon = CheckGameWon();

            if (gameWon)
            {
                Debug.Log("You Won!");
            }
        }
        
        private void ResetPelletsAndPoints()
        {
            totalPellets = PelletManager.Instance.numberOfPellets;
            currentPelletsHit = 0;
        }

        private bool CheckGameWon()
        {
            return currentPelletsHit >= totalPellets;
        }
    }
}