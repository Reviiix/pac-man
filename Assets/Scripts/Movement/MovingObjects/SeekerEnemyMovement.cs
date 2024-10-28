using System;
using System.Collections;
using System.Collections.Generic;
using GridSystem.GridItems;
using UnityEngine;

namespace Movement.MovingObjects
{
    [Serializable]
    public class SeekerEnemyMovement : MovingObject
    {
        [SerializeField] private Pathfinder pathFinder;
        private Queue<GridItem> path;
        public GridItem target;
    
        protected override void OnDestinationReached()
        {
            base.OnDestinationReached();
            UpdateNext();
        }
        
        private void UpdateNext()
        {
            UpdatePath();
            if (PathComplete(path)) return;
            path.Dequeue(); //Current position
            next = path.Dequeue();
            currentMovementDirection = MovementManager.GetDirectionOfNeighbor(current, next);
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
