using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

[System.Serializable]
public class MaskPart
{
    public string partType;
    public List<Sprite> partSprites;
}
public class GameController : MonoBehaviour
{
    [Header("Debug Info")]
    [SerializeField] public string KillerMaskId;

    [SerializeField] public List<MaskPart> maskPartList;
    
    [Header("Dependencies")]
    [SerializeField] private UIManager UIManager;

    [Header("Prefabs")]
    [SerializeField] private SuspectHandler suspectHandlerPrefab;

    [Header("Resources")]
    [SerializeField] public List<Sprite> HatSprites;
    [SerializeField] public List<Sprite> AntifaceSprites;
    [SerializeField] public List<Sprite> AccesorySprites;

    [Header("Key Bindings")]
    [SerializeField] private KeyCode resetKey = KeyCode.Escape;

    void Awake()
    {
        Time.timeScale = 0f;
    }

    void Start()
    {
        StartGame();
        GenerateClueList();
    }

    void Update()
    {
        if (Input.GetKeyDown(resetKey))
        {
            StartCoroutine(ResetGame());
        }
    }

    public void StartGame()
    {
        KillerMaskId = GenerateRandomMaskID();
        CreateKiller(KillerMaskId);
        CreateMaskedNpcs(12);
        StartCoroutine(StartCountdown());
    }
    IEnumerator ResetGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        yield return null;
    }

    IEnumerator StartCountdown(float runTimer = 5f)
    {
        yield return new WaitForSeconds(runTimer);
        WinCondition(false);
    }
    

    /// <summary>
    /// Genera un ID de máscara aleatorio que no coincide con el ID de la máscara del asesino si este ya esta definido.
    /// </summary>
    /// <returns></returns>
    public string GenerateRandomMaskID()
    {
        if (string.IsNullOrEmpty(KillerMaskId))
        {
            int eyeIndex = Random.Range(0, AntifaceSprites.Count);
            int noseIndex = Random.Range(0, HatSprites.Count);
            int mouthIndex = Random.Range(0, AccesorySprites.Count);

            return $"{eyeIndex}{noseIndex}{mouthIndex}";
        }
        string newMaskID;
        do
        {
            int eyeIndex = Random.Range(0, AntifaceSprites.Count);
            int noseIndex = Random.Range(0, HatSprites.Count);
            int mouthIndex = Random.Range(0, AccesorySprites.Count);

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

        // Configuración en píxeles
        float spacingPx = 250f; // cada 250px en X
        float topScreenY = Screen.height * 0.8f;
        float bottomScreenY = Screen.height * 0.2f;

        float topStartX = 0f;
        float bottomStartX = 0f;

        // Distribuir top (de izquierda a derecha, cada 250px)
        for (int i = 0; i < topCount; i++)
        {
            float screenX = topStartX + i * spacingPx;
            Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenX, topScreenY, zDistance));
            string newMask = GenerateRandomMaskID();
            CreateMaskedNpc(newMask, new Vector3(worldPos.x, worldPos.y, 0f));
        }

        // Distribuir bottom (de izquierda a derecha, cada 250px)
        for (int i = 0; i < bottomCount; i++)
        {
            float screenX = bottomStartX + i * spacingPx;
            Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(screenX, bottomScreenY, zDistance));
            string newMask = GenerateRandomMaskID();
            CreateMaskedNpc(newMask, new Vector3(worldPos.x, worldPos.y, 0f));
        }

        Debug.Log(cantidad + " NPCs enmascarados creados (espaciados " + spacingPx + "px en X).");
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
    
    public void AccusedKiller(string accusedID)
    {
        if (accusedID == KillerMaskId)    
        {
            WinCondition(true);
        }
        else
        {
            WinCondition(false);
        }
    }

    private void WinCondition(bool condition)
    {
        if (condition)
        {
            UIManager.ShowScreen("Win");
        }
        else
        {
            UIManager.ShowScreen("Lose");
        }
    }

    private void GenerateClueList()
    {
        var clueIndex = new List<MaskPart>();
        
        clueIndex.Add(maskPartList[0]);

        /*
        clueIndex.Add(KillerMaskId[0]);
        clueIndex.Add(KillerMaskId[0]);        
        int killerEyeIndex = int.Parse(KillerMaskId[0].ToString());
        for (int i = 0; i < AntifaceSprites.Count; i++)
        {
            if (i == killerEyeIndex) continue;
            clueIndex.Add(i.ToString()[0]);
        }
        
        
        Debug.Log("Clue Eye Index: " + new string(clueIndex.ToArray()));
        */
    }

    // Minimo pistas para ganar = Index[0].length *2
    // Maximo pistas para ganar = Index[0].length + Index[1].length + Index[2].length * 2
        
}