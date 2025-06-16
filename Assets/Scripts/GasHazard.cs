using UnityEngine;

public class GasHazard : MonoBehaviour
{
    public int damageAmount = 10;
    public float damageInterval = 2f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                if (!IsInvoking(nameof(ApplyGasDamage)))
                {
                    InvokeRepeating(nameof(ApplyGasDamage), 0f, damageInterval);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CancelInvoke(nameof(ApplyGasDamage));
        }
    }

    void ApplyGasDamage()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerHealth health = player.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damageAmount);
            }
        }
    }
}
