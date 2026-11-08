using System;
using System.Collections.Generic;
using System.Linq;
using Abstractions;
using GridSystem;
using GridSystem.GridItems;
using UnityEngine;

namespace Movement
{
    /// <summary>
    /// Generate a path towards the the target using the A* algorithm.
    /// </summary>
    public class Pathfinder : Singleton<Pathfinder>
    {
        private static GridManager _grid;
        private GridItem[] openPath;
        private List<GridItem> openList;
        private List<GridItem> closedList;
        private const int StraightMovementCost = 10;

        public static void Initialise(GridManager gridManager)
        {
            _grid = gridManager;
        }

        private static void SetInitialisePathfindingValues()
        {
            var amountOfGridItems = _grid.gridItems.Count;
        
            for (var i = 0; i < amountOfGridItems; i++)
            {
                _grid.gridItems[i].InitialisePathfindingValues();
            }
        }

        /// <summary>
        /// Returns the shortest walkable path from start to end, or null when there is none.
        /// </summary>
        public Queue<GridItem> FindPath(GridItem start, GridItem end)
        {
            if (start == null || end == null || end is Wall)
            {
                Debug.LogWarning("No pathway available: start or end is missing, or end is a wall.");
                return null;
            }

            openList = new List<GridItem> { start };
            closedList = new List<GridItem>();
            SetInitialisePathfindingValues();
            start.SetPathFindingStartPosition(CalculateDistanceCost(start, end));
            while (openList.Count > 0)
            {
                var current = GetTheLowestFCostGridItem(openList);
                if (current == end)
                {
                    return CalculatePath(end);
                }
                openList.Remove(current);
                closedList.Add(current);
                SetNeighbors(current, end);
            }
            Debug.LogWarning("No pathway available.");
            return null;
        }

        /// <summary>
        /// Records cheaper routes to the current item's walkable neighbors and adds them to the open list.
        /// </summary>
        private void SetNeighbors(GridItem current, GridItem end)
        {
            foreach (var neighbor in current.Neighbors)
            {
                if (neighbor is Wall) continue;
            
                if (closedList.Contains(neighbor)) continue;

                var potentialGCosts = current.gCost + CalculateDistanceCost(current, neighbor);
            
                if (potentialGCosts >= neighbor.gCost) continue;
            
                neighbor.CalculateGAndHCost(current, potentialGCosts, CalculateDistanceCost(neighbor, end));

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                }
            }
        }

        private static Queue<GridItem> CalculatePath(GridItem end)
        {
            var path = new List<GridItem> { end };
            var current = end;
            while (current.previous != null)
            {
                path.Add(current.previous);
                current = current.previous;
            }
            path.Reverse();
            return new Queue<GridItem>(path);
        }
    
        /// <summary>
        /// Returns the Manhattan distance cost between two items, since movement is restricted to 4 directions.
        /// </summary>
        private static int CalculateDistanceCost(GridItem a, GridItem b)
        {
            var x = Math.Abs(a.Indices.Key - b.Indices.Key);
            var y = Math.Abs(a.Indices.Value - b.Indices.Value);
            return StraightMovementCost * (x + y);
        }

        private static GridItem GetTheLowestFCostGridItem(IReadOnlyList<GridItem> items)
        {
            var lowestCostNode = items[0];
            foreach (var item in items.Where(t => t.fCost < lowestCostNode.fCost))
            {
                lowestCostNode = item;
            }
            return lowestCostNode;
        }
    }
}
