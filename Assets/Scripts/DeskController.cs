using System.Collections;
using UnityEngine;

/// <summary>
/// Quando muda o artista, põe na mesa os objetos que ele entrega
/// (os cartões a mais ficam escondidos).
/// </summary>
public class DeskController : MonoBehaviour
{
    [SerializeField] private DocumentCard[] cards;

    private void OnEnable()
    {
        if (GameManager.Instance != null) Subscribe();
        else StartCoroutine(WaitAndSubscribe());
    }

    private IEnumerator WaitAndSubscribe()
    {
        while (GameManager.Instance == null) yield return null;
        Subscribe();
    }

    private void Subscribe() => GameManager.Instance.OnArtistChanged += ShowArtist;

    private void OnDisable()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnArtistChanged -= ShowArtist;
    }

    private void ShowArtist(ArtistData a)
    {
        if (a.handouts.Count > cards.Length)
            Debug.LogWarning($"[Pimba] {a.artistName} entrega {a.handouts.Count} objetos, mas só há {cards.Length} cartões na mesa.");

        for (int i = 0; i < cards.Length; i++)
        {
            if (i < a.handouts.Count) cards[i].Show(a, a.handouts[i]);
            else cards[i].Hide();
        }
    }
}