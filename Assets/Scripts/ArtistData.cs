using System;
using System.Collections.Generic;
using UnityEngine;

public enum ArtistKind { Human, Robot, Impostor }

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
    public bool isPimba = true;           // true = deve ser aceite
    [TextArea] public string rejectReason = "Não é Pimba."; // Mostrado no fim, se errares

    [Header("Painel de informação (parte de baixo)")]
    public int age = 40;
    public string origin = "Alentejo";
    public string instrument = "Acordeão";
    public int careerYears = 10;
    public string famousSong = "A Minha Aldeia";
    [TextArea] public string notes = "Sem notas.";

    [Header("Conversa (barra de cima)")]
    public List<DialogueLine> dialogue = new List<DialogueLine>();

    // ---------- Placeholders ----------
    public static ArtistData CreatePlaceholder(string name, ArtistKind kind, bool pimba)
    {
        var a = CreateInstance<ArtistData>();
        a.name = name;
        a.artistName = name;
        a.kind = kind;
        a.isPimba = pimba;
        a.rejectReason = pimba ? "" : "Placeholder: não é Pimba.";
        a.instrument = pimba ? "Acordeão" : (kind == ArtistKind.Robot ? "Sintetizador" : "Guitarra elétrica");
        a.famousSong = pimba ? "O Meu Amor É Um Tractor" : "Heavy Metal Forever";
        a.dialogue = new List<DialogueLine>
        {
            new DialogueLine { question = "Como te chamas?", answer = "Chamo-me " + name + "." },
            new DialogueLine { question = "Que tipo de música fazes?", answer = pimba ? "Pimba, claro! Do coração." : "Hmm... rock progressivo experimental." },
            new DialogueLine { question = "Tocas algum instrumento?", answer = "Toco " + a.instrument + "." },
        };
        return a;
    }
}