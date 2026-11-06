using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Mostra o artista na cabine e liga os botões Aceitar / Recusar.
/// </summary>
public class ArtistViewUI : MonoBehaviour
{
    [Header("Artista (cabine)")]
    [SerializeField] private Image portraitImage;
    [Tooltip("Aumenta ou diminui a foto do artista (1 = tamanho normal da caixa).")]
    [SerializeField, Range(0.5f, 2.5f)] private float portraitScale = 1.3f;
    [Tooltip("Ligado: a foto não se deforma. Desligado: estica para encher a caixa.")]
    [SerializeField] private bool keepAspect = true;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Sprite placeholderHuman;
    [SerializeField] private Sprite placeholderRobot;
    [SerializeField] private Sprite placeholderImpostor;

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

        Sprite sprite = a.portrait;
        if (sprite == null) sprite = LoadPortrait(a);
        if (sprite == null) sprite = PlaceholderFor(a.kind);

        portraitImage.sprite = sprite;
        portraitImage.preserveAspect = keepAspect;
        portraitImage.rectTransform.localScale = Vector3.one * portraitScale;
        portraitImage.enabled = true;
        // Sem sprite: usa uma cor diferente por artista, para se ver a mudança
        portraitImage.color = sprite != null ? Color.white : PlaceholderColor(a);
    }

    /// <summary>
    /// Procura a foto em Assets/Resources/Portraits (ignora maiúsculas, acentos, espaços e "_").
    /// Se o artista tem portraitKey, usa SÓ a chave (assim o copycat nunca apanha a foto do original).
    /// </summary>
    private Sprite LoadPortrait(ArtistData a)
    {
        string wanted = !string.IsNullOrEmpty(a.portraitKey) ? a.portraitKey : a.artistName;
        var found = SpriteLibrary.Find("Portraits", wanted);
        if (found != null) return found;

        Debug.LogWarning($"[Pimba] Sem foto para '{a.artistName}'. Procurei '{wanted}' em Assets/Resources/Portraits. " +
                         $"Encontradas: {SpriteLibrary.Listing("Portraits")}. " +
                         "Confirma o nome e que a imagem é um Sprite (2D and UI).");
        return null;
    }

    private Sprite PlaceholderFor(ArtistKind k) => k switch
    {
        ArtistKind.Robot => placeholderRobot,
        ArtistKind.Impostor => placeholderImpostor,
        _ => placeholderHuman
    };

    private Color PlaceholderColor(ArtistData a)
    {
        float hue = (Mathf.Abs(a.artistName.GetHashCode()) % 100) / 100f;
        return Color.HSVToRGB(hue, 0.45f, 0.55f);
    }

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