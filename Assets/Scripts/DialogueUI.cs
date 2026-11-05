using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Chat ÚNICO com scroll: as conversas antigas ficam guardadas e nunca fazem reset.
/// Cada candidato começa com um separador; depois da decisão o artista reage.
/// </summary>
public class DialogueUI : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform content;
    [SerializeField] private TMP_Text linePrefab;
    [SerializeField] private float delayBetweenLines = 1.2f;
    [SerializeField] private string playerLabel = "Tu";

    private const string PlayerColor = "#8FE3A8";
    private const string ArtistColor = "#FFFFFF";
    private const string ReactColor = "#FFD479";
    private const string SystemColor = "#7FA6C9";

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

    private void Subscribe()
    {
        GameManager.Instance.OnArtistChanged += HandleArtist;
        GameManager.Instance.OnDecisionMade += HandleDecision;
    }

    private void OnDisable()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnArtistChanged -= HandleArtist;
        GameManager.Instance.OnDecisionMade -= HandleDecision;
    }

    private void HandleArtist(ArtistData artist)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(PlayDialogue(artist));
    }

    private void HandleDecision(ArtistData artist, bool accepted, bool correct)
    {
        if (routine != null) { StopCoroutine(routine); routine = null; }

        AddLine($"<color={SystemColor}><i>({(accepted ? "Aceite" : "Recusado")})</i></color>");
        AddLine($"<color={ReactColor}><b>{artist.artistName}:</b></color> {ReactionLines.Get(artist, accepted)}");
    }

    private IEnumerator PlayDialogue(ArtistData artist)
    {
        AddLine($"<color={SystemColor}>— Novo candidato —</color>");
        yield return new WaitForSeconds(delayBetweenLines * 0.5f);

        foreach (var line in artist.dialogue)
        {
            AddLine($"<color={PlayerColor}><b>{playerLabel}:</b></color> {line.question}");
            yield return new WaitForSeconds(delayBetweenLines);
            AddLine($"<color={ArtistColor}><b>{artist.artistName}:</b></color> {line.answer}");
            yield return new WaitForSeconds(delayBetweenLines);
        }
    }

    private void AddLine(string text)
    {
        // Só desce sozinho se o jogador já estiver no fundo (para poder ler mensagens antigas)
        bool wasAtBottom = scrollRect.verticalNormalizedPosition <= 0.05f
                           || content.rect.height <= scrollRect.viewport.rect.height + 5f;

        var t = Instantiate(linePrefab, content);
        t.text = text;

        if (wasAtBottom) StartCoroutine(ScrollDown());
    }

    private IEnumerator ScrollDown()
    {
        yield return null; // espera o layout atualizar
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }
}