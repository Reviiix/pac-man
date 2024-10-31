using System;
using GridSystem;
using GridSystem.GridItems;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    private GridItem parent;

    public void Initialise(GridItem p)
    {
        parent = p;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        PelletHit();
    }

    private void OnTriggerStay(Collider other)
    {
        PelletHit();
    }

    private void PelletHit()
    {
        Debug.Log("Hah`ahahahakbdiyvwqe");
        Destroy(gameObject);
    }
}
