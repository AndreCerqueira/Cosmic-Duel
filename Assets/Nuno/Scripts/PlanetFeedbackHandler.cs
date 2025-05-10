using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using MoreMountains.Feedbacks;

public class PlanetFeedbackHandler : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Feedbacks")]
    [SerializeField] private MMF_Player clickFeedback;
    [SerializeField] private MMF_Player pointerEnterFeedback;
    [SerializeField] private MMF_Player pointerExitFeedback;

    [Header("Auto-enter")]
    [SerializeField] private bool autoPointerEnterFeedback;
    [SerializeField] private float autoPointerEnterFeedbackDelay = 1f;

    [Header("Estado")]
    [SerializeField] private bool interactable = true;      // se precisares de bloquear em runtime

    public bool Interactable
    {
        get => interactable;
        set => interactable = value;
    }

    /* ---------- Unity ---------- */
    private void OnEnable()
    {
        if (autoPointerEnterFeedback)
            StartCoroutine(AutoPointerEnterFeedbackCoroutine());
    }

    private void OnDisable() => StopAllCoroutines();

    /* ---------- IPointer ---------- */
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!interactable) return;
        pointerEnterFeedback?.PlayFeedbacks();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!interactable) return;
        pointerExitFeedback?.PlayFeedbacks();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!interactable) return;
        clickFeedback?.PlayFeedbacks();
    }

    /* ---------- Auto-loop ---------- */
    private IEnumerator AutoPointerEnterFeedbackCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(autoPointerEnterFeedbackDelay);
            if (!interactable) continue;
            pointerEnterFeedback?.PlayFeedbacks();
        }
    }
}
