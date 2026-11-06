using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Um cartão/objeto genérico na mesa. O DeskController diz-lhe o que mostrar
/// (cartão de cidadão, passaporte, CD, autógrafo, crachá VIP...).
/// </summary>
public class DocumentCard : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;

    private RectTransform rect;
    private Image image;
    private Vector2 spawnPos;
    private bool captured;

    private static readonly Color IdentityColor = new Color(0.85f, 0.87f, 0.80f);
    private static readonly Color PassportColor = new Color(0.55f, 0.70f, 0.85f);
    private static readonly Color PortfolioColor = new Color(0.93f, 0.90f, 0.78f);
    private static readonly Color MediaColor = new Color(0.90f, 0.80f, 0.60f);
    private static readonly Color AutographColor = new Color(0.98f, 0.97f, 0.92f);
    private static readonly Color VipColor = new Color(0.90f, 0.75f, 0.25f);

    private void Capture()
    {
        if (captured) return;
        rect = (RectTransform)transform;
        image = GetComponent<Image>();
        spawnPos = rect.anchoredPosition;
        captured = true;
    }

    private void Awake() => Capture();

    public void Hide()
    {
        Capture();
        gameObject.SetActive(false);
    }

    public void Show(ArtistData a, Handout h)
    {
        Capture();
        gameObject.SetActive(true);

        // Volta ao sítio inicial com um pequeno desvio, como se tivessem acabado de ser entregues
        rect.anchoredPosition = spawnPos + new Vector2(UnityEngine.Random.Range(-20f, 20f), UnityEngine.Random.Range(-20f, 20f));
        rect.localEulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(-4f, 4f));

        bodyText.enableAutoSizing = true;
        bodyText.fontSizeMin = 16;
        bodyText.fontSizeMax = 28;
        bodyText.alignment = TextAlignmentOptions.TopLeft;

        // texto automático, a não ser que o Handout tenha um texto próprio
        string Custom(string auto) => string.IsNullOrEmpty(h.text) ? auto : h.text;

        Color bg;
        string title;
        string body;

        switch (h.kind)
        {
            case HandoutKind.Identity:
                bg = IdentityColor;
                title = "CARTÃO DE CIDADÃO";
                body = Custom($"<b>Nome:</b> {a.IdName}\n<b>Nascimento:</b> {a.birthDate}\n<b>Válido até:</b> {a.validUntil}\n<b>Nº:</b> {a.identityNumber}");
                break;

            case HandoutKind.Passport:
                bg = PassportColor;
                title = "PASSAPORTE";
                body = Custom($"<b>Nome:</b> {a.PassportName}\n<b>País:</b> {a.passportCountry}\n<b>Nº:</b> {a.passportNumber}");
                break;

            case HandoutKind.Portfolio:
                bg = PortfolioColor;
                title = "PORTEFÓLIO";
                string auto = $"<b>Nome:</b> {a.PortfolioName}\n<b>Idade:</b> {a.age}";
                if (!string.IsNullOrEmpty(a.origin)) auto += $"\n<b>Origem:</b> {a.origin}";
                if (!string.IsNullOrEmpty(a.instrument)) auto += $"\n<b>Instrumento:</b> {a.instrument}";
                auto += $"\n<b>Carreira:</b> {a.careerYears} anos";
                if (!string.IsNullOrEmpty(a.famousSong)) auto += $"\n<b>Êxito:</b> {a.famousSong}";
                if (!string.IsNullOrEmpty(a.notes)) auto += $"\n<b>Notas:</b> {a.notes}";
                body = Custom(auto);
                break;

            case HandoutKind.Media:
                bg = MediaColor;
                title = string.IsNullOrEmpty(h.label) ? "ÁLBUM" : h.label.ToUpperInvariant();
                body = Custom($"<b>Artista:</b> {a.artistName}");
                break;

            case HandoutKind.Autograph:
                bg = AutographColor;
                title = "AUTÓGRAFO";
                bodyText.alignment = TextAlignmentOptions.Center;
                body = $"<i>{(string.IsNullOrEmpty(h.text) ? a.artistName : h.text)}</i>";
                break;

            default: // VipBadge
                bg = VipColor;
                title = "CRACHÁ VIP";
                bodyText.alignment = TextAlignmentOptions.Center;
                body = Custom($"<size=170%><b>VIP</b></size>\n{a.artistName}");
                break;
        }

        if (image != null)
        {
            image.sprite = h.sprite;                       // sem sprite = bloco de cor
            image.color = h.sprite != null ? Color.white : bg;
        }
        titleText.text = title;
        bodyText.text = body;
    }
}