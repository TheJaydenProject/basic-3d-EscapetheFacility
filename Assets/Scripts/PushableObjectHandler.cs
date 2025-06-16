using UnityEngine;

/// <summary>
/// Applies push force to rigidbody objects when the player collides with them using a CharacterController.
/// Useful for simulating realistic physics like pushing crates or barrels.
/// </summary>
/*
 * Author: Jayden Wong
 * Date: 6/16/2025
 * Description: Enables the player to push non-kinematic rigidbody objects by applying horizontal force
 * during collisions. Also logs a message when pushing, throttled to once every few seconds to prevent spam.
 */
public class PushableObjectHandler : MonoBehaviour
{
    /// <summary>
    /// CharacterController attached to the player.
    /// Required for detecting collision interactions.
    /// </summary>
    public CharacterController controller;

    /// <summary>
    /// Strength of the push applied to the object.
    /// </summary>
    public float pushPower = 4f;

    /// <summary>
    /// Minimum delay (in seconds) between push debug logs.
    /// </summary>
    public float pushLogCooldown = 3f;

    /// <summary>
    /// Timer to control push log frequency.
    /// </summary>
    private float lastPushTime = -Mathf.Infinity;

    /// <summary>
    /// Triggered when the CharacterController hits another collider.
    /// Tries to apply impulse force if the object is pushable.
    /// </summary>
    /// <param name="hit">Collision information from the controller.</param>
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // Exit if the object can't be pushed
        if (body == null || body.isKinematic)
            return;

        // Prevent pushing downward
        if (hit.moveDirection.y < -0.3f)
            return;

        // Push only horizontally
        Vector3 pushDirection = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);

        // Apply the push force at collision point
        body.AddForceAtPosition(pushDirection * pushPower, hit.point, ForceMode.Impulse);

        // Throttled debug message
        if (Time.time - lastPushTime >= pushLogCooldown)
        {
            Debug.Log($"[PushableObjectHandler] Pushed: {body.name}");
            lastPushTime = Time.time;
        }
    }
}
