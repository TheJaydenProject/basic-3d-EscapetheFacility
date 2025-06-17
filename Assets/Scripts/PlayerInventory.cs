using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public bool hasKeycard = false;
    public bool hasGasMask = false;

    public void GiveKeycard()
    {
        hasKeycard = true;
    }

    public void GiveGasMask()
    {
        hasGasMask = true;
    }

    public bool HasKeycard()
    {
        return hasKeycard;
    }

    public bool HasGasMask()
    {
        return hasGasMask;
    }
}
