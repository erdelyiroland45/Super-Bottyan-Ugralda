using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class beszed : MonoBehaviour
{
    [SerializeField] private GameObject beszedUI;

    private void Start()
    {
        beszedUI.SetActive(false);
    }

    public void BeszedVege()
    {
        beszedUI.SetActive(false);
        Time.timeScale = 1f;
    }
}
