using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// O elenco do jogo, POR ESTA ORDEM. É usado quando a lista Artists do GameManager está vazia.
/// Datas, números e países são valores de teste: muda-os à vontade.
/// Para pores imagens: cria assets (Create > Pimba > Artista) e arrasta-os para o GameManager.
/// </summary>
public static class ArtistRoster
{
    public static List<ArtistData> Build()
    {
        var list = new List<ArtistData>();

        // ================= REAIS =================

        // 1. Tio Canções
        var tio = Make("Tio Canções", ArtistKind.Human, true);
        tio.birthDate = "14/03/1962"; tio.identityNumber = "11223344"; tio.validUntil = "03/2031";
        Say(tio, "Bom dia!");
        Say(tio, "Vim apresentar-me, tome, tome.");
        Give(tio, HandoutKind.Identity);
        Give(tio, HandoutKind.Media, "CD do Tio Canções");
        Give(tio, HandoutKind.Autograph);
        list.Add(tio);

        // 2. Quim das Couves
        var quim = Make("Quim das Couves", ArtistKind.Human, true);
        quim.birthDate = "02/09/1958"; quim.identityNumber = "22334455"; quim.validUntil = "09/2030";
        Say(quim, "Ora, com licença.");
        Say(quim, "Espero que esteja tudo ajeitado!");
        Give(quim, HandoutKind.Identity);
        Give(quim, HandoutKind.Media, "Cassete do Quim das Couves");
        Give(quim, HandoutKind.Autograph);
        list.Add(quim);

        // 3. Azulhos
        var azulhos = Make("Azulhos", ArtistKind.Human, true);
        azulhos.birthDate = "23/06/1991"; azulhos.identityNumber = "33445566"; azulhos.validUntil = "06/2032";
        Say(azulhos, "Olá, olá!");
        Say(azulhos, "Obrigada pela oportunidade, acho que está aqui tudo o que me pediu.");
        Give(azulhos, HandoutKind.Identity);
        Give(azulhos, HandoutKind.Media, "Slopify do Azulhos");
        Give(azulhos, HandoutKind.Autograph);
        list.Add(azulhos);

        // 4. Carol Descendentes
        var carol = Make("Carol Descendentes", ArtistKind.Human, true);
        carol.birthDate = "11/11/1997"; carol.identityNumber = "44556677"; carol.validUntil = "11/2033";
        Say(carol, "Hey! Agradeço por poder vir aqui");
        Say(carol, "Deveria entregar mais alguma coisa?");
        Give(carol, HandoutKind.Identity);
        Give(carol, HandoutKind.Media, "Slopify da Carol Descendentes");
        Give(carol, HandoutKind.Autograph);
        list.Add(carol);

        // 5. Aquário (sem cartão de cidadão: entrega passaporte)
        var aquario = Make("Aquário", ArtistKind.Human, true);
        aquario.passportNumber = "P4821907"; aquario.passportCountry = "Reino Unido";
        Say(aquario, "Oi mate!");
        Say(aquario, "Glad to be here, here's everything. I don't have a Cartão de cidadão, so here's my passport!");
        Give(aquario, HandoutKind.Passport);
        Give(aquario, HandoutKind.Media, "CD de Aquário");
        Give(aquario, HandoutKind.Autograph);
        list.Add(aquario);

        // ================= COPYCAT =================

        // 6. Tio Canções (falso): cartão de cidadão caducado + crachá VIP
        var copy = Make("Tio Canções", ArtistKind.Impostor, false);
        copy.birthDate = "14/03/1962"; copy.identityNumber = "11223348"; copy.validUntil = "03/2021"; // caducado
        copy.rejectReason = "Era um copycat do Tio Canções.";
        copy.flawHint = "O cartão de cidadão estava caducado e trouxe um crachá VIP, que um artista verdadeiro não entrega.";
        Say(copy, "Ah, olá…");
        Say(copy, "uhm, está aqui o que pediu!");
        Give(copy, HandoutKind.Identity);
        Give(copy, HandoutKind.Media, "CD do Tio Canções");
        Give(copy, HandoutKind.VipBadge);
        list.Add(copy);

        // ================= IAs =================

        // 7. Tugatron_1: documentos de outra pessoa + autógrafo em binário
        var t1 = Make("Tugatron_1", ArtistKind.Robot, false);
        t1.identityName = "Marta Silva"; t1.birthDate = "22/08/1989"; t1.identityNumber = "30415582"; t1.validUntil = "09/2030";
        t1.portfolioName = "Marta Silva"; t1.age = 37; t1.origin = "Braga"; t1.instrument = "Violino"; t1.careerYears = 12;
        t1.rejectReason = "Era uma IA.";
        t1.flawHint = "Os documentos eram de outra pessoa (Marta Silva) e o autógrafo estava em binário.";
        Say(t1, "Ol-a");
        Give(t1, HandoutKind.Identity);
        Give(t1, HandoutKind.Portfolio);
        // "Tugatron" em binário (ASCII)
        Give(t1, HandoutKind.Autograph, null, "01010100 01110101 01100111 01100001 01110100 01110010 01101111 01101110");
        list.Add(t1);

        // 8. Tugatron_2: não fala; documentos que não batem certo + crachá VIP
        var t2 = Make("Tugatron_2", ArtistKind.Robot, false);
        t2.identityName = "Rui Costa"; t2.birthDate = "05/11/1975"; t2.identityNumber = "27719046"; t2.validUntil = "03/2031";
        t2.passportName = "Ruy Costa"; t2.passportNumber = "P0000000"; t2.passportCountry = "Portugal";
        t2.rejectReason = "Era uma IA.";
        t2.flawHint = "Os nomes nos documentos não eram os do artista e o passaporte tinha um nome diferente do cartão de cidadão.";
        Say(t2, "...");
        Give(t2, HandoutKind.Identity);
        Give(t2, HandoutKind.Passport);
        Give(t2, HandoutKind.VipBadge);
        list.Add(t2);

        return list;
    }

    // ---------------- helpers ----------------
    static ArtistData Make(string name, ArtistKind kind, bool genuine)
    {
        var a = ScriptableObject.CreateInstance<ArtistData>();
        a.name = name;
        a.artistName = name;
        a.kind = kind;
        a.isGenuine = genuine;
        a.handouts = new List<Handout>();
        a.dialogue = new List<DialogueLine>();
        return a;
    }

    static void Say(ArtistData a, string line)
    {
        a.dialogue.Add(new DialogueLine { answer = line });
    }

    static void Give(ArtistData a, HandoutKind kind, string label = null, string text = null)
    {
        a.handouts.Add(new Handout { kind = kind, label = label, text = text });
    }
}