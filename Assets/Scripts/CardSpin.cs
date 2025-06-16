using UnityEngine;

/// <summary>
/// Continuously rotates the GameObject around the Y-axis at a constant speed.
/// Intended for spinning collectibles or objects that draw player attention.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 6/16/2025
 * Description: This script rotates an object around its Y-axis to create a visual cue,
 * typically used for collectibles or interactive items in a Unity 3D environment.
 */
public class SpinObject : MonoBehaviour
{
    /// <summary>
    /// Rotation speed around the Y-axis in degrees per second.
    /// Can be adjusted in the Inspector.
    /// </summary>
    public float yRotationSpeed = 90f;

    /// <summary>
    /// Rotates the object every frame to simulate spinning.
    /// </summary>
    void Update()
    {
        transform.Rotate(0, yRotationSpeed * Time.deltaTime, 0);
    }
}
