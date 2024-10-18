using System;
using UnityEngine;
using UnityEngine.UI;

namespace GridSystem
{
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(Button))]
    public class GridItem : MonoBehaviour
    {
        private bool Initialised { get; set; }
        private bool Active { get; set; }
        private Image display;
        private Action<GridItem> onClick;

        public void Initialise(Action<GridItem> gridManagerOnClick)
        {
            if (Initialised)
            {
                Debug.LogError($"Do not initialise {nameof(GridItem)} more than once.");
                return;
            }
            AssignFields();
            SubscribeToEvents(gridManagerOnClick);
            Initialised = true;
            Active = true;
        }

        private void AssignFields()
        {
            //Secured by the require component attribute.
            GetComponent<Button>().onClick.AddListener(OnClick);
            display = GetComponent<Image>();
        }
    
        private void SubscribeToEvents(Action<GridItem> gridManagerOnClick)
        {
            onClick += gridManagerOnClick;
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

        public void ResetCard(bool instant = true)
        {
            Active = true;
        }
    }
}
