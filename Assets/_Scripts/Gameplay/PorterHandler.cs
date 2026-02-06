using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PorterHandler : MonoBehaviour, IInteractable
{
    [Header("Tutorial")]
    [SerializeField] public float delayCallPlayer = 4f;

    [Header("Dependencies")]
    [SerializeField] private GameController gameController;

    [Header("Interaction Indicators")]
    [SerializeField] private GameObject indicatorRoot;
    [SerializeField] private GameObject interrogateIndicator;
    [SerializeField] private Image interactionFill; // fill radial worldspace

    private void OnEnable() => StartCoroutine(DelayedWitnessing(delayCallPlayer));
    private IEnumerator DelayedWitnessing(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetInteractionIndicator(true);
    }

    public void OnInteracted()
    {
        gameController.OnPlayerAskPorter();
    }

    public void SetInteractionIndicator(bool show)
    {
        if (!indicatorRoot) return;
        indicatorRoot.SetActive(show);

        //interrogateIndicator.SetActive(isWitness);
        //accuseIndicator.SetActive(!isWitness);

        if (!show) return;
        //if (isWitnessAlready) indicatorRoot.SetActive(false);
    }

    public void SetInteractionProgress(float normalized)
    {
        if (interactionFill == null) return;

        interactionFill.fillAmount = Mathf.Clamp01(normalized);
        interactionFill.enabled = true;
    }

    public void ClearInteractionProgress()
    {
        if (interactionFill == null) return;

        interactionFill.fillAmount = 0f;
        interactionFill.enabled = false;
    }

    public Transform GetTransform() => transform;

    public InteractionType GetInteractionType()
    {
        return InteractionType.Cinematic;
    }

}
