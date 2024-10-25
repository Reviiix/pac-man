using System;
using UnityEngine;

namespace Movement.MovingObjects
{
    [Serializable]
    public class PlayerMovement : MovingObject
    {
        public void CheckForInput()
        {
            if (Input.GetKeyUp(KeyCode.W))
            {
                DetermineDirectionChange(KeyCode.W);
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                DetermineDirectionChange(KeyCode.A);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                DetermineDirectionChange(KeyCode.S);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                DetermineDirectionChange(KeyCode.D);
            }
        }
    }
}