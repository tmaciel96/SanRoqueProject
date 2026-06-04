using Unity.AI.MCP.Editor;
using UnityEditor;

/// <summary>
/// Keeps the Unity MCP bridge reachable from Codex when Unity reports zero
/// direct MCP connections for the current account/session.
/// </summary>
[InitializeOnLoad]
internal static class UnityMcpDirectConnectionUnlock
{
    const int MinimumDirectConnections = 1;

    static UnityMcpDirectConnectionUnlock()
    {
        EditorApplication.delayCall += EnsureDirectConnectionAllowed;
        UnityMCPBridge.MaxDirectConnectionsPolicyChanged += EnsureDirectConnectionAllowed;
    }

    static void EnsureDirectConnectionAllowed()
    {
        if (UnityMCPBridge.MaxDirectConnectionsResolver() == 0)
        {
            UnityMCPBridge.MaxDirectConnectionsResolver = () => MinimumDirectConnections;
        }
    }
}
