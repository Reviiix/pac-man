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

        public void Initialise()
        {
            numberOfPellets = allActivePellets.Count;
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