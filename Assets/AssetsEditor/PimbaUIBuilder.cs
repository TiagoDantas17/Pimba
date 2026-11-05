#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Constrói TODA a UI de gameplay automaticamente e liga os scripts.
/// Colocar este ficheiro em Assets/Editor/ (cria a pasta Editor se não existir).
/// Menu: Pimba > Construir UI de Gameplay
/// </summary>
public static class PimbaUIBuilder
{
    static readonly Color Dark = new Color(0.15f, 0.12f, 0.1f);
    static readonly Color Paper = new Color(0.93f, 0.9f, 0.78f);

    [MenuItem("Pimba/Construir UI de Gameplay")]
    public static void Build()
    {
        if (!EditorUtility.DisplayDialog("Construir UI",
            "Isto vai APAGAR o conteúdo atual do Canvas desta cena e reconstruí-lo de raiz. Continuar?",
            "Sim", "Cancelar")) return;

        // ---------- Canvas ----------
        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            var cgo = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Undo.RegisterCreatedObjectUndo(cgo, "Pimba UI");
            canvas = cgo.GetComponent<Canvas>();
        }
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var canvasGO = canvas.gameObject;

        var scaler = canvasGO.GetComponent<CanvasScaler>();
        if (scaler == null) scaler = Undo.AddComponent<CanvasScaler>(canvasGO);
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;
        if (canvasGO.GetComponent<GraphicRaycaster>() == null) Undo.AddComponent<GraphicRaycaster>(canvasGO);

        // Limpar lixo antigo
        var gmOnCanvas = canvasGO.GetComponent<GameManager>();
        if (gmOnCanvas != null) Undo.DestroyObjectImmediate(gmOnCanvas);
        for (int i = canvasGO.transform.childCount - 1; i >= 0; i--)
            Undo.DestroyObjectImmediate(canvasGO.transform.GetChild(i).gameObject);
        DestroyOld<DialogueUI>(canvasGO);
        DestroyOld<ArtistViewUI>(canvasGO);
        DestroyOld<EndingController>(canvasGO);

        // ---------- Fundo e painéis ----------
        var bg = NewPanel("Background", canvas.transform, Paper, Vector2.zero, Vector2.one, 0);

        var artistPanel = NewPanel("ArtistPanel", canvas.transform, new Color(0.35f, 0.28f, 0.2f), new Vector2(0, 0), new Vector2(0.34f, 1), 20);
        var dialoguePanel = NewPanel("DialoguePanel", canvas.transform, new Color(1f, 1f, 1f, 0.85f), new Vector2(0.34f, 0.62f), Vector2.one, 20);
        var infoPanel = NewPanel("InfoPanel", canvas.transform, new Color(1f, 1f, 1f, 0.85f), new Vector2(0.34f, 0), new Vector2(1, 0.62f), 20);

        // ---------- Painel do artista ----------
        var progressText = NewText("ProgressText", artistPanel.transform, "1/8", 36, TextAlignmentOptions.Center, Color.white, new Vector2(0, 0.93f), Vector2.one);
        var portrait = NewPanel("Portrait", artistPanel.transform, Color.gray, new Vector2(0.1f, 0.33f), new Vector2(0.9f, 0.92f), 0);
        var nameText = NewText("NameText", artistPanel.transform, "Nome", 48, TextAlignmentOptions.Center, Color.white, new Vector2(0, 0.21f), new Vector2(1, 0.31f));
        var acceptBtn = NewButton("AcceptButton", artistPanel.transform, "ACEITAR", new Color(0.2f, 0.6f, 0.25f), new Vector2(0.05f, 0.04f), new Vector2(0.48f, 0.18f));
        var rejectBtn = NewButton("RejectButton", artistPanel.transform, "RECUSAR", new Color(0.75f, 0.2f, 0.2f), new Vector2(0.52f, 0.04f), new Vector2(0.95f, 0.18f));

        // ---------- Conversa (ScrollView) ----------
        var scrollRT = NewRect("Scroll View", dialoguePanel.transform);
        Stretch(scrollRT, Vector2.zero, Vector2.one, 15);
        var scroll = scrollRT.gameObject.AddComponent<ScrollRect>();

        var viewport = NewRect("Viewport", scrollRT);
        Stretch(viewport, Vector2.zero, Vector2.one, 0);
        viewport.gameObject.AddComponent<RectMask2D>();
        var vpImg = viewport.gameObject.AddComponent<Image>();
        vpImg.color = new Color(0, 0, 0, 0); // precisa de um Graphic para receber scroll do rato

        var content = NewRect("Content", viewport);
        content.anchorMin = new Vector2(0, 1);
        content.anchorMax = new Vector2(1, 1);
        content.pivot = new Vector2(0.5f, 1);
        content.offsetMin = Vector2.zero;
        content.offsetMax = Vector2.zero;
        var vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 12;
        vlg.padding = new RectOffset(10, 10, 10, 10);
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scroll.viewport = viewport;
        scroll.content = content;
        scroll.horizontal = false;
        scroll.vertical = true;
        scroll.movementType = ScrollRect.MovementType.Clamped;
        scroll.scrollSensitivity = 30f;

        // Prefab da linha de diálogo
        var linePrefab = CreateLinePrefab();

        // ---------- Info ----------
        var infoText = NewText("InfoText", infoPanel.transform, "Informação do artista", 40, TextAlignmentOptions.TopLeft, Dark, Vector2.zero, Vector2.one);
        Stretch(infoText.rectTransform, Vector2.zero, Vector2.one, 30);

