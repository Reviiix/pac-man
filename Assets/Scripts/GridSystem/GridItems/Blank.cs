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
            hasPellet = false;
            DestroyPelletObject();
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
        public void InitialiseBlank(GameObject newPelletPrefab)
        {
            Initialise(Indices);
            pelletPrefab = newPelletPrefab;
            Display.color = Color.white;
            UnityEditorInternal.ComponentUtility.MoveComponentUp(GetComponent<Blank>());
        }
    
        [ContextMenu(nameof(AddPellet))]
        private void AddPellet()
        {
            if (hasPellet)
            {
                Debug.LogWarning($"Attempting to add a duplicate {nameof(pelletPrefab)} to {nameof(GridItem)}: {Indices.Key},{Indices.Value}. Skipped creation to avoid duplicate.");
                return;
            }
            hasPellet = true;
            var parent = transform;
            pellet = Instantiate(pelletPrefab, parent.position, Quaternion.identity, parent);
        }
    
        [ContextMenu(nameof(ChangeToWall))]
        private void ChangeToWall()
        {
            gameObject.AddComponent<Wall>().InitialiseWall(pelletPrefab);
            RemovePellet();
            RemoveSelf();
        }
        #endif
    }
}
