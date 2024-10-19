using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace GridSystem.GridItems
{
    public class Blank : GridItem
    {
        private bool hasPellet;
        [CanBeNull] private GameObject pellet;

        [ContextMenu(nameof(RemovePellet))]
        private void RemovePellet()
        {
            if (!hasPellet) return;
            DestroyPelletObject();
            hasPellet = false;
        }

        private void DestroyPelletObject()
        {
            if (Application.isPlaying)
            {
                Destroy(pellet);
            }
            else
            {
                UnityEditor.EditorApplication.delayCall+=()=>
                {
                    DestroyImmediate(pellet);
                };
            }
        }

        #if UNITY_EDITOR
        public void InitialiseBlank()
        {
            Initialise(Indices);
            Display.color = Color.white;
        }
    
        [ContextMenu(nameof(AddPellet))]
        public void AddPellet()
        {
            var parent = transform;
            pellet = Instantiate(pelletPrefab, parent.position, Quaternion.identity, parent);
            hasPellet = true;
        }
    
        [ContextMenu(nameof(MakeWall))]
        public void MakeWall()
        {
            gameObject.AddComponent<Wall>().InitialiseWall();
            RemoveSelf();
        }
        #endif
    }
}
