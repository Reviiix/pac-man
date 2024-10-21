using UnityEditor;
using UnityEngine;

namespace GridSystem.GridItems
{
    public class Wall : GridItem
    {
        #if UNITY_EDITOR
        public void InitialiseWall(GameObject pellet)
        {
            Initialise(Indices);
            Display.color = Color.black;
            pelletPrefab = pellet;
        }
    
        [ContextMenu(nameof(ChangeToBlank))]
        private void ChangeToBlank()
        {
            var blank = gameObject.AddComponent<Blank>();
            blank.InitialiseBlank(pelletPrefab);
            RemoveSelf();
        }
        #endif
    }
}
