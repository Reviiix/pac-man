using System;
using GridSystem.GridItems;
using Random = UnityEngine.Random;

namespace Movement.MovingObjects
{
    [Serializable]
    public class RandomEnemyMovement : MovingObject
    {
        private readonly MovementManager.Direction[] directions = MovementManager.Directions;
        
        protected override void OnDestinationReached()
        {
            base.OnDestinationReached();
            if (ShouldChangeDirection())
            {
                ChangeDirection(GetValidRandomDirection(currentMovementDirection));   
            }
        }

        private bool ShouldChangeDirection()
        {
            return current.GetAdjacentItem(currentMovementDirection) is Wall || ShouldTakeAlternativeRoute();
        }

        private bool ShouldTakeAlternativeRoute()
        {
            return Random.Range(0, directions.Length) == 0 && current.MultiplePathWaysAvailable();
        }

        private MovementManager.Direction GetValidRandomDirection(MovementManager.Direction currentDirection)
        {
            while (true)
            {
                var randomDirection = directions[Random.Range(0, directions.Length)];

                if (randomDirection == currentDirection) continue;
                
                if (current.GetAdjacentItem(randomDirection) is Wall) continue;

                return randomDirection;
            }
        }
    }
}