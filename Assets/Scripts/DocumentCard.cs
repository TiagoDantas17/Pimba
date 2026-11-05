using TMPro;
using UnityEngine;

public enum DocType { Identity, Member, Passport, Portfolio }

/// <summary>
/// Um cartão/documento na mesa. Preenche o texto a partir do ArtistData.
/// O passaporte só aparece quando o artista tem passaporte.
/// </summary>
public class DocumentCard : MonoBehaviour
{
    [SerializeField] private DocType type;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;

    private RectTransform rect;
    private Vector2 spawnPos;
    private bool captured;

    private void Capture()
    {
        if (captured) return;
        rect = (RectTransform)transform;
        spawnPos = rect.anchoredPosition;
        captured = true;
    }

    private void Awake() => Capture();

    public void Show(ArtistData a)
    {
        Capture();

        if (type == DocType.Passport && !a.hasPassport)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);

        // Volta ao sítio inicial com um pequeno desvio, como se tivessem acabado de ser entregues
        rect.anchoredPosition = spawnPos + new Vector2(Random.Range(-20f, 20f), Random.Range(-20f, 20f));
        rect.localEulerAngles = new Vector3(0, 0, Random.Range(-4f, 4f));

        switch (type)
        {
            case DocType.Identity:
                titleText.text = "CARTÃO DE CIDADÃO";
                bodyText.text = $"<b>Nome:</b> {a.IdName}\n<b>Nascimento:</b> {a.birthDate}\n<b>Nº:</b> {a.identityNumber}";
                break;
            case DocType.Member:
                titleText.text = "CARTÃO DE SÓCIO";
                bodyText.text = $"<b>Nome:</b> {a.MemberName}\n<b>Nº de sócio:</b> {a.memberNumber}";
                break;
            case DocType.Passport:
                titleText.text = "PASSAPORTE";
                bodyText.text = $"<b>Nome:</b> {a.PassportName}\n<b>País:</b> {a.passportCountry}\n<b>Nº:</b> {a.passportNumber}";
                break;
            default:
                titleText.text = "PORTEFÓLIO";
                bodyText.text = $"<b>Idade:</b> {a.age}\n<b>Origem:</b> {a.origin}\n<b>Instrumento:</b> {a.instrument}\n" +
                                $"<b>Carreira:</b> {a.careerYears} anos\n<b>Êxito:</b> {a.famousSong}\n<b>Notas:</b> {a.notes}";
                break;
        }
    }
}
