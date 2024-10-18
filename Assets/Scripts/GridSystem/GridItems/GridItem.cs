using System;
using UnityEngine;
using UnityEngine.UI;

namespace GridSystem
{
    [RequireComponent(typeof(Image))]
    public abstract class GridItem : MonoBehaviour
    {
        private bool Initialised { get; set; }
        private bool Active { get; set; }
        protected Image Display;
        private Action<GridItem> onClick;

        public void Initialise()
        {
            if (Initialised)
            {
                Debug.LogError($"Do not initialise {nameof(GridItem)} more than once.");
                return;
            }
            AssignFields();
            Initialised = true;
            Active = true;
        }

        private void AssignFields()
        {
            //Secured by the require component attribute.
            Display = GetComponent<Image>();
        }

        private void OnDisable()
        {
            if (onClick != null)
            {
                onClick -= GridManager.OnItemClick;
            }
        }
    
        private void OnClick()
        {
            if (!Active) return;
            onClick(this);
        }

        public void ResetItem()
        {
            Active = true;
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

        public void SetComponentOrder(GridItem component)
        {
            UnityEditorInternal.ComponentUtility.MoveComponentUp(component);
        }
        
    }
}
