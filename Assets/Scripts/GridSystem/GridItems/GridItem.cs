using System;
using System.Collections.Generic;
using System.Linq;
using Movement;
using UnityEngine;
using UnityEngine.Serialization;
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
        public int gCost;
        public int fCost;
        #endregion
        public GridItem[] Neighbors { get; private set; }
        #if UNITY_EDITOR
        [SerializeField] protected GameObject pelletPrefab; //These references are only needed while building the Grid, no point in storing them in builds
        #endif

        public void Initialise(KeyValuePair<int, int> indices)
        {
            Display = GetComponent<Image>();
            Indices = indices;
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
        
        public void CalculateGAndHCost(GridItem current, int gCosts)
        {
            previous = current;
            SetGCost(gCosts);
            CalculateFCost();
            
            
        }
        
        public void InitialisePathfindingValues()
        {
            gCost = int.MaxValue;
            CalculateFCost();
            previous = null;
        }
        
        public void SetPathFindingStartPosition()
        {
            gCost = 0;
            CalculateFCost();
        }
        
        private void CalculateFCost()
        {
            fCost = gCost + fCost;
        }
        
        private void SetGCost(int cost)
        {
            gCost = cost;
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
