using UnityEngine;
using System.Collections.Generic;

public class GameController : MonoBehaviour
{
    [Header("mascara del asesino")]
    [SerializeField] public string KillerMaskId ;

    [Header("Renderers de la Máscara")]
    [SerializeField] private SuspectHandler suspectHandlerPrefab;

    [Header("Colecciones de Sprites")]
    [SerializeField] public List<Sprite> noseSprites;
    [SerializeField] public List<Sprite> eyeSprites;
    [SerializeField] public List<Sprite> mouthSprites;

    void Start()
    {
        KillerMaskId = GenerateRandomMaskID();
        CreateKiller(KillerMaskId);
        CreateMaskedNpcs(2);
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
    /// Crea una cantidad especificada de NPCs enmascarados.
    public void CreateMaskedNpcs(int cantidad)
    {
        for (int i = 0; i < cantidad; i++)
        {
            string newMask = GenerateRandomMaskID();
            CreateMaskedNpc(newMask);
        }
    }
    public void CreateMaskedNpc(string mask)
    {        
        int eyeIndex = int.Parse(mask[0].ToString());
        int noseIndex = int.Parse(mask[1].ToString());
        int mouthIndex = int.Parse(mask[2].ToString());
        
        SuspectHandler suspectObj = Instantiate(suspectHandlerPrefab.gameObject, Vector3.zero, Quaternion.identity).GetComponent<SuspectHandler>();
        suspectObj.gameController = this;
        suspectObj.ApplyMaskFromData(mask);
        Debug.Log("NPC creado con máscara: " + mask);
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