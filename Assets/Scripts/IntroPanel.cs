using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Ecrã de introdução (briefing). O jogo só começa quando carregas em COMEÇAR.
/// </summary>
public class IntroPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button startButton;

    private void Start() => startButton.onClick.AddListener(OnStart);

    private void OnStart()
    {
        panel.SetActive(false);
        if (GameManager.Instance != null) GameManager.Instance.StartGame();
    }
}
