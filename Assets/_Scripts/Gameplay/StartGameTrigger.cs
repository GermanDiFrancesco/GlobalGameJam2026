using UnityEngine;

public class StartGameTrigger : MonoBehaviour
{
    [Header("Flag")]
    public bool hasEntered = false;
    
    [Header("Dependencies")]
    [SerializeField] GameController gameController;
    [SerializeField] GameObject entrance;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !hasEntered)
        {
            Debug.Log("Exit Tutorial Area - Starting Game");
            gameController.StartGame();
            entrance.SetActive(true);
        }
    }
}
