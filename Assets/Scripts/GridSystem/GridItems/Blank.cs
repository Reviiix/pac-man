using JetBrains.Annotations;
using UnityEngine;

namespace GridSystem.GridItems
{
    public class Blank : GridItem
    {
        private bool hasPellet;
        [CanBeNull] private GameObject pellet;
        private bool hasBonusItem;
        [CanBeNull] private GameObject bonusItem;

        #if UNITY_EDITOR
        public void InitialiseBlank(GameObject newPelletPrefab, GameObject newBonusItemPrefab)
        {
            Initialise(Indices);
            pelletPrefab = newPelletPrefab;
            bonusItemPrefab = newBonusItemPrefab;
            Display.color = Color.white;
            UnityEditorInternal.ComponentUtility.MoveComponentUp(GetComponent<Blank>());
        }

        #region Pellet
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
            pellet.GetComponent<Pellet>().Initialise(this);
            PelletManager.Instance.AddPelletToList(this);
        }
        
        [ContextMenu(nameof(RemovePellet))]
        private void RemovePellet()
        {
            // if (!hasPellet) return;
            hasPellet = false;
            DestroyPelletObject();
            PelletManager.Instance.RemovePelletFromList(this);
        }

        private void DestroyPelletObject()
        {
            if (pellet == null)
            {
                pellet = transform.GetChild(0).gameObject;
                if (pellet == null) return;
            }
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
        #endregion Pellet

        #region BonusItem
        [ContextMenu(nameof(AddBonusItem))]
        private void AddBonusItem()
        {
            if (hasBonusItem)
            {
                Debug.LogWarning($"Attempting to add a duplicate {nameof(bonusItemPrefab)} to {nameof(GridItem)}: {Indices.Key},{Indices.Value}. Skipped creation to avoid duplicate.");
                return;
            }
            hasBonusItem = true;
            var parent = transform;
            bonusItem = Instantiate(bonusItemPrefab, parent.position, Quaternion.identity, parent);
            bonusItem.GetComponent<BonusItem>().Initialise(this);
            PelletManager.Instance.AddPelletToList(this);
        }
        
        [ContextMenu(nameof(RemoveBonusItem))]
        private void RemoveBonusItem()
        {
            //if (!hasBonusItem) return;
            hasBonusItem = false;
            DestroyBonusItemObject();
            //PelletManager.Instance.RemovePelletFromList(this);
        }

        private void DestroyBonusItemObject()
        {
            if (bonusItem == null)
            {
                bonusItem = transform.GetChild(0).gameObject;
                if (bonusItem == null) return;
            }
            if (Application.isPlaying)
            {
                Destroy(bonusItem);
            }
            else
            {
                UnityEditor.EditorApplication.delayCall+=()=>
                {
                    DestroyImmediate(bonusItem);
                };
            }
        }
        #endregion BonusItem

        [ContextMenu(nameof(ChangeToWall))]
        private void ChangeToWall()
        {
            gameObject.AddComponent<Wall>().InitialiseWall(pelletPrefab, bonusItemPrefab);
            RemovePellet();
            RemoveSelf();
        }
        #endif
    }
}
