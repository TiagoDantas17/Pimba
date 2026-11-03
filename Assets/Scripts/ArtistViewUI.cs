using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mostra o artista (lado direito) e o painel de informação (em baixo).
/// Também liga os botões Aceitar / Recusar.
/// </summary>
public class ArtistViewUI : MonoBehaviour
{
    [Header("Artista (direita)")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Sprite placeholderHuman;
    [SerializeField] private Sprite placeholderRobot;
    [SerializeField] private Sprite placeholderImpostor;

    [Header("Painel de informação (baixo)")]
    [SerializeField] private TMP_Text infoText;

    [Header("Botões")]
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button rejectButton;

    [Header("Progresso (opcional)")]
    [SerializeField] private TMP_Text progressText;

    private void OnEnable() => StartCoroutine(Init());

    private IEnumerator Init()
    {
        while (GameManager.Instance == null) yield return null;
        var gm = GameManager.Instance;
        gm.OnArtistChanged += ShowArtist;
        gm.OnProgress += ShowProgress;
        gm.OnDecisionsEnabled += SetButtons;

        acceptButton.onClick.AddListener(() => gm.Decide(true));
        rejectButton.onClick.AddListener(() => gm.Decide(false));
    }

    private void OnDisable()
    {
        acceptButton.onClick.RemoveAllListeners();
        rejectButton.onClick.RemoveAllListeners();
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnArtistChanged -= ShowArtist;
        GameManager.Instance.OnProgress -= ShowProgress;
        GameManager.Instance.OnDecisionsEnabled -= SetButtons;
    }

    private void ShowArtist(ArtistData a)
    {
        nameText.text = a.artistName;
        portraitImage.sprite = a.portrait != null ? a.portrait : PlaceholderFor(a.kind);
        portraitImage.enabled = portraitImage.sprite != null;
        portraitImage.color = portraitImage.sprite == null ? Color.gray : Color.white;

        infoText.text =
            $"<b>Nome:</b> {a.artistName}\n" +
            $"<b>Idade:</b> {a.age}\n" +
            $"<b>Origem:</b> {a.origin}\n" +
            $"<b>Instrumento:</b> {a.instrument}\n" +
            $"<b>Anos de carreira:</b> {a.careerYears}\n" +
            $"<b>Música mais famosa:</b> {a.famousSong}\n" +
            $"<b>Notas:</b> {a.notes}";
    }

    private Sprite PlaceholderFor(ArtistKind k) => k switch
    {
        ArtistKind.Robot => placeholderRobot,
        ArtistKind.Impostor => placeholderImpostor,
        _ => placeholderHuman
    };

    private void ShowProgress(int cur, int total)
    {
        if (progressText != null) progressText.text = $"{cur}/{total}";
    }

    private void SetButtons(bool enabled)
    {
        acceptButton.interactable = enabled;
        rejectButton.interactable = enabled;
    }
}