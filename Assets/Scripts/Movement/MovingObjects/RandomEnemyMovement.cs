using System;
using GridSystem.GridItems;
using Random = UnityEngine.Random;

namespace Movement
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
                ChangeDirection(GetRandomDirection());   
            }
        }

        private bool ShouldChangeDirection()
        {
            return currentPosition.GetAdjacentItem(currentDirection) is Wall || ShouldTakeAlternativeRoute();
        }

        private bool ShouldTakeAlternativeRoute()
        {
            var randomChance = UnityEngine.Random.Range(0, directions.Length);
            return randomChance == 0 && currentPosition.MultiplePathWaysAvailable();
        }

        private MovementManager.Direction GetRandomDirection()
        {
            while (true)
            {
                var randomDirection = directions[Random.Range(0, directions.Length)];

                if (randomDirection == currentDirection) continue;
                
                if (randomDirection == MovementManager.GetOppositeDirection(currentDirection)) continue;

                if (currentPosition.GetAdjacentItem(randomDirection) is Wall) continue;
                return randomDirection;
            }
        }
    }
}