using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bolt : MonoBehaviour
{
    public static bool Bolt = false;
    public GameObject ShopsUI;

    void Update ()
    {
        if (collision.CompareTag("Player"))
        {
            if (Bolt)
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
        Bolt = false;
    }
    void Pause() 
    {
        ShopsUI.SetActive(true);
        Time.timeScale = 0f;
        Bolt = true;
    }
}