        // ---------- Ecrã final ----------
        var ending = NewPanel("EndingPanel", canvas.transform, new Color(0, 0, 0, 0.92f), Vector2.zero, Vector2.one, 0);
        var resultText = NewText("ResultText", ending.transform, "Resultado", 64, TextAlignmentOptions.Center, Color.white, new Vector2(0.1f, 0.62f), new Vector2(0.9f, 0.92f));
        var mistakesText = NewText("MistakesText", ending.transform, "", 30, TextAlignmentOptions.TopLeft, Color.white, new Vector2(0.08f, 0.28f), new Vector2(0.55f, 0.6f));
        var bossFired = NewPanel("BossFired", ending.transform, new Color(0.8f, 0.2f, 0.2f), new Vector2(0.62f, 0.3f), new Vector2(0.92f, 0.6f), 0);
        var bossHappy = NewPanel("BossHappy", ending.transform, new Color(0.2f, 0.7f, 0.3f), new Vector2(0.62f, 0.3f), new Vector2(0.92f, 0.6f), 0);
        var bossLine = NewText("BossLineText", ending.transform, "", 44, TextAlignmentOptions.Center, Color.yellow, new Vector2(0.1f, 0.14f), new Vector2(0.9f, 0.27f));
        var backBtn = NewButton("BackToMenuButton", ending.transform, "VOLTAR AO MENU", new Color(0.3f, 0.3f, 0.3f), new Vector2(0.38f, 0.03f), new Vector2(0.62f, 0.11f));
        bossFired.gameObject.SetActive(false);
        bossHappy.gameObject.SetActive(false);
        ending.gameObject.SetActive(false);

        // ---------- Controladores (fora do Canvas) ----------
        var ctrl = new GameObject("UIControllers");
        Undo.RegisterCreatedObjectUndo(ctrl, "Pimba UI");
        var dlg = ctrl.AddComponent<DialogueUI>();
        var view = ctrl.AddComponent<ArtistViewUI>();
        var end = ctrl.AddComponent<EndingController>();

        Set(dlg, "scrollRect", scroll);
        Set(dlg, "content", content);
        Set(dlg, "linePrefab", linePrefab);

        Set(view, "portraitImage", portrait);
        Set(view, "nameText", nameText);
        Set(view, "infoText", infoText);
        Set(view, "acceptButton", acceptBtn);
        Set(view, "rejectButton", rejectBtn);
        Set(view, "progressText", progressText);

        Set(end, "endingPanel", ending.gameObject);
        Set(end, "resultText", resultText);
        Set(end, "mistakesText", mistakesText);
        Set(end, "bossFiredGroup", bossFired.gameObject);
        Set(end, "bossHappyGroup", bossHappy.gameObject);
        Set(end, "bossLineText", bossLine);
        Set(end, "backToMenuButton", backBtn);

        // ---------- GameManager e EventSystem ----------
        if (Object.FindFirstObjectByType<GameManager>() == null)
        {
            var gm = new GameObject("GameManager");
            Undo.RegisterCreatedObjectUndo(gm, "Pimba UI");
            gm.AddComponent<GameManager>();
        }
        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            EditorApplication.ExecuteMenuItem("GameObject/UI/Event System");

        EditorSceneManager.MarkSceneDirty(canvasGO.scene);
        Selection.activeGameObject = canvasGO;
        Debug.Log("[Pimba] UI construída. Carrega em Play!");
    }

    // ---------------- helpers ----------------
    static void DestroyOld<T>(GameObject keep) where T : Component
    {
        foreach (var c in Object.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (c != null && c.gameObject != keep) Undo.DestroyObjectImmediate(c.gameObject);
    }

    static RectTransform NewRect(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(go, "Pimba UI");
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    static void Stretch(RectTransform r, Vector2 min, Vector2 max, float margin)
    {
        r.anchorMin = min;
        r.anchorMax = max;
        r.offsetMin = new Vector2(margin, margin);
        r.offsetMax = new Vector2(-margin, -margin);
    }

    static Image NewPanel(string name, Transform parent, Color color, Vector2 min, Vector2 max, float margin)
    {
        var r = NewRect(name, parent);
        Stretch(r, min, max, margin);
        var img = r.gameObject.AddComponent<Image>();
        img.color = color;
        return img;
    }

    static TextMeshProUGUI NewText(string name, Transform parent, string text, float size,
        TextAlignmentOptions align, Color color, Vector2 min, Vector2 max)
    {
        var r = NewRect(name, parent);
        Stretch(r, min, max, 5);
        var t = r.gameObject.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = size;
        t.alignment = align;
        t.color = color;
        return t;
    }

    static Button NewButton(string name, Transform parent, string label, Color color, Vector2 min, Vector2 max)
    {
        var r = NewRect(name, parent);
        Stretch(r, min, max, 0);
        var img = r.gameObject.AddComponent<Image>();
        img.color = color;
        var btn = r.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;
        var t = NewText("Label", r, label, 40, TextAlignmentOptions.Center, Color.white, Vector2.zero, Vector2.one);
        t.raycastTarget = false;
        return btn;
    }

    static TMP_Text CreateLinePrefab()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");
        var go = new GameObject("DialogueLine", typeof(RectTransform));
        var t = go.AddComponent<TextMeshProUGUI>();
        t.fontSize = 34;
        t.color = Dark;
        t.text = "Linha de diálogo";
        t.raycastTarget = false;
        const string path = "Assets/Prefabs/DialogueLine.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponent<TMP_Text>();
    }

    static void Set(Object target, string field, Object value)
    {
        var so = new SerializedObject(target);
        var p = so.FindProperty(field);
        if (p == null) { Debug.LogWarning("[Pimba] Campo não encontrado: " + field); return; }
        p.objectReferenceValue = value;
        so.ApplyModifiedProperties();
    }

    // Overloads para passar Image/TMP/Button diretamente
    static void Set(Object target, string field, Component value) => Set(target, field, (Object)value);
}
#endif