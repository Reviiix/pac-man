using UnityEngine;

namespace GridSystem.GridItems
{
    public class Wall : GridItem
    {
        #if UNITY_EDITOR
        public void InitialiseWall()
        {
            Initialise(Indices);
            Display.color = Color.black;
        }
    
        [ContextMenu(nameof(RevertToBlank))]
        public void RevertToBlank()
        {
            var v = gameObject.AddComponent<Blank>();
            v.InitialiseBlank();
            RemoveSelf();
        }
        #endif
    }
}
