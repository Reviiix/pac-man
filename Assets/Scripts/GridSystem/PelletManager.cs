using System;
using System.Collections.Generic;
using System.Linq;
using Abstractions;
using GridSystem.GridItems;
using UnityEngine;
using UnityEngine.Serialization;

namespace GridSystem
{
    [ExecuteInEditMode]
    public class PelletManager : Singleton<PelletManager>
    {
        [SerializeField] private List<Blank> allActivePellets = new List<Blank>();
        public int numberOfPellets { get; private set; }

        /// <summary>
        /// Counts only tiles that still hold a pellet, so stale list entries and bonus-item tiles are ignored.
        /// </summary>
        public void Initialise()
        {
            numberOfPellets = allActivePellets.Count(tile => tile != null && tile.GetComponentInChildren<Pellet>() != null);
        }

        public void AddPelletToList(Blank gridItem)
        {
            if (allActivePellets.Contains(gridItem)) return;
            allActivePellets.Add(gridItem);
        }

        public void RemovePelletFromList(Blank gridItem)
        {
            if (!allActivePellets.Contains(gridItem)) return;
            allActivePellets.Remove(gridItem);
        }
    }
}