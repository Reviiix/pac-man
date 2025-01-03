using UnityEngine;

public class LogManager : MonoBehaviour
{
    /// <summary>
    /// Errors and warnings should appear regardless of settings so do not send them through this method
    /// </summary>
    public static void Log(string message, Object context)
    {
        if (ProjectSettings.DebugTools) return;

        Debug.Log(message, context);
    }
}
