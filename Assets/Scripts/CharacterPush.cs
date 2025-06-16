using UnityEngine;

/// <summary>
/// Handles the logic for allowing the player to push rigidbody objects using a CharacterController.
/// </summary>
public class CharacterPush : MonoBehaviour
{
    /// <summary>
    /// Reference to the CharacterController component on the player.
    /// </summary>
    public CharacterController controller;

    /// <summary>
    /// Force applied to pushable objects.
    /// </summary>
    public float pushPower = 4f;

    /// <summary>
    /// Called when the CharacterController collides with another object.
    /// Attempts to apply force to rigidbody objects.
    /// </summary>
    /// <param name="hit">Collision data from the controller hit.</param>
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody body = hit.collider.attachedRigidbody;

        // Ignore if no rigidbody or it's kinematic
        if (body == null || body.isKinematic)
            return;

        // Prevent pushing down (like stairs)
        if (hit.moveDirection.y < -0.3f)
            return;

        // Only horizontal push direction
        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        Vector3 collisionPoint = hit.point;

        // Apply force at collision point
        body.AddForceAtPosition(pushDir * pushPower, collisionPoint, ForceMode.Impulse);
    }
}
