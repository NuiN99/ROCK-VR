using UnityEngine;

namespace NuiN.NExtensions
{
    [CreateAssetMenu(menuName = "ScriptableObjects/CameraShake/Shake Config")]
    public class CameraShakeConfigSO : ScriptableObject
    {
        [field: SerializeField] public float PositionStrength { get; private set; } = 0.08f;
        [field: SerializeField] public float RotationStrength { get; private set; } = 0.1f;
        [field: SerializeField] public float Frequency { get; private set; } = 25f;
        [field: SerializeField] public int NumBounces { get; private set; } = 5;

        [field: SerializeField, Header("Clamp Options")] public bool Clamp { get; private set; }
        [field: SerializeField] public float MaxPosStrength { get; private set; } = 0.2f;
        [field: SerializeField] public float MaxRotStrength { get; private set; } = 0.225f;
    }
}