using UnityEngine;


[CreateAssetMenu(fileName = "NewConfig", menuName = "Configs/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [field:SerializeField] public float RotationSpeed { get; private set; }
    [field: SerializeField] public float minX { get; private set; } = -30;
    [field:SerializeField]public float maxX { get; private set; } = 55;
}