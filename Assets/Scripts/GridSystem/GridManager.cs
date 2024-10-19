using System;
using System.Collections.Generic;
using System.Linq;
using GridSystem.GridItems;
using UnityEngine;

namespace GridSystem
{
    [ExecuteInEditMode]
    public class GridManager : Singleton<GridManager>
    {
        private bool generatingGrid;
        private const string GridTag = "Grid";
        private readonly List<GridItem> gridItems = new ();
        private GameObject gridObject;
        [SerializeField] private Transform gridArea;
        [SerializeField] private GameObject gridPrefab;
        [SerializeField] private GameObject rowPrefab;
        [SerializeField] private GameObject cardPrefab;
        private const int MinimumItems = 3;
        private const int MaximumRows = 100;
        private const int MaximumColumns = 100;
        [SerializeField] [Range(MinimumItems, MaximumRows)] public int amountOfRows = MaximumRows / 2;
        [SerializeField] [Range(MinimumItems, MaximumColumns)] public int amountOfColumns = MaximumColumns / 2;

        #region Generate Grid
        #if UNITY_EDITOR
        private void OnValidate()
        {
            return;
            if (Application.isPlaying) return;
            if (generatingGrid) return;
            GenerateGrid();
        }

        [ContextMenu(nameof(ResetGridGenerationSystem))]
        public void ResetGridGenerationSystem()
        {
            generatingGrid = false;
            amountOfRows = MaximumRows / 2;
            amountOfColumns = MaximumColumns / 2;
            GenerateGrid();
        }
        
        [ContextMenu(nameof(Initialise))]
        public void Initialise()
        {
            SetGridItemsCollection();
            foreach (var item in gridItems)
            {
                item.Initialise(item.Indices);
            }
            SetNeighbors();
        }
        #endif 
        
        private void GenerateGrid(Action completeCallback = null)
        {
            generatingGrid = true;
            ResetGrid(()=>CreateGrid(() =>
            {
                gridObject = GameObject.FindWithTag(GridTag);
                generatingGrid = false;
                completeCallback?.Invoke();
            }));
        }

        private void CreateGrid(Action completeCallback = null)
        {
            gridObject = Instantiate(gridPrefab, gridArea);
            for (var i = 0; i < amountOfRows; i++)
            {
                var row = Instantiate(rowPrefab, gridObject.transform).GetComponent<GridRow>();
                for (var j = 0; j < amountOfColumns; j++)
                {
                    if (Application.isPlaying)
                    {
                        var gridItem = Instantiate(cardPrefab, row.transform).GetComponent<GridItem>();
                        gridItem.Initialise(new KeyValuePair<int, int>(j, i));
                        gridItems.Add(gridItem);
                    }
                    else
                    {
                        Instantiate(cardPrefab, row.transform);
                    }
                }
            }
            SetNeighbors();
            completeCallback?.Invoke();
        }
        
        private void SetNeighbors()
        {
            foreach (var item in gridItems)
            {
                item.SetNeighbors();
            }
        }
        
        private void SetGridItemsCollection()
        {
            gridItems.Clear();
            gridObject = GameObject.FindWithTag(GridTag);
            for (var i = 0; i < amountOfRows; i++)
            {
                for (var j = 0; j < amountOfColumns; j++)
                {
                    var v = gridObject.transform.GetChild(i).transform.GetChild(j).GetComponent<GridItem>();
                    gridItems.Add(v);
                    v.Initialise(new KeyValuePair<int, int>(j, i));
                }
            }
        }

        private void ResetGrid(Action completeCallback = null)
        {
            gridObject = GameObject.FindWithTag(GridTag);
            if (!gridObject)
            {
                completeCallback?.Invoke();
                return;
            }
            if (Application.isPlaying)
            {
                Destroy(gridObject);
                completeCallback?.Invoke();
            }
            else
            {
                UnityEditor.EditorApplication.delayCall+=()=>
                {
                    DestroyImmediate(gridObject);
                    completeCallback?.Invoke();
                };
            }
        }
        #endregion Generate Grid
        

        public GridItem GetItem(KeyValuePair<int, int> indices)
        {
            var x = indices.Key; //Cache for LINQs hidden multiple access.
            var y = indices.Value;
            return gridItems.Where(gridItem => gridItem.Indices.Key == x).FirstOrDefault(gridItem => gridItem.Indices.Value == y);
        }
    }
}

