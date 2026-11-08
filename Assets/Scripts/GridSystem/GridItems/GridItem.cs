using System;
using System.Collections.Generic;
using System.Linq;
using Movement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GridSystem.GridItems
{
    [RequireComponent(typeof(Image))]
    public abstract class GridItem : MonoBehaviour
    {
        public KeyValuePair<int, int> Indices { get; private set; } //Key = X, Value = Y
        protected Image Display;
        public GridItem previous;
        #region Pathfinding
        public int gCost; //Cost of the cheapest known path from the start to this item.
        public int hCost; //Estimated cost from this item to the target (heuristic).
        public int fCost; //gCost + hCost, used to pick which item to explore next.
        #endregion
        public GridItem[] Neighbors { get; private set; }
        #if UNITY_EDITOR
        [SerializeField] protected GameObject pelletPrefab; //These references are only needed while building the Grid, no point in storing them in builds
        [SerializeField] protected GameObject bonusItemPrefab;
        #endif
        [SerializeField] private TMP_Text debugIndices;

        public void Initialise(KeyValuePair<int, int> indices)
        {
            Display = GetComponent<Image>();
            Indices = indices;

            if (ProjectSettings.DebugSettings.DebugGridIndices)
            {
                debugIndices.text = $"{Indices.Key},{Indices.Value}";
            }
            else
            {
                Destroy(debugIndices.gameObject);
            }
        }

        public void SetNeighbors()
        {
            Neighbors = new[]
            {
                //Row Above
                GridManager.Instance.GetItem(new KeyValuePair<int, int>(Indices.Key-1,Indices.Value)),
                //Same Row
                GridManager.Instance.GetItem(new KeyValuePair<int, int>(Indices.Key,Indices.Value-1)),
                GridManager.Instance.GetItem(new KeyValuePair<int, int>(Indices.Key,Indices.Value+1)),
                //Row Below
                GridManager.Instance.GetItem(new KeyValuePair<int, int>(Indices.Key+1,Indices.Value)),
            };
            Neighbors = Neighbors.Where(x => x != null).ToArray(); //Remove edge piece neighbors
        }

        public GridItem GetAdjacentItem(MovementManager.Direction direction)
        {
            return direction switch
            {
                MovementManager.Direction.Up => GetUpperNeighbor(),
                MovementManager.Direction.Down => GetLowerNeighbor(),
                MovementManager.Direction.Left => GetLeftNeighbor(),
                MovementManager.Direction.Right => GetRightNeighbor(),
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, $"{nameof(direction)} is not accounted for in the switch statement")
            };
        }

        private GridItem GetUpperNeighbor()
        {
            return Neighbors.FirstOrDefault(x => x.Indices.Value == Indices.Value - 1);
        }
        
        private GridItem GetLowerNeighbor()
        {
            return Neighbors.FirstOrDefault(x => x.Indices.Value == Indices.Value + 1);
        }
        
        private GridItem GetLeftNeighbor()
        {
            return Neighbors.FirstOrDefault(x => x.Indices.Key == Indices.Key - 1);
        }
        
        private GridItem GetRightNeighbor()
        {
            return Neighbors.FirstOrDefault(x => x.Indices.Key == Indices.Key + 1);
        }
        
        public void CalculateGAndHCost(GridItem current, int gCosts, int hCosts)
        {
            previous = current;
            gCost = gCosts;
            hCost = hCosts;
            CalculateFCost();
        }

        public void InitialisePathfindingValues()
        {
            //Unvisited items are "infinitely" far away. fCost is set directly rather than summed to avoid int overflow.
            gCost = int.MaxValue;
            hCost = 0;
            fCost = int.MaxValue;
            previous = null;
        }

        public void SetPathFindingStartPosition(int hCosts)
        {
            gCost = 0;
            hCost = hCosts;
            CalculateFCost();
        }

        private void CalculateFCost()
        {
            fCost = gCost + hCost;
        }
        
        public bool MultiplePathWaysAvailable()
        {
            return Neighbors.Count(x => x is Blank) > 2;
        }

        protected void RemoveSelf()
        {
            if (Application.isPlaying)
            {
                Destroy(this);
            }
            else
            {
                UnityEditor.EditorApplication.delayCall+=()=>
                {
                    DestroyImmediate(this);
                };
            }
        }
    }
}
