using UnityEngine;

namespace GridSystem.GridItems
{
    public class Blank : GridItem
    {
        public void SetBlank()
        {
            Initialise(Indices);
            Display.color = Color.white;
        }
    
        [ContextMenu(nameof(AddPellet))]
        public void AddPellet()
        {
            gameObject.AddComponent<Wall>().SetWall();
            RemoveSelf();
        }
    
        [ContextMenu(nameof(MakeWall))]
        public void MakeWall()
        {
            gameObject.AddComponent<Wall>().SetWall();
            RemoveSelf();
        }
    }
}
