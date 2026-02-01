using UnityEngine;

public class LoseBehaviuor : MonoBehaviour
{

    [SerializeField] private UIManager _uiManager;
    void OnEnable()
    {
        Time.timeScale = 0f;
        Debug.Log("Juego detenido en pantalla.");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) _uiManager.OnStartGameButtonPressed();
    }
}
