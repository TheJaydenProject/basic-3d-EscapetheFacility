using UnityEngine;

/// <summary>
/// Continuously rotates the GameObject around the Y-axis.
/// Useful for spinning collectibles or interactive items.
/// </summary>
public class SpinObject : MonoBehaviour
{
    /// <summary>
    /// Rotation speed around the Y-axis in degrees per second.
    /// </summary>
    public float yRotationSpeed = 90f;

    /// <summary>
    /// Rotates the object every frame around the Y-axis.
    /// </summary>
    void Update()
    {
        transform.Rotate(0, yRotationSpeed * Time.deltaTime, 0);
    }
}
