using UnityEngine;

namespace GridSystem.GridItems
{
    public class Wall : GridItem
    {
        public void SetWall()
        {
            Initialise(Indices);
            Display.color = Color.black;
        }
    
        [ContextMenu(nameof(RevertToBlank))]
        public void RevertToBlank()
        {
            var v = gameObject.AddComponent<Blank>();
            v.SetBlank();
            RemoveSelf();
        }
    }
}
