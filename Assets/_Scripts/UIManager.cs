using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private GameController gameController;

    [Header("Buttons")]
    [SerializeField] private Button comenzarButton;

    [Header("UI Screens")]
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

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

    public void OnStartGameButtonPressed()
    {
        HideAllScreens();
        Debug.Log("Juego iniciado.");        
    }

    public Sprite ShowWitnessClueSprite(Clue assignedClue)
    {
        return gameController.GetSprite(assignedClue.part);
    }

}
