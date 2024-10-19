using GridSystem;
using UnityEngine;

public class ProjectInitializer : MonoBehaviour
{
    private void Start()
    {
        GridManager.Instance.Initialise();
        MovementManager.Instance.Initialise();
    }
}
