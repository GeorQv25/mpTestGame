using UnityEngine;


public class GateSpawnPoint : MonoBehaviour
{
    private void Awake() => GateSpawnPointsHolder.AddSpawnPoint(transform);
    private void OnDestroy() => GateSpawnPointsHolder.RemoveSpawnPoint(transform);
}