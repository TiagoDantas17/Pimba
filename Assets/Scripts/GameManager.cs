using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GameResult
{
    public int total;
    public int correct;
    public int wrong;
    public bool fired;                 // true = o chefe despede-te
    public List<string> mistakes;      // Nomes + motivo dos erros
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Artistas")]
    [Tooltip("Se ficar vazio, são gerados artistas placeholder.")]
    [SerializeField] private List<ArtistData> artists = new List<ArtistData>();
    [SerializeField] private int placeholderCount = 8;
    [SerializeField] private bool shuffle = true;

    [Header("Regras")]
    [SerializeField] private int maxMistakesBeforeFired = 3;
    [SerializeField] private float delayBetweenArtists = 2.5f;   // tempo para ler a reação
    [Tooltip("Se false, o jogo só começa quando chamares StartGame() (ex.: botão do ecrã de intro).")]
    [SerializeField] private bool startAutomatically = true;

    // Eventos para a UI ouvir
    public event Action<ArtistData> OnArtistChanged;
    public event Action<int, int> OnProgress;                    // atual, total
    public event Action<ArtistData, bool, bool> OnDecisionMade;  // artista, aceitou?, acertou?
    public event Action<GameResult> OnGameEnded;
    public event Action<bool> OnDecisionsEnabled;                // ativar/desativar botões

    public ArtistData Current => current;

    private static readonly string[] PlaceholderNames =
    {
        "Zé Maria", "Ana Luz", "Rui Costa", "Marta Silva", "Tiago Pires",
        "Inês Faria", "Carlos Mota", "Beatriz Lemos", "Duarte Reis", "Sofia Prado"
    };

    private int index = -1;
    private int correct, wrong;
    private readonly List<string> mistakes = new List<string>();
    private ArtistData current;
    private bool decisionLocked;
    private bool started;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private IEnumerator Start()
    {
        if (artists.Count == 0) GeneratePlaceholders();
        if (shuffle) Shuffle(artists);

        // Espera 2 frames para a UI ter tempo de subscrever os eventos
        yield return null;
        yield return null;
        if (startAutomatically) StartGame();
    }

    /// <summary>Começa o jogo (chamado pelo botão do ecrã de intro).</summary>
    public void StartGame()
    {
        if (started) return;
        started = true;
        NextArtist();
    }

    private void GeneratePlaceholders()
    {
        for (int i = 0; i < placeholderCount; i++)
        {
            bool genuine = i % 2 == 0;
            ArtistKind kind = genuine ? ArtistKind.Human : ((i / 2) % 2 == 0 ? ArtistKind.Robot : ArtistKind.Impostor);
            string name = PlaceholderNames[i % PlaceholderNames.Length];
            artists.Add(ArtistData.CreatePlaceholder(name, kind, genuine));
        }
    }

    private void NextArtist()
    {
        index++;
        if (index >= artists.Count) { EndGame(); return; }

        current = artists[index];
        decisionLocked = false;
        OnProgress?.Invoke(index + 1, artists.Count);
        OnArtistChanged?.Invoke(current);
        OnDecisionsEnabled?.Invoke(true);
    }

    /// <summary>Chamado pelos botões Aceitar (true) / Recusar (false).</summary>
    public void Decide(bool accept)
    {
        if (decisionLocked || current == null) return;
        decisionLocked = true;
        OnDecisionsEnabled?.Invoke(false);

        bool right = accept == current.isGenuine;
        if (right) correct++;
        else
        {
            wrong++;
            string why = current.isGenuine
                ? "Recusaste um artista real."
                : "Deixaste entrar um impostor. " + (string.IsNullOrEmpty(current.flawHint) ? current.rejectReason : current.flawHint);
            mistakes.Add(current.artistName + " - " + why);
        }

        // O jogador NÃO vê se acertou (só no fim), mas o artista reage no chat.
        OnDecisionMade?.Invoke(current, accept, right);

        StartCoroutine(NextAfterDelay());
    }

    private IEnumerator NextAfterDelay()
    {
        yield return new WaitForSeconds(delayBetweenArtists);
        NextArtist();
    }

    private void EndGame()
    {
        var result = new GameResult
        {
            total = artists.Count,
            correct = correct,
            wrong = wrong,
            fired = wrong >= maxMistakesBeforeFired,
            mistakes = mistakes
        };
        OnGameEnded?.Invoke(result);
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
