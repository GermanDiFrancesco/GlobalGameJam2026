using UnityEngine;
using System.Collections.Generic;

public class SuspectHandler : MonoBehaviour
{
    [Header("Dependencias")]
    [SerializeField] public GameController gameController;

    [Header("Mascara del asesino")]
    [SerializeField] public bool isKiller;

    [Header("Renderers de la Máscara")]
    [SerializeField] private SpriteRenderer nose;
    [SerializeField] private SpriteRenderer eyes;
    [SerializeField] private SpriteRenderer mouth;

    public void ApplyMaskFromData(string maskData)
    {

        int eyeIndex = int.Parse(maskData[0].ToString());
        int noseIndex = int.Parse(maskData[1].ToString());
        int mouthIndex = int.Parse(maskData[2].ToString());

        eyes.sprite = gameController.eyeSprites[eyeIndex];
        nose.sprite = gameController.noseSprites[noseIndex];
        mouth.sprite = gameController.mouthSprites[mouthIndex];
    }
      

}
