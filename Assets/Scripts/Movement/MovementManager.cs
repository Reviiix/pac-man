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
        private readonly Dictionary<GridItem, GridItem> mirroredWalkableItems = new();

        public void Initialise()
        {
            CacheMirroredWalkableItems();
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
            var playerPosition = GetPlayersNextPosition();
            return mirroredWalkableItems.TryGetValue(playerPosition, out var mirrored) ? mirrored : playerPosition;
        }

        /// <summary>
        /// Precomputes the walkable mirror of every walkable item so the Scared ghost does no searching at runtime.
        /// </summary>
        private void CacheMirroredWalkableItems()
        {
            mirroredWalkableItems.Clear();
            foreach (var gridItem in GridManager.Instance.gridItems)
            {
                if (gridItem is Wall) continue;
                mirroredWalkableItems[gridItem] = GetInverseGridItem(gridItem);
            }
        }

        /// <summary>
        /// Mirrors the item horizontally across the grid, falling back to the nearest walkable item when the mirror is a wall.
        /// </summary>
        private static GridItem GetInverseGridItem(GridItem original)
        {
            var inverseX = GridManager.Instance.amountOfColumns - 1 - original.Indices.Key;
            var mirrored = GridManager.Instance.GetItem(new KeyValuePair<int, int>(inverseX, original.Indices.Value));
            if (mirrored == null) return original;
            return mirrored is Wall ? GetNearestWalkableItem(mirrored) ?? original : mirrored;
        }

        /// <summary>
        /// Breadth-first search outward from the given item until a non-wall item is found.
        /// </summary>
        private static GridItem GetNearestWalkableItem(GridItem start)
        {
            var visited = new HashSet<GridItem> { start };
            var queue = new Queue<GridItem>();
            queue.Enqueue(start);
            while (queue.Count > 0)
            {
                var item = queue.Dequeue();
                if (item is not Wall) return item;
                foreach (var neighbor in item.Neighbors)
                {
                    if (visited.Add(neighbor))
                    {
                        queue.Enqueue(neighbor);
                    }
                }
            }
            return null;
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

        /// <summary>
        /// Returns the direction from an item to its adjacent neighbor, with rows indexed top to bottom so Up is Value - 1.
        /// </summary>
        public static Direction GetDirectionOfNeighbor(GridItem original, GridItem neighbor)
        {
            var deltaX = neighbor.Indices.Key - original.Indices.Key;
            var deltaY = neighbor.Indices.Value - original.Indices.Value;
            if (deltaX < 0) return Direction.Left;
            if (deltaX > 0) return Direction.Right;
            if (deltaY < 0) return Direction.Up;
            if (deltaY > 0) return Direction.Down;
            Debug.LogWarning($"{nameof(GetDirectionOfNeighbor)} was given the same grid item twice ({original.Indices.Key},{original.Indices.Value}).");
            return Direction.Left;
        }

        public enum Direction
        {
            Up, Down, Left, Right
        }
    }
}
