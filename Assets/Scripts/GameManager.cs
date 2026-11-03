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
    [SerializeField] private float delayBetweenArtists = 0.8f;

    // Eventos para a UI ouvir
    public event Action<ArtistData> OnArtistChanged;
    public event Action<int, int> OnProgress;        // atual, total
    public event Action<GameResult> OnGameEnded;
    public event Action<bool> OnDecisionsEnabled;    // ativar/desativar botões

    private int index = -1;
    private int correct, wrong;
    private readonly List<string> mistakes = new List<string>();
    private ArtistData current;
    private bool decisionLocked;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (artists.Count == 0) GeneratePlaceholders();
        if (shuffle) Shuffle(artists);
        NextArtist();
    }

    private void GeneratePlaceholders()
    {
        for (int i = 0; i < placeholderCount; i++)
        {
            bool pimba = i % 2 == 0;
            ArtistKind kind = pimba ? ArtistKind.Human : (i % 3 == 0 ? ArtistKind.Robot : ArtistKind.Impostor);
            artists.Add(ArtistData.CreatePlaceholder("Artista " + (i + 1), kind, pimba));
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

        // Nota: o jogador NÃO vê se acertou durante o jogo (só no fim).
        if (accept == current.isPimba) correct++;
        else
        {
            wrong++;
            string why = current.isPimba ? "Recusaste um artista Pimba." : "Deixaste entrar: " + current.rejectReason;
            mistakes.Add(current.artistName + " - " + why);
        }

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