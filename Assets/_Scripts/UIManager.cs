using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [Header("Pantallas UI")]
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private Button comenzarButton;

    public void ShowScreen(string screenName)
    {
        HideAllScreens();
        Time.timeScale = 0f;
        switch (screenName)
        {
            case "Menu":
                menuScreen?.SetActive(true);
                break;
            case "Win":
                winScreen?.SetActive(true);
                break;
            case "Lose":
                loseScreen?.SetActive(true);
                break;
            default:
                Debug.LogWarning("Nombre de pantalla no reconocido: " + screenName);
                break;
        }
    }

    public void HideAllScreens()
    {
        menuScreen?.SetActive(false);
        winScreen?.SetActive(false);
        loseScreen?.SetActive(false);
        Time.timeScale = 1f;
    }

}
