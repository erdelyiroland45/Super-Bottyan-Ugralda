using UnityEngine;

public class Portalkezelo : MonoBehaviour
{
    [SerializeField] private GameObject portal; // Reference to the portal GameObject

    private void Start()
    {
        if (portal != null)
        {
            portal.SetActive(false); // Hide the portal at the beginning
        }
        else
        {
            Debug.LogWarning("Portal GameObject reference is missing in PortalManager.");
        }
    }

    public void ActivatePortal()
    {
        if (portal != null)
        {
            portal.SetActive(true); // Show the portal
            Debug.Log("Portal is now active.");
        }
    }
}
