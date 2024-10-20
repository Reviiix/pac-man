using System;
using Abstractions;
using UnityEngine;

namespace Movement
{
    public class MovementManager : Singleton<MovementManager>
    {
        public static readonly Direction[] Directions = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
        private Transform[] objectsToMove;
        [SerializeField] private PlayerMovement player;
        [SerializeField] private RandomEnemyMovement randomEnemy;

        public void Initialise()
        {
            CreateCollection();
            player.Initialise(this);
            randomEnemy.Initialise(this);
        }

        private void Update()
        {
            player.CheckForInput();
            MoveObjects();
        }

        private void MoveObjects()
        {
            player.CheckPosition();
            randomEnemy.CheckPosition();
            MovementJobs.MoveObjects(objectsToMove, GetDestinations());
        }
        
        /// <summary>
        /// job system works better with collections than single objects
        /// </summary>
        private void CreateCollection()
        {
            objectsToMove = new[] { player.transform, randomEnemy.transform };
        }

        private Vector3[] GetDestinations()
        {
            return new [] {player.GetDestination(), randomEnemy.GetDestination()};
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

        public enum Direction
        {
            Up, Down, Left, Right
        }
    }
}
