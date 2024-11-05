using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doga : MonoBehaviour
{
    [SerializeField] private GameObject dogaUI;
    [SerializeField] private GameObject doga2UI;

    private void Start()
    {
        dogaUI.SetActive(false);
    }

    public void Jovalasz()
    {
        ToggleShop();
        doga2UI.SetActive(false);
    }

    public void Rosszvalasz()
    {
        dogaUI.SetActive(false);
        doga2UI.SetActive(false);
        Time.timeScale = 1f;
    }

    public void Utolsojovalasz()
    {
        ToggleShop();
        doga2UI.SetActive(false);
    }

    private void ToggleShop()
    {
        dogaUI.SetActive(true);
    }

}
