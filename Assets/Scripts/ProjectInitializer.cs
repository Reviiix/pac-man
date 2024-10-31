using GridSystem;
using Movement;
using UnityEngine;

/// <summary>
/// Using a project initializer can help reduce the amount of race conditions by offering more granular control of the initialisation sequence.
/// Also offers an exy way to implement dependency injections instead of always relying on the singleton pattern.
/// </summary>
public class ProjectInitializer : MonoBehaviour
{
    private void Start()
    {
        GridManager.Instance.Initialise();
        MovementManager.Instance.Initialise();
        Pathfinder.Initialise(GridManager.Instance);
        PelletManager.Instance.Initialise();
    }
}
