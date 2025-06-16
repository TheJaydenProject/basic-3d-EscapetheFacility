using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool hasKeycard = false;

    public void GiveKeycard()
    {
        hasKeycard = true;
    }

    public bool HasKeycard()
    {
        return hasKeycard;
    }
}