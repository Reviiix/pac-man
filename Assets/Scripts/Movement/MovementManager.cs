using System;
using Abstractions;
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
        [SerializeField] private RandomEnemyMovement randomEnemy;
        [SerializeField] private SeekerEnemyMovement seekerEnemy;
        private bool canMove;

        public void Initialise()
        {
            CreateCollection();
            player.Initialise(this);
            randomEnemy.Initialise(this);
            seekerEnemy.Initialise(this);
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
            randomEnemy.CheckPosition();
            seekerEnemy.CheckPosition();
            MovementJobs.MoveObjects(objectsToMove, CalculateDestinations());
        }
        
        /// <summary>
        /// job system works better with collections than single objects
        /// </summary>
        private void CreateCollection()
        {
            objectsToMove = new[] { player.transform, randomEnemy.transform, seekerEnemy.transform };
        }

        private Vector3[] CalculateDestinations()
        {
            return new [] {player.GetDestination(), randomEnemy.GetDestination(), seekerEnemy.GetDestination()};
        }
        
        public GridItem GetPlayerTransform()
        {
            return player.GetNextPosition();
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
