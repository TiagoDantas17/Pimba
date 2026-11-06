using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Ecrã final: resultado (só aqui se sabe quantos erros), e
/// imagem/cutscene do chefe a despedir (placeholder).
/// </summary>
public class EndingController : MonoBehaviour
{
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text mistakesText;

    [Header("Chefe (placeholder)")]
    [SerializeField] private GameObject bossFiredGroup;   // imagem/cutscene do despedimento
    [SerializeField] private GameObject bossHappyGroup;   // imagem/cutscene de parabéns
    [SerializeField] private TMP_Text bossLineText;
    [SerializeField] private float bossDelay = 2f;

    [SerializeField] private Button backToMenuButton;
    [SerializeField] private string menuSceneName = "MainMenu";

    private void Start()
    {
        endingPanel.SetActive(false);
        if (bossFiredGroup) bossFiredGroup.SetActive(false);
        if (bossHappyGroup) bossHappyGroup.SetActive(false);
        backToMenuButton.onClick.AddListener(() => SceneManager.LoadScene(menuSceneName));
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        while (GameManager.Instance == null) yield return null;
        GameManager.Instance.OnGameEnded += ShowEnding;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnGameEnded -= ShowEnding;
    }

    private void ShowEnding(GameResult r)
    {
        endingPanel.SetActive(true);
        resultText.text = $"Acertaste {r.correct} de {r.total}\nErraste {r.wrong}";

        var sb = new StringBuilder();
        foreach (var m in r.mistakes) sb.AppendLine("- " + m);
        mistakesText.text = sb.ToString();

        StartCoroutine(BossRoutine(r.fired));
    }

    private IEnumerator BossRoutine(bool fired)
    {
        yield return new WaitForSeconds(bossDelay);
        if (fired)
        {
            if (bossFiredGroup) bossFiredGroup.SetActive(true);
            if (bossLineText) bossLineText.text = "Estás despedido! O quarteirão cultural foi uma vergonha!";
        }
        else
        {
            if (bossHappyGroup) bossHappyGroup.SetActive(true);
            if (bossLineText) bossLineText.text = "Excelente trabalho! O quarteirão cultural foi um sucesso!";
        }
    }
}
