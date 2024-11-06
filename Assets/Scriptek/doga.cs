using UnityEngine;
using UnityEngine.SceneManagement;

public class doga : MonoBehaviour
{
    // Define the static event to notify when a mistake is made
    public static event System.Action OnMistakeMade;

    [SerializeField] private GameObject dogaUI;
    [SerializeField] private GameObject doga2UI;

    private void Start()
    {
        dogaUI.SetActive(false);
    }

    // Call this method when a correct answer is given
    public void Jovalasz()
    {
        ToggleShop();
        doga2UI.SetActive(false);
    }

    // Call this method when a wrong answer is given
    public void Rosszvalasz()
    {
        dogaUI.SetActive(false);
        doga2UI.SetActive(false);
        Time.timeScale = 1f;

        // Trigger the OnMistakeMade event
        OnMistakeMade?.Invoke();
    }

    // Call this method for the final answer choice
    public void Utolsojovalasz()
    {
        ToggleShop();
        doga2UI.SetActive(false);
        SceneManager.LoadScene("END");
    }

    // Toggles the shop UI (dogaUI)
    private void ToggleShop()
    {
        dogaUI.SetActive(true);
    }
}
