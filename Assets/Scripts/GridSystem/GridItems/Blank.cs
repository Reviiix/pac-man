using GridSystem;
using UnityEngine;

public class Blank : GridItem
{
    public void SetBlank()
    {
        Initialise();
        Display.color = Color.white;
    }
    
    [ContextMenu(nameof(AddPellet))]
    public void AddPellet()
    {
        gameObject.AddComponent<Wall>().SetWall();
        SetComponentOrder(this);
        RemoveSelf();
    }
    
    [ContextMenu(nameof(MakeWall))]
    public void MakeWall()
    {
        gameObject.AddComponent<Wall>().SetWall();
        SetComponentOrder(this);
        RemoveSelf();
    }
}
