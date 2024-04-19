using UnityEngine;


[CreateAssetMenu(fileName = "NewConfig", menuName = "Configs/LauncherConfig")]
public class LauncherConfig : ScriptableObject
{
    [field: SerializeField] public float MinHoldAmplification { get; private set; } = 0.2f;
    [field: SerializeField] public float MaxHoldAmplification { get; private set; } = 3f;
    [field: SerializeField] public float ForceAmmount { get; private set; } = 40f;
    [field: SerializeField] public float AttackRate { get; private set; } = 5f;
}