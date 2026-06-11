using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Administra los puntos de spawn del mapa y el ciclo de vida de los RescueMapMarker.
/// Referenciado por RescueManager para mostrar/ocultar markers.
///
/// Setup en escena:
/// 1. Crear un GameObject vacío "RescueMapController" con este componente.
/// 2. Crear N GameObjects vacíos hijos como puntos de spawn y asignarlos a spawnPoints[].
///    (Reemplazá los botones viejos del mapa con estos transforms)
/// 3. Asignar el prefab RescueMapMarker.
/// </summary>
public class RescueMapController : MonoBehaviour
{
    [Header("Puntos de spawn en el mapa")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("Prefab del marker")]
    [SerializeField] private RescueMapMarker markerPrefab;

    // Mapa request → marker instanciado activo
    private Dictionary<RescueRequest, RescueMapMarker> activeMarkers
        = new Dictionary<RescueRequest, RescueMapMarker>();

    // ── API para RescueManager ────────────────────────────────────────────────

    /// Retorna los índices de spawn points que no tienen un marker activo.
    public List<int> GetAvailableSpawnIndices(List<RescueRequest> existingRequests)
    {
        var usedIndices = new HashSet<int>();
        foreach (var req in existingRequests)
            usedIndices.Add(req.spawnPointIndex);

        var available = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (!usedIndices.Contains(i))
                available.Add(i);
        }
        return available;
    }

    /// Instancia y muestra un marker para el request dado.
    public void ShowMarker(RescueRequest request)
    {
        if (activeMarkers.ContainsKey(request))
        {
            Debug.LogWarning($"[RescueMapController] Ya existe marker para {request.animalData.animalName}");
            return;
        }

        if (request.spawnPointIndex < 0 || request.spawnPointIndex >= spawnPoints.Length)
        {
            Debug.LogError($"[RescueMapController] spawnPointIndex {request.spawnPointIndex} fuera de rango.");
            return;
        }

        Transform spawnPoint = spawnPoints[request.spawnPointIndex];
        RescueMapMarker marker = Instantiate(markerPrefab, spawnPoint.position, Quaternion.identity, spawnPoint);
        marker.Setup(request);

        activeMarkers[request] = marker;
    }

    /// Destruye el marker asociado al request.
    public void HideMarker(RescueRequest request)
    {
        if (!activeMarkers.TryGetValue(request, out var marker)) return;

        if (marker != null) Destroy(marker.gameObject);
        activeMarkers.Remove(request);
    }

    /// Destruye todos los markers activos (útil al cerrar/resetear).
    public void ClearAllMarkers()
    {
        foreach (var marker in activeMarkers.Values)
            if (marker != null) Destroy(marker.gameObject);

        activeMarkers.Clear();
    }

#if UNITY_EDITOR
    // Visualiza los spawn points en el editor
    private void OnDrawGizmos()
    {
        if (spawnPoints == null) return;
        Gizmos.color = Color.yellow;
        foreach (var sp in spawnPoints)
        {
            if (sp == null) continue;
            Gizmos.DrawWireSphere(sp.position, 0.3f);
            UnityEditor.Handles.Label(sp.position + Vector3.up * 0.4f, sp.name);
        }
    }
#endif
}