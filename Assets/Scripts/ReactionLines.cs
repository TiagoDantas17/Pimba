/// <summary>
/// Falas que o artista diz DEPOIS de o jogador decidir.
/// Muda os textos aqui à vontade.
/// </summary>
public static class ReactionLines
{
    // ----- NÃO PERMITIR (recusar) -----
    static readonly string[] RejectGenuine =
    {
        "Mas eu tenho tudo certo, eu recuso-me a usar ChatGPT...",
        "Eu sabia que esta empresa não me ia valorizar.",
        "Vocês estão a recusar-me à base do quê?",
    };
    static readonly string[] RejectImpostor =
    {
        "Bem me parecia que a peruca não era do tamanho certo...",
        "Mas eu juro que não sou um boneco de cera, digo, uma pessoa, digo...",
    };
    static readonly string[] RejectRobot =
    {
        "Bip Bip, Bop Bip",
    };

    // ----- PERMITIR (aceitar) -----
    static readonly string[] AcceptGenuine =
    {
        "Muito obrigada pela oportunidade, eu prometo que não vou desapontá-lo!",
        "Vê, você tem olho para o negócio, e sabe que eu sou a escolha certa.",
        "Ah... é só isto? Pensei que ia verificar as minhas biometrias também. Obrigado, obrigado.",
    };
    static readonly string[] AcceptImpostor =
    {
        "Afinal a tonalidade do batom não estava tão errada assim.",
        "Mas eu juro- Oh, vai-me deixar entrar... HAHAHAHA",
    };
    static readonly string[] AcceptRobot =
    {
        "Bip Bip, ChatGPT gera-me uma música sobre como enganar humanos.",
    };

    public static string Get(ArtistData a, bool accepted)
    {
        string[] pool;
        if (accepted)
            pool = a.isGenuine ? AcceptGenuine : (a.kind == ArtistKind.Robot ? AcceptRobot : AcceptImpostor);
        else
            pool = a.isGenuine ? RejectGenuine : (a.kind == ArtistKind.Robot ? RejectRobot : RejectImpostor);

        return pool[UnityEngine.Random.Range(0, pool.Length)];
    }
}