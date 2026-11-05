using System.Collections;
using UnityEngine;

/// <summary>
/// Quando muda o artista, atualiza os cartões em cima da mesa.
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
        foreach (var c in cards) c.Show(a);
    }
}
