using System;
using System.Collections.Generic;
using Abstractions;
using GridSystem;
using GridSystem.GridItems;
using Movement.MovingObjects;
using UnityEngine;

namespace Movement
{
    public class MovementManager : Singleton<MovementManager>
    {
        public static readonly Direction[] Directions = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        private Transform[] objectsToMove;
        [SerializeField] private PlayerMovement player;
        [SerializeField] private SeekerEnemyMovement[] seekerEnemy;
        private bool canMove;

        public void Initialise()
        {
            CreateCollection();
            player.Initialise(this);
            foreach (var seeker in seekerEnemy)
            {
                seeker.Initialise(this);
            }
            
            canMove = true;
        }

        private void Update()
        {
            if (!canMove) return;
            player.CheckForInput();
            MoveObjects();
        }

        private void MoveObjects()
        {
            player.CheckPosition();
            foreach (var seeker in seekerEnemy)
            {
                seeker.CheckPosition();
            }
            MovementJobs.MoveObjects(objectsToMove, CalculateDestinations());
        }
        
        /// <summary>
        /// job system works better with collections than single objects
        /// </summary>
        private void CreateCollection()
        {
            // objectsToMove = new[] { player.transform, randomEnemy.transform, seekerEnemy.transform };
            objectsToMove = new Transform[1 + seekerEnemy.Length];
            objectsToMove[0] = player.transform;
            for (var i = 0; i < seekerEnemy.Length; i++)
            {
                objectsToMove[i + 1] = seekerEnemy[i].transform;
            }
        }

        private Vector3[] CalculateDestinations()
        {
            var array = new Vector3[1 + seekerEnemy.Length];

            for (var i = 0; i < array.Length; i++)
            {
                var movement = new Vector3();
                
                if (i == 0)
                {
                    movement = player.GetDestination();
                }
                else
                {
                    movement = seekerEnemy[i - 1].GetDestination();
                }

                array[i] = movement;
            }
            
            return array;
        }
        
        public GridItem GetPlayersNextPosition()
        {
            return player.GetNextPosition();
        }
        
        public GridItem GetInversePlayerTransform()
        {
            return GetInverseGridItem(GetPlayersNextPosition());
        }
        
        private static GridItem GetInverseGridItem(GridItem original)
        {
            return GridManager.Instance.GetItem(new KeyValuePair<int, int>(GetInverseX(original.Indices.Key, GridManager.Instance.amountOfColumns / 2), original.Indices.Value));
        }

        private static int GetInverseX(int x, int centreX)
        {
            if (x > centreX)
            {
                return GetInverseXRight(x, centreX);
            }
            if (x < centreX)
            {
                return GetInverseXLeft(x, centreX);
            }
            return x; //Exact centre
        }

        private static int GetInverseXLeft(int x, int centreX)
        {
            return centreX + (centreX - x);
        }
        
        private static int GetInverseXRight(int x, int centreX)
        {
            return centreX + (centreX - x);
        }

        public Direction GetPlayerDirection()
        {
            return player.currentMovementDirection;
        }

        public static Direction GetOppositeDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public static Direction GetDirectionOfNeighbor(GridItem original, GridItem neighbor)
        {
            if (neighbor.Indices.Key <= original.Indices.Key) return Direction.Left;
            if (neighbor.Indices.Key >= original.Indices.Key) return Direction.Right;
            if (neighbor.Indices.Value >= original.Indices.Value) return Direction.Up;
            if (neighbor.Indices.Value <= original.Indices.Value) return Direction.Down;
            Debug.Log($"{nameof(original)} has no neighbors");
            return Direction.Left;
        }

        public enum Direction
        {
            Up, Down, Left, Right
        }
    }
}
