using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using GridSystem.GridItems;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Movement.MovingObjects
{
    [Serializable]
    public class SeekerEnemyMovement : MovingObject
    {
        [SerializeField] private Pathfinder pathFinder;
        private Queue<GridItem> path = new ();
        public GridItem Target { get; private set; }
        public SeekerType type;
        private readonly MovementManager.Direction[] randomDirections = MovementManager.Directions;

        public enum SeekerType
        {
            Chase,
            CutOff,
            Random,
            Scared
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
                case SeekerType.CutOff:
                case SeekerType.Scared:
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
                default:
                    throw new ArgumentOutOfRangeException();
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

        private GridItem GetScaredTarget()
        {
            const int distanceBeforeRetreat = 5;
            return path.Count <= distanceBeforeRetreat ? MovementManager.Instance.GetInversePlayerTransform() : MovementManager.Instance.GetPlayerTransform();
        }
        
        private GridItem GetCutOffTarget()
        {
            var cutoffTarget = MovementManager.Instance.GetPlayerTransform();
            var direction = MovementManager.Instance.GetPlayerDirection();
            var previousDirection = direction;
            var spacesAhead = 3;

            for (var i = 0; i < spacesAhead; i++)
            {
                if (cutoffTarget.GetAdjacentItem(direction) is Wall)
                {
                    // Setting direction priorities
                    if (cutoffTarget.GetAdjacentItem(MovementManager.Direction.Up) is not Wall && previousDirection != MovementManager.Direction.Down)
                    {
                        cutoffTarget = cutoffTarget.GetAdjacentItem(MovementManager.Direction.Up);
                        previousDirection = MovementManager.Direction.Up;
                    }
                    else if (cutoffTarget.GetAdjacentItem(MovementManager.Direction.Down) is not Wall && previousDirection != MovementManager.Direction.Up)
                    {
                        cutoffTarget = cutoffTarget.GetAdjacentItem(MovementManager.Direction.Down);
                        previousDirection = MovementManager.Direction.Down;
                    }
                    else if (cutoffTarget.GetAdjacentItem(MovementManager.Direction.Left) is not Wall && previousDirection != MovementManager.Direction.Right)
                    {
                        cutoffTarget = cutoffTarget.GetAdjacentItem(MovementManager.Direction.Left);
                        previousDirection = MovementManager.Direction.Left;
                    }
                    else if (cutoffTarget.GetAdjacentItem(MovementManager.Direction.Right) is not Wall && previousDirection != MovementManager.Direction.Left)
                    {
                        cutoffTarget = cutoffTarget.GetAdjacentItem(MovementManager.Direction.Right);
                        previousDirection = MovementManager.Direction.Right;
                    }
                }
                else
                {
                    cutoffTarget = cutoffTarget.GetAdjacentItem(direction);
                }
            }

            return cutoffTarget;
        }

        /// <summary>
        /// Only setting a new path on path junctions helps reduce the amount of calculations done and makes the seeker enemy less relentless.
        /// </summary>
        private void UpdatePath()
        {
            switch (type)
            {
                case SeekerType.Chase:
                    Target = MovementManager.Instance.GetPlayerTransform();
                    break;
                case SeekerType.CutOff:
                    Target = GetCutOffTarget();
                    if (ProjectSettings.DebugTools)
                    {
                        //MovementManager.Instance.targetDebug.position = target.transform.position;
                    }
                    break;
                case SeekerType.Scared:
                    Target = GetScaredTarget();
                    if (ProjectSettings.DebugTools)
                    {
                        MovementManager.Instance.targetDebug.position = Target.transform.position;
                    }
                    break;
            }
            
            path  = pathFinder.FindPath(current, Target);
        }

        private bool PathComplete(ICollection p)
        {
            return p is not { Count: > 1 };
        }
    }
}
