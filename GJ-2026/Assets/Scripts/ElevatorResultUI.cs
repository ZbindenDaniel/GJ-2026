using System.Collections;
using TMPro;
using UnityEngine;

public class ElevatorResultUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageLabel;
    [SerializeField] private GameObject messageRoot;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float displaySeconds = 2.5f;

    private Coroutine hideRoutine;

    private void Awake()
    {
        if (messageLabel == null)
        {
            messageLabel = GetComponentInChildren<TMP_Text>(true);
        }

        if (messageRoot == null)
        {
            messageRoot = messageLabel != null ? messageLabel.gameObject : gameObject;
        }

        if (canvasGroup == null)
        {
            canvasGroup = messageRoot.GetComponent<CanvasGroup>();
        }

        SetVisible(false);
    }

    public void ShowResult(bool success, int targetLevel)
    {
        string message = success
            ? $"Correct! Going to level {targetLevel}."
            : $"Wrong elevator. Returning to level {targetLevel}.";

        ShowMessage(message);
    }

    public void ShowMessage(string message)
    {
        if (messageLabel == null)
        {
            Debug.LogWarning("ElevatorResultUI missing messageLabel.");
            return;
        }

        messageLabel.text = message;
        SetVisible(true);

        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
        }

        hideRoutine = StartCoroutine(HideAfterDelay(displaySeconds));
    }

    private IEnumerator HideAfterDelay(float delaySeconds)
    {
        yield return new WaitForSeconds(delaySeconds);
        SetVisible(false);
        hideRoutine = null;
    }

    private void SetVisible(bool visible)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
            return;
        }

        if (messageRoot != null && messageRoot != gameObject)
        {
            messageRoot.SetActive(visible);
            return;
        }

        if (messageLabel != null)
        {
            messageLabel.enabled = visible;
        }
    }
}
