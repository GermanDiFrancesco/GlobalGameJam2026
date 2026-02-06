using System.Collections;
using UnityEngine;

public class PorterHandler : MonoBehaviour
{
    [Header("Tutorial")]
    [SerializeField] public float delayWitnessing;

    [Header("Dependencies")]
    [SerializeField] private GameController gameController;

    [Header("Interaction Indicators")]
    [SerializeField] private GameObject speechRoot;
    [SerializeField] private GameObject indicatorRoot;
    [SerializeField] private GameObject accuseIndicator;
    [SerializeField] private GameObject interrogateIndicator;

    private void Start()
    {
        delayWitnessing = Random.Range(3f, 20f);
        StartCoroutine(DelayedWitnessing(delayWitnessing));
    }

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

}
