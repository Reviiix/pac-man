public static class ProjectSettings
{
    public struct DebugSettings
    {
        public const bool DebugTools = true;
        private const bool _debugGridIndices = false;
        public static bool DebugGridIndices => _debugGridIndices && DebugTools;
    
        private const bool _debugEnemyTargets = true;
        public static bool DebugEnemyTargets => _debugEnemyTargets && DebugTools;
    
        private const bool _debugLogs = true;
        public static bool DebugLogs => _debugLogs && DebugTools;
    }
}
