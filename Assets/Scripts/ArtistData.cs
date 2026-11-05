using System;
using System.Collections.Generic;
using UnityEngine;

public enum ArtistKind { Human, Robot, Impostor } // Human = real | Robot = IA | Impostor = copycat

[Serializable]
public class DialogueLine
{
    [TextArea] public string question; // O que o jogador "pergunta" (automático)
    [TextArea] public string answer;   // Resposta do artista
}

/// <summary>
/// Dados de um artista. Podes criar assets em: Create > Pimba > Artista
/// ou deixar o GameManager gerar placeholders em runtime.
/// </summary>
[CreateAssetMenu(menuName = "Pimba/Artista", fileName = "NovoArtista")]
public class ArtistData : ScriptableObject
{
    [Header("Identidade")]
    public string artistName = "Artista";
    public Sprite portrait;               // Placeholder: pode ficar vazio
    public ArtistKind kind = ArtistKind.Human;

    [Header("Verdade (o jogador não vê isto)")]
    public bool isGenuine = true;         // true = artista real = deve ser aceite
    [TextArea] public string rejectReason = "Não é um artista real.";
    [TextArea] public string flawHint = ""; // Qual é a falha nos documentos (mostrada no fim)

    [Header("Portefólio")]
    public int age = 40;
    public string origin = "Lisboa";
    public string instrument = "Acordeão";
    public int careerYears = 10;
    public string famousSong = "A Minha Aldeia";
    [TextArea] public string notes = "Sem notas.";

    [Header("Cartão de cidadão")]
    public string birthDate = "01/01/1985";
    public string identityName;           // vazio = igual a artistName
    public string identityNumber = "12345678";

    [Header("Cartão de sócio")]
    public string memberName;             // vazio = igual a artistName
    public string memberNumber = "S-1000";

    [Header("Passaporte (quando aplicável)")]
    public bool hasPassport = false;
    public string passportName;           // vazio = igual a artistName
    public string passportNumber = "P1234567";
    public string passportCountry = "Portugal";

    [Header("Conversa")]
    public List<DialogueLine> dialogue = new List<DialogueLine>();

    public string IdName => string.IsNullOrEmpty(identityName) ? artistName : identityName;
    public string MemberName => string.IsNullOrEmpty(memberName) ? artistName : memberName;
    public string PassportName => string.IsNullOrEmpty(passportName) ? artistName : passportName;

    // ---------- Placeholders ----------
    static readonly string[] Origins = { "Lisboa", "Porto", "Braga", "Faro", "Coimbra", "Évora" };
    static readonly string[] Instruments = { "Guitarra", "Violino", "Acordeão", "Piano", "Bateria", "Voz" };
    static readonly string[] Songs = { "Maré Alta", "Rua Sem Fim", "Cais do Sol", "Noite Larga", "Fado Novo" };

    public static ArtistData CreatePlaceholder(string name, ArtistKind kind, bool genuine)
    {
        var a = CreateInstance<ArtistData>();
        a.name = name;
        a.artistName = name;
        a.kind = kind;
        a.isGenuine = genuine;

        int year = UnityEngine.Random.Range(1960, 1996);
        a.birthDate = $"{UnityEngine.Random.Range(1, 29):00}/{UnityEngine.Random.Range(1, 13):00}/{year}";
        a.age = 2026 - year;
        a.origin = Origins[UnityEngine.Random.Range(0, Origins.Length)];
        a.instrument = Instruments[UnityEngine.Random.Range(0, Instruments.Length)];
        a.careerYears = UnityEngine.Random.Range(3, 25);
        a.famousSong = Songs[UnityEngine.Random.Range(0, Songs.Length)];
        a.identityNumber = UnityEngine.Random.Range(10000000, 99999999).ToString();
        a.memberNumber = "S-" + UnityEngine.Random.Range(1000, 9999);
        a.hasPassport = UnityEngine.Random.value < 0.4f;
        a.passportNumber = "P" + UnityEngine.Random.Range(1000000, 9999999);

        if (!genuine)
        {
            a.rejectReason = kind == ArtistKind.Robot ? "Era uma IA." : "Era um copycat.";
            int flaw = UnityEngine.Random.Range(0, a.hasPassport ? 3 : 2);
            string mangled = name.Replace('a', 'o');
            if (mangled == name) mangled = name + "s";
            switch (flaw)
            {
                case 0:
                    a.memberName = mangled;
                    a.flawHint = "O nome no cartão de sócio era diferente.";
                    break;
                case 1:
                    a.age += UnityEngine.Random.Range(7, 15);
                    a.flawHint = "A idade no portefólio não batia com a data de nascimento.";
                    break;
                default:
                    a.passportName = mangled;
                    a.flawHint = "O nome no passaporte era diferente.";
                    break;
            }
        }

        string who = genuine ? "" : (kind == ArtistKind.Robot ? "Bip... quer dizer, " : "Hmm... ");
        a.dialogue = new List<DialogueLine>
        {
            new DialogueLine { question = "Como te chamas?", answer = "Chamo-me " + name + "." },
            new DialogueLine { question = "De onde és?", answer = who + "sou de " + a.origin + "." },
            new DialogueLine { question = "O que fazes em palco?", answer = who + "toco " + a.instrument + " há " + a.careerYears + " anos." },
        };
        return a;
    }
}