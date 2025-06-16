using UnityEngine;

public class CharacterPush : MonoBehaviour
{
    public CharacterController controller;
    public float pushPower = 4f;

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
