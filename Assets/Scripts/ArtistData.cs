using System;
using System.Collections.Generic;
using UnityEngine;

public enum ArtistKind { Human, Robot, Impostor } // Human = real | Robot = IA | Impostor = copycat

public enum HandoutKind
{
    Identity,   // Cartão de cidadão
    Passport,   // Passaporte
    Portfolio,  // Portefólio
    Media,      // CD / Cassete / Slopify (usa "label" para o nome)
    Autograph,  // Autógrafo (usa "text" para a assinatura)
    VipBadge    // Crachá VIP
}

[Serializable]
public class DialogueLine
{
    [TextArea] public string question; // OPCIONAL: se vazio, só fala o artista
    [TextArea] public string answer;   // O que o artista diz
}

/// <summary>
/// Um objeto que o artista entrega e que aparece na mesa.
/// </summary>
[Serializable]
public class Handout
{
    public HandoutKind kind = HandoutKind.Identity;

    [Tooltip("Só para Media: nome do objeto, ex.: 'CD do Tio Canções'.")]
    public string label;

    [TextArea, Tooltip("Opcional. Autógrafo: a assinatura. Outros: substitui o texto automático do cartão.")]
    public string text;

    [Tooltip("Imagem própria deste objeto (opcional).")]
    public Sprite sprite;

    /// <summary>Como o objeto é dito na linha "(entrega ...)".</summary>
    public string SpokenName()
    {
        switch (kind)
        {
            case HandoutKind.Identity: return "cartão de cidadão";
            case HandoutKind.Passport: return "passaporte";
            case HandoutKind.Portfolio: return "portefólio";
            case HandoutKind.Media: return string.IsNullOrEmpty(label) ? "álbum" : label;
            case HandoutKind.Autograph: return "autógrafo";
            default: return "crachá VIP";
        }
    }
}

/// <summary>
/// Dados de um artista. Podes criar assets em: Create > Pimba > Artista.
/// Se a lista Artists do GameManager estiver vazia, o jogo usa o elenco do ArtistRoster.cs.
/// </summary>
[CreateAssetMenu(menuName = "Pimba/Artista", fileName = "NovoArtista")]
public class ArtistData : ScriptableObject
{
    [Header("Identidade")]
    public string artistName = "Artista";
    public Sprite portrait;               // Imagem do artista na cabine (assets criados à mão)
    [Tooltip("Nome do ficheiro em Assets/Resources/Portraits/ (sem .png). Usado se Portrait estiver vazio.")]
    public string portraitKey;
    [Tooltip("Ajuste só para este artista, por cima do Portrait Scale geral. 1 = igual aos outros; 0.6 = mais pequeno.")]
    [Range(0.2f, 2f)] public float portraitScaleMultiplier = 1f;
    [Range(0.2f, 2f), Tooltip("Tamanho só deste artista (multiplica o Portrait Scale geral). 1 = normal; menos que 1 = mais pequeno.")]
    public float portraitSize = 1f;
    public ArtistKind kind = ArtistKind.Human;

    [Header("Verdade (o jogador não vê isto)")]
    public bool isGenuine = true;         // true = artista real = deve ser aceite
    [TextArea] public string rejectReason = "Não é um artista real.";
    [TextArea] public string flawHint = ""; // A falha (mostrada no fim se deixares entrar)

    [Header("O que entrega (por ordem)")]
    public List<Handout> handouts = new List<Handout>
    {
        new Handout { kind = HandoutKind.Identity },
        new Handout { kind = HandoutKind.Autograph },
    };

    [Header("Cartão de cidadão")]
    public string identityName;           // vazio = igual a artistName
    public string birthDate = "01/01/1985";
    public string validUntil = "03/2031";
    public string identityNumber = "12345678";

    [Header("Passaporte")]
    public string passportName;           // vazio = igual a artistName
    public string passportNumber = "P1234567";
    public string passportCountry = "Portugal";

    [Header("Portefólio")]
    public string portfolioName;          // vazio = igual a artistName
    public int age = 40;
    public string origin = "";
    public string instrument = "";
    public int careerYears = 10;
    public string famousSong = "";        // opcional
    [TextArea] public string notes = "";  // opcional

    [Header("Conversa (o que diz ao chegar)")]
    public List<DialogueLine> dialogue = new List<DialogueLine>();

    public string IdName => string.IsNullOrEmpty(identityName) ? artistName : identityName;
    public string PassportName => string.IsNullOrEmpty(passportName) ? artistName : passportName;
    public string PortfolioName => string.IsNullOrEmpty(portfolioName) ? artistName : portfolioName;

    /// <summary>Ex.: "(entrega cartão de cidadão, CD do Tio Canções e autógrafo)"</summary>
    public string DeliveryText()
    {
        if (handouts == null || handouts.Count == 0) return "";

        var names = new List<string>();
        foreach (var h in handouts) names.Add(h.SpokenName());

        string joined = names.Count == 1
            ? names[0]
            : string.Join(", ", names.GetRange(0, names.Count - 1)) + " e " + names[names.Count - 1];

        return "(entrega " + joined + ")";
    }
}