using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    [Header("mascara del asesino")]
    [SerializeField] public string KillerMaskId ;
    [Header("UI Manager")]
    [SerializeField] private UIManager UIManager;

    [Header("Renderers de la Máscara")]
    [SerializeField] private SuspectHandler suspectHandlerPrefab;

    [Header("Colecciones de Sprites")]
    [SerializeField] public List<Sprite> noseSprites;
    [SerializeField] public List<Sprite> eyeSprites;
    [SerializeField] public List<Sprite> mouthSprites;

    [Header("Reset")]
    [SerializeField] private KeyCode resetKey = KeyCode.Escape;

    void Start()
    {
        Time.timeScale = 0f;
        init();
    }

    void Update()
    {
        if (Input.GetKeyDown(resetKey))
        {
            StartCoroutine(ResetGame());
        }
    }

    IEnumerator EndTimer(float runTimer = 5f)
    {
        yield return new WaitForSeconds(runTimer);
        CheckWinCondition();
    }
    private void CheckWinCondition()
    {
        Debug.Log("Verificando condición de victoria...");
        UIManager.ShowScreen("Lose");
    }

    IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        init();
    }
    public void init()
    {
        UIManager.HideAllScreens();
        Time.timeScale = 1f;
        KillerMaskId = GenerateRandomMaskID();
        CreateKiller(KillerMaskId);
        CreateMaskedNpcs(6);
        StartCoroutine(EndTimer());
        Debug.Log("Juego iniciado.");
    }

    /// <summary>
    /// Genera un ID de máscara aleatorio que no coincide con el ID de la máscara del asesino si este ya esta definido.
    /// </summary>
    /// <returns></returns>
    public string GenerateRandomMaskID()
    {
        if (string.IsNullOrEmpty(KillerMaskId))
        {
            int eyeIndex = Random.Range(0, eyeSprites.Count);
            int noseIndex = Random.Range(0, noseSprites.Count);
            int mouthIndex = Random.Range(0, mouthSprites.Count);

            return $"{eyeIndex}{noseIndex}{mouthIndex}";
        }
        string newMaskID;
        do
        {
            int eyeIndex = Random.Range(0, eyeSprites.Count);
            int noseIndex = Random.Range(0, noseSprites.Count);
            int mouthIndex = Random.Range(0, mouthSprites.Count);

            newMaskID = $"{eyeIndex}{noseIndex}{mouthIndex}";
        } while (newMaskID == KillerMaskId);

        return newMaskID;

    }

    /// <summary>
    /// Crea una cantidad especificada de NPCs enmascarados distribuidos a lo largo del eje X,
    /// algunos en la parte superior y algunos en la parte inferior de la pantalla.
    /// </summary>
    public void CreateMaskedNpcs(int cantidad)
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("Main Camera no encontrada. No se pueden posicionar NPCs.");
            for (int i = 0; i < cantidad; i++)
            {
                string newMask = GenerateRandomMaskID();
                CreateMaskedNpc(newMask, Vector3.zero);
            }
            return;
        }

        float zDistance = Mathf.Abs(cam.transform.position.z);

        int topCount = (cantidad + 1) / 2;    // si es impar, top tiene uno más
        int bottomCount = cantidad / 2;

        float leftPad = 0.05f;
        float rightPad = 0.95f;
        float topY = 0.9f;
        float bottomY = 0.1f;

        // Distribuir top
        for (int i = 0; i < topCount; i++)
        {
            float t = topCount == 1 ? 0.5f : Mathf.Lerp(leftPad, rightPad, i / (float)(topCount - 1));
            Vector3 worldPos = cam.ViewportToWorldPoint(new Vector3(t, topY, zDistance));
            string newMask = GenerateRandomMaskID();
            CreateMaskedNpc(newMask, new Vector3(worldPos.x, worldPos.y, 0f));
        }

        // Distribuir bottom (de izquierda a derecha)
        for (int i = 0; i < bottomCount; i++)
        {
            float t = bottomCount == 1 ? 0.5f : Mathf.Lerp(leftPad, rightPad, i / (float)(bottomCount - 1));
            Vector3 worldPos = cam.ViewportToWorldPoint(new Vector3(t, bottomY, zDistance));
            string newMask = GenerateRandomMaskID();
            CreateMaskedNpc(newMask, new Vector3(worldPos.x, worldPos.y, 0f));
        }
    }

    public void CreateMaskedNpc(string mask, Vector3 position)
    {        
        int eyeIndex = int.Parse(mask[0].ToString());
        int noseIndex = int.Parse(mask[1].ToString());
        int mouthIndex = int.Parse(mask[2].ToString());
        
        SuspectHandler suspectObj = Instantiate(suspectHandlerPrefab.gameObject, position, Quaternion.identity).GetComponent<SuspectHandler>();
        suspectObj.gameController = this;
        suspectObj.ApplyMaskFromData(mask);
        Debug.Log("NPC creado con máscara: " + mask + " en " + position);
    }

    public void CreateKiller(string mask)
    {        
        int eyeIndex = int.Parse(mask[0].ToString());
        int noseIndex = int.Parse(mask[1].ToString());
        int mouthIndex = int.Parse(mask[2].ToString());
        
        SuspectHandler suspectObj = Instantiate(suspectHandlerPrefab.gameObject, Vector3.zero, Quaternion.identity).GetComponent<SuspectHandler>();
        suspectObj.gameController = GetComponent<GameController>();
        suspectObj.ApplyMaskFromData(mask);
        suspectObj.isKiller = true;
        Debug.Log("Asesino creado con máscara: " + mask);
    }
    
    
}