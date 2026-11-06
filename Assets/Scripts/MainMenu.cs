using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Menu principal: Jogar, Créditos, Sair.
/// Liga cada método ao OnClick do respetivo botão.
/// </summary>
public class MainMenu : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "SceneTiago";
    [SerializeField] private GameObject creditsPanel;

    private void Start()
    {
        if (creditsPanel) creditsPanel.SetActive(false);
    }

    public void Play() => SceneManager.LoadScene(gameSceneName);

    public void ToggleCredits()
    {
        if (creditsPanel) creditsPanel.SetActive(!creditsPanel.activeSelf);
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}