using System;
using System.Collections.Generic;
using System.Linq;
using Abstractions;
using GridSystem;
using GridSystem.GridItems;
using UnityEngine;

namespace Movement
{
    public class Pathfinder : Singleton<Pathfinder>
    {
        private static GridManager _grid;
        private GridItem[] openPath;
        private List<GridItem> openList;
        private List<GridItem> closedList;
        private const int StraightMovementCost = 10;
        private const int DiagonalMovementCost = 14;

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

        public Queue<GridItem> FindPath(GridItem start, GridItem end)
        {
            openList = new List<GridItem> { start };
            closedList = new List<GridItem>();
            SetInitialisePathfindingValues();
            start.SetPathFindingStartPosition();
            while (openList.Count > 0)
            {
                var current = GetTheLowestFCostGridItem(openList);
                if (current == end)
                {
                    return CalculatePath(end);
                }
                openList.Remove(current);
                closedList.Add(current);
                SetNeighbors(current);
            }
            Debug.LogWarning("No pathway available.");
            return null;
        }

        private void SetNeighbors(GridItem current)
        {
            foreach (var neighbor in current.Neighbors)
            {
                if (neighbor is Wall) continue;
            
                if (closedList.Contains(neighbor)) continue;

                var potentialGCosts = current.gCost + CalculateDistanceCost(current, neighbor);
            
                if (potentialGCosts >= neighbor.gCost) continue;
            
                neighbor.CalculateGAndHCost(current, potentialGCosts);

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
    
        private static int CalculateDistanceCost(GridItem a, GridItem b)
        {
            var x = (int)MathF.Abs(a.Indices.Key - b.Indices.Key);
            var y = (int)MathF.Abs(a.Indices.Value - b.Indices.Value);
            return DiagonalMovementCost * Mathf.Min(x, y) + StraightMovementCost * Mathf.Abs(x - y);
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
