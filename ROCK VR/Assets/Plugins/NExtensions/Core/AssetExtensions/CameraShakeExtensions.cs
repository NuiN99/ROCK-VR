using CameraShake;
using UnityEngine;

namespace NuiN.NExtensions
{
    public static class CameraShakeExtensions
    {
        public static void ShortShake2D(this CameraShakePresets presets, CameraShakeConfigSO shakeParams)
        {
            float clampedPos = shakeParams.Clamp ? Mathf.Min(shakeParams.MaxPosStrength, shakeParams.PositionStrength) : shakeParams.PositionStrength;
            float clampedRot = shakeParams.Clamp ? Mathf.Min(shakeParams.MaxRotStrength, shakeParams.RotationStrength) : shakeParams.RotationStrength;

            presets.ShortShake2D(clampedPos, clampedRot, shakeParams.Frequency, shakeParams.NumBounces);
        }
    }
}
