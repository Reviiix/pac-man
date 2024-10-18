using GridSystem;
using UnityEngine;

public class Wall : GridItem
{
    public void SetWall()
    {
        Initialise();
        Display.color = Color.black;
    }
    
    [ContextMenu(nameof(RevertToBlank))]
    public void RevertToBlank()
    {
        var v = gameObject.AddComponent<Blank>();
            v.SetBlank();
        SetComponentOrder(v);
        RemoveSelf();
    }
}
