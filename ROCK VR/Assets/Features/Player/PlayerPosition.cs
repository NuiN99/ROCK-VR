using NuiN.ScriptableHarmony;
using UnityEngine;

public class PlayerPosition : MonoBehaviour
{
    public static Vector3 Value { get; private set; }
    void FixedUpdate()
    {
        Value = transform.position;
    }
}
