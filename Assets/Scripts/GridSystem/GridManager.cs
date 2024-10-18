using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    [ExecuteInEditMode]
    public class GridManager : MonoBehaviour
    {
        public static Action<GridItem> OnItemClick;
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
                        gridItems.Add(gridItem);
                        gridItem.Initialise();
                    }
                    else
                    {
                        Instantiate(cardPrefab, row.transform);
                    }
                }
            }
            completeCallback?.Invoke();
        }

        private void ResetGrid(Action completeCallback = null)
        {
            ResetGridItems();
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
    
        private void ResetGridItems()
        {
            foreach (var item in gridItems)
            {
                item.ResetItem();
            }
            gridItems.Clear();
        }

        private static void OnGridItemClick(GridItem gridItem)
        {
            Debug.Log($"{gridItem.name} clicked.");
            OnItemClick?.Invoke(gridItem);
        }
    }
}

