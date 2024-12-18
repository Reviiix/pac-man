using System;
using System.Collections;
using System.Collections.Generic;
using GridSystem.GridItems;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Movement.MovingObjects
{
    [Serializable]
    public class SeekerEnemyMovement : MovingObject
    {
        [SerializeField] private Pathfinder pathFinder;
        private Queue<GridItem> path;
        public GridItem target { get; private set; }
        public SeekerType type;
        
        private readonly MovementManager.Direction[] randomDirections = MovementManager.Directions;

        public enum SeekerType
        {
            Chase,
            CutOff,
            Random
        }

        protected override void OnDestinationReached()
        {
            base.OnDestinationReached();
            UpdateNext();
        }
        
        private void UpdateNext()
        {

            switch (type)
            {
                case SeekerType.Chase:
                    UpdatePath();
                    if (PathComplete(path)) return;
                    path.Dequeue(); //Current position
                    next = path.Dequeue();
                    currentMovementDirection = MovementManager.GetDirectionOfNeighbor(current, next);
                    break;
                case SeekerType.Random:
                    if (ShouldChangeDirection())
                    {
                        ChangeDirection(GetValidRandomDirection(currentMovementDirection));   
                    }
                    break;
            }
        }
        
        private bool ShouldChangeDirection()
        {
            return current.GetAdjacentItem(currentMovementDirection) is Wall || ShouldTakeAlternativeRoute();
        }
        
        private bool ShouldTakeAlternativeRoute()
        {
            return Random.Range(0, randomDirections.Length) == 0 && current.MultiplePathWaysAvailable();
        }
        
        private MovementManager.Direction GetValidRandomDirection(MovementManager.Direction currentDirection)
        {
            while (true)
            {
                var randomDirection = randomDirections[Random.Range(0, randomDirections.Length)];

                if (randomDirection == currentDirection) continue;
                
                if (current.GetAdjacentItem(randomDirection) is Wall) continue;

                return randomDirection;
            }
        }

        /// <summary>
        /// Only setting a new path on path junctions helps reduce the amount of calculations done and makes the seeker enemy less relentless.
        /// </summary>
        private void UpdatePath()
        {
            target = MovementManager.Instance.GetPlayerTransform();
            path  = pathFinder.FindPath(current, target);
        }

        private bool PathComplete(ICollection p)
        {
            return p is not { Count: > 1 };
        }
    }
}
