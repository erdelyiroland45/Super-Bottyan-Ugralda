using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : MonoBehaviour
{
    public static bool BoltActive = false;
    public GameObject ShopsUI;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (BoltActive)
            {
                Vissza();
            }
            else 
            {
                Pause();
            }
        }
    }

    void Vissza() 
    {
        ShopsUI.SetActive(false);
        Time.timeScale = 1f;
        BoltActive = false;
    }

    void Pause() 
    {
        ShopsUI.SetActive(true);
        Time.timeScale = 0f;
        BoltActive = true;
    }
}
