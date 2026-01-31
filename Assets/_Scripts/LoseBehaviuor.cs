using UnityEngine;

public class LoseBehaviuor : MonoBehaviour
{
    void OnEnable()
    {
        Time.timeScale = 0f;
        Debug.Log("Juego detenido en pantalla de derrota.");
    }
}
