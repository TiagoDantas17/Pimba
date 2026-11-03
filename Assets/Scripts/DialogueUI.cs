using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Barra de conversa (em cima). A conversa corre sozinha;
/// o jogador só faz scroll e lê.
/// Setup: ScrollRect > Viewport > Content (Vertical Layout Group + Content Size Fitter).
/// linePrefab = um TMP_Text simples.
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private TMP_Text linePrefab;
    [SerializeField] private float delayBetweenLines = 1.2f;
    [SerializeField] private string playerLabel = "Tu";
    [SerializeField] private bool autoScrollToBottom = true;

    private Coroutine routine;

    private void OnEnable()
    {
        if (GameManager.Instance != null) Subscribe();
        else StartCoroutine(WaitAndSubscribe());
    }

    private IEnumerator WaitAndSubscribe()
    {
        while (GameManager.Instance == null) yield return null;
        Subscribe();
    }

    private void Subscribe() => GameManager.Instance.OnArtistChanged += HandleArtist;

    private void OnDisable()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnArtistChanged -= HandleArtist;
    }

    private void HandleArtist(ArtistData artist)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(PlayDialogue(artist));
    }

    private IEnumerator PlayDialogue(ArtistData artist)
    {
        Clear();
        foreach (var line in artist.dialogue)
        {
            AddLine($"<b>{playerLabel}:</b> {line.question}");
            yield return new WaitForSeconds(delayBetweenLines);
            AddLine($"<b>{artist.artistName}:</b> {line.answer}");
            yield return new WaitForSeconds(delayBetweenLines);
        }
    }

    private void AddLine(string text)
    {
        var t = Instantiate(linePrefab, content);
        t.text = text;
        if (autoScrollToBottom) StartCoroutine(ScrollDown());
    }

    private IEnumerator ScrollDown()
    {
        yield return null; // espera o layout atualizar
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void Clear()
    {
        for (int i = content.childCount - 1; i >= 0; i--)
            Destroy(content.GetChild(i).gameObject);
    }
}