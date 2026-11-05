#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Constrói a UI com a estética do concept art (cabine à esquerda, mesa de madeira à direita,
/// ecrã de chat, cartões arrastáveis, botões redondos) e liga os scripts.
/// Colocar em Assets/Editor/.   Menu: Pimba > Construir UI (Estética)
/// Resolução de referência: 2560x1440 (a mesma do concept art).
/// </summary>
public static class PimbaArtUIBuilder
{
    static readonly Color Ink = new Color(0.13f, 0.1f, 0.08f);
    static readonly Color Paper = new Color(0.93f, 0.9f, 0.78f);
    static readonly Color Wood = new Color(0.27f, 0.19f, 0.14f);
    static readonly Color BoothDark = new Color(0.05f, 0.04f, 0.045f);
    static readonly Color FrameBlue = new Color(0.43f, 0.62f, 0.82f);
    static readonly Color ChatText = new Color(0.88f, 0.96f, 0.92f);

    // converte um x absoluto do ecrã (0..1) para um x relativo ao painel Desk (que começa em 0.452)
    static float RX(float absX) => (absX - 0.452f) / 0.548f;

    [MenuItem("Pimba/Construir UI (Estética)")]
    public static void Build()
    {
        if (!EditorUtility.DisplayDialog("Construir UI (Estética)",
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
        scaler.referenceResolution = new Vector2(2560, 1440);
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
        DestroyOld<DeskController>(canvasGO);
        DestroyOld<IntroPanel>(canvasGO);

        var knob = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

        // ================= CABINE (esquerda) =================
        NewPanel("Background", canvas.transform, BoothDark, Vector2.zero, Vector2.one, 0);
        var booth = NewPanel("Booth", canvas.transform, BoothDark, new Vector2(0, 0), new Vector2(0.452f, 1), 0);
        NewPanel("BoothLight", booth.transform, new Color(1f, 0.85f, 0.45f, 0.35f), new Vector2(0.12f, 0.965f), new Vector2(0.85f, 1f), 0);
        var portrait = NewPanel("Portrait", booth.transform, new Color(0.3f, 0.25f, 0.25f), new Vector2(0.14f, 0.28f), new Vector2(0.86f, 0.93f), 0);
        NewPanel("Counter", booth.transform, new Color(0.2f, 0.14f, 0.11f), new Vector2(0, 0), new Vector2(1, 0.27f), 0);
        // Decoração do balcão: troca a Source Image destes objetos pelos teus desenhos
        NewPanel("Decor_Duck", booth.transform, new Color(0.75f, 0.6f, 0.15f), new Vector2(0.05f, 0.1f), new Vector2(0.14f, 0.24f), 0);
        NewPanel("Decor_Laptop", booth.transform, new Color(0.65f, 0.75f, 0.9f, 0.9f), new Vector2(0.2f, 0.04f), new Vector2(0.55f, 0.25f), 0);
        NewPanel("Decor_PencilCup", booth.transform, new Color(0.5f, 0.5f, 0.5f), new Vector2(0.88f, 0.07f), new Vector2(0.97f, 0.2f), 0);
        var nameText = NewText("NameText", booth.transform, "Nome", 60, TextAlignmentOptions.Center, Color.white, new Vector2(0.1f, 0.275f), new Vector2(0.9f, 0.36f));
        var progressText = NewText("ProgressText", booth.transform, "1/8", 44, TextAlignmentOptions.Left, Color.white, new Vector2(0.03f, 0.92f), new Vector2(0.35f, 0.99f));

        // ================= MESA (direita) =================
        var desk = NewPanel("Desk", canvas.transform, Wood, new Vector2(0.452f, 0), Vector2.one, 0);
        NewPanel("BoothEdge", canvas.transform, new Color(0.22f, 0.14f, 0.14f), new Vector2(0.446f, 0), new Vector2(0.458f, 1), 0);

        // Ecrã de chat
        NewPanel("ChatFrame", desk.transform, FrameBlue, new Vector2(0.005f, 0.5f), new Vector2(1f, 1f), 0);
        var screen = NewPanel("ChatScreen", desk.transform, new Color(0.03f, 0.03f, 0.035f), new Vector2(RX(0.477f), 0.66f), new Vector2(RX(0.977f), 1f), 0);

        var badge = NewPanel("Badge", desk.transform, new Color(0.02f, 0.02f, 0.03f), new Vector2(RX(0.506f), 0.538f), new Vector2(RX(0.625f), 0.649f), 0);
        NewText("BadgeText", badge.transform, "quarteirão cultural", 34, TextAlignmentOptions.Center, new Color(0.3f, 0.7f, 1f), Vector2.zero, Vector2.one);

        NewPanel("Decor_Photo", desk.transform, new Color(0.95f, 0.65f, 0.65f), new Vector2(0.79f, 0.396f), new Vector2(0.982f, 0.622f), 0);

        // Scroll do chat
        var scroll = BuildScroll(screen.transform, 30, out var content);
        var linePrefab = CreateLinePrefab();

        // Área da mesa onde os cartões se arrastam
        var deskArea = NewRect("DeskArea", desk.transform);
        deskArea.anchorMin = new Vector2(0.02f, 0.03f);
        deskArea.anchorMax = new Vector2(0.84f, 0.52f);
        deskArea.offsetMin = Vector2.zero;
        deskArea.offsetMax = Vector2.zero;

        var cardId = NewCard("Card_Identity", deskArea, DocType.Identity, new Color(0.85f, 0.87f, 0.8f), new Vector2(-260, 170));
        var cardMember = NewCard("Card_Member", deskArea, DocType.Member, new Color(0.75f, 0.85f, 0.95f), new Vector2(260, 170));
        var cardPassport = NewCard("Card_Passport", deskArea, DocType.Passport, new Color(0.55f, 0.75f, 0.55f), new Vector2(-260, -170));
        var cardPortfolio = NewCard("Card_Portfolio", deskArea, DocType.Portfolio, Paper, new Vector2(260, -170));

        // Botões redondos (trocar a Source Image pelos teus desenhos)
        var rejectBtn = NewButton("RejectButton", desk.transform, "X", new Color(0.75f, 0.2f, 0.2f), new Vector2(0.8715f, 0.139f), new Vector2(0.9785f, 0.243f), knob);
        var acceptBtn = NewButton("AcceptButton", desk.transform, "OK", new Color(0.35f, 0.6f, 0.3f), new Vector2(0.8715f, 0.014f), new Vector2(0.9785f, 0.118f), knob);

        // ================= ECRÃ FINAL =================
        var ending = NewPanel("EndingPanel", canvas.transform, new Color(0, 0, 0, 0.93f), Vector2.zero, Vector2.one, 0);
        var resultText = NewText("ResultText", ending.transform, "Resultado", 80, TextAlignmentOptions.Center, Color.white, new Vector2(0.1f, 0.62f), new Vector2(0.9f, 0.92f));
        var mistakesText = NewText("MistakesText", ending.transform, "", 36, TextAlignmentOptions.TopLeft, Color.white, new Vector2(0.08f, 0.28f), new Vector2(0.55f, 0.6f));
        var bossFired = NewPanel("BossFired", ending.transform, new Color(0.8f, 0.2f, 0.2f), new Vector2(0.62f, 0.3f), new Vector2(0.92f, 0.6f), 0);
        var bossHappy = NewPanel("BossHappy", ending.transform, new Color(0.2f, 0.7f, 0.3f), new Vector2(0.62f, 0.3f), new Vector2(0.92f, 0.6f), 0);
        var bossLine = NewText("BossLineText", ending.transform, "", 54, TextAlignmentOptions.Center, Color.yellow, new Vector2(0.1f, 0.14f), new Vector2(0.9f, 0.27f));
        var backBtn = NewButton("BackToMenuButton", ending.transform, "VOLTAR AO MENU", new Color(0.3f, 0.3f, 0.3f), new Vector2(0.36f, 0.03f), new Vector2(0.64f, 0.11f), null);
        bossFired.gameObject.SetActive(false);
        bossHappy.gameObject.SetActive(false);
        ending.gameObject.SetActive(false);

        // ================= INTRO (briefing) =================
        var intro = NewPanel("IntroPanel", canvas.transform, new Color(0, 0, 0, 0.85f), Vector2.zero, Vector2.one, 0);
        var introCard = NewPanel("IntroCard", intro.transform, Paper, new Vector2(0.2f, 0.1f), new Vector2(0.8f, 0.9f), 0);
        NewText("IntroTitle", introCard.transform, "QUARTEIRÃO CULTURAL", 72, TextAlignmentOptions.Center, Ink, new Vector2(0.05f, 0.87f), new Vector2(0.95f, 0.98f));
        var introBody = NewText("IntroBody", introCard.transform,
            "Como organizador de eventos, foi-te dada a tarefa de recrutar artistas para um quarteirão cultural.\n\n" +
            "Mas, com os artistas de hoje em dia a serem honestos, ou nem tanto, cabe-te a ti descobrir e escolher bem quem vais aceitar, para o espetáculo não ser um fracasso.\n\n" +
            "Tens de verificar a identidade do artista, o nº de sócio e o passaporte (quando aplicável).\n\n" +
            "Cuidado: copycats ou IAs conseguem falsificar documentos, mas têm sempre falhas. Fica atento.\n\n" +
            "Choose wisely, o sucesso do evento está nas tuas mãos.",
            40, TextAlignmentOptions.TopLeft, Ink, new Vector2(0.06f, 0.2f), new Vector2(0.94f, 0.86f));
        var startBtn = NewButton("StartButton", introCard.transform, "COMEÇAR", new Color(0.35f, 0.6f, 0.3f), new Vector2(0.35f, 0.04f), new Vector2(0.65f, 0.15f), null);

        // ================= CONTROLADORES =================
        var ctrl = new GameObject("UIControllers");
        Undo.RegisterCreatedObjectUndo(ctrl, "Pimba UI");
        var dlg = ctrl.AddComponent<DialogueUI>();
        var view = ctrl.AddComponent<ArtistViewUI>();
        var deskCtrl = ctrl.AddComponent<DeskController>();
        var end = ctrl.AddComponent<EndingController>();
        var introCtrl = ctrl.AddComponent<IntroPanel>();

        Set(dlg, "scrollRect", scroll);
        Set(dlg, "content", content);
        Set(dlg, "linePrefab", linePrefab);

        Set(view, "portraitImage", portrait);
        Set(view, "nameText", nameText);
        Set(view, "acceptButton", acceptBtn);
        Set(view, "rejectButton", rejectBtn);
        Set(view, "progressText", progressText);

        SetArray(deskCtrl, "cards", new Object[] { cardId, cardMember, cardPassport, cardPortfolio });

        Set(end, "endingPanel", ending.gameObject);
        Set(end, "resultText", resultText);
        Set(end, "mistakesText", mistakesText);
        Set(end, "bossFiredGroup", bossFired.gameObject);
        Set(end, "bossHappyGroup", bossHappy.gameObject);
        Set(end, "bossLineText", bossLine);
        Set(end, "backToMenuButton", backBtn);

        Set(introCtrl, "panel", intro.gameObject);
        Set(introCtrl, "startButton", startBtn);

        // ================= GameManager e EventSystem =================
        var gm = Object.FindFirstObjectByType<GameManager>();
        if (gm == null)
        {
            var gmGO = new GameObject("GameManager");
            Undo.RegisterCreatedObjectUndo(gmGO, "Pimba UI");
            gm = gmGO.AddComponent<GameManager>();
        }
        SetBool(gm, "startAutomatically", false); // o jogo começa no botão COMEÇAR

        if (Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            EditorApplication.ExecuteMenuItem("GameObject/UI/Event System");

        EditorSceneManager.MarkSceneDirty(canvasGO.scene);
        Selection.activeGameObject = canvasGO;
        Debug.Log("[Pimba] UI (estética) construída. Põe a Game view em 16:9 e carrega em Play!");
    }

    // ---------------- cartões ----------------
    static DocumentCard NewCard(string name, Transform parent, DocType type, Color color, Vector2 pos)
    {
        var r = NewRect(name, parent);
        r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
        r.pivot = new Vector2(0.5f, 0.5f);
        r.sizeDelta = new Vector2(440, 300);
        r.anchoredPosition = pos;

        var img = r.gameObject.AddComponent<Image>();
        img.color = color;

        var title = NewText("Title", r, "", 34, TextAlignmentOptions.Center, Ink, new Vector2(0, 0.82f), Vector2.one);
        title.fontStyle = FontStyles.Bold;
        title.raycastTarget = false;
        var body = NewText("Body", r, "", 28, TextAlignmentOptions.TopLeft, Ink, Vector2.zero, new Vector2(1, 0.82f));
        Stretch(body.rectTransform, Vector2.zero, new Vector2(1, 0.82f), 18);
        body.raycastTarget = false;

        var card = r.gameObject.AddComponent<DocumentCard>();
        r.gameObject.AddComponent<DraggableCard>();

        var so = new SerializedObject(card);
        so.FindProperty("type").enumValueIndex = (int)type;
        so.FindProperty("titleText").objectReferenceValue = title;
        so.FindProperty("bodyText").objectReferenceValue = body;
        so.ApplyModifiedProperties();
        return card;
    }

    // ---------------- scroll ----------------
    static ScrollRect BuildScroll(Transform parent, float margin, out RectTransform content)
    {
        var scrollRT = NewRect("Scroll View", parent);
        Stretch(scrollRT, Vector2.zero, Vector2.one, margin);
        var scroll = scrollRT.gameObject.AddComponent<ScrollRect>();

        var viewport = NewRect("Viewport", scrollRT);
        Stretch(viewport, Vector2.zero, Vector2.one, 0);
        viewport.gameObject.AddComponent<RectMask2D>();
        var vpImg = viewport.gameObject.AddComponent<Image>();
        vpImg.color = new Color(0, 0, 0, 0); // precisa de um Graphic para receber scroll do rato

        content = NewRect("Content", viewport);
        content.anchorMin = new Vector2(0, 1);
        content.anchorMax = new Vector2(1, 1);
        content.pivot = new Vector2(0.5f, 1);
        content.offsetMin = Vector2.zero;
        content.offsetMax = Vector2.zero;
        var vlg = content.gameObject.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 14;
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
        scroll.scrollSensitivity = 40f;
        return scroll;
    }

    static TMP_Text CreateLinePrefab()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Prefabs")) AssetDatabase.CreateFolder("Assets", "Prefabs");
        var go = new GameObject("DialogueLine", typeof(RectTransform));
        var t = go.AddComponent<TextMeshProUGUI>();
        t.fontSize = 36;
        t.color = ChatText;
        t.text = "Linha de diálogo";
        t.raycastTarget = false;
        const string path = "Assets/Prefabs/DialogueLine.prefab";
        PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return AssetDatabase.LoadAssetAtPath<GameObject>(path).GetComponent<TMP_Text>();
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

    static Button NewButton(string name, Transform parent, string label, Color color, Vector2 min, Vector2 max, Sprite sprite)
    {
        var r = NewRect(name, parent);
        Stretch(r, min, max, 0);
        var img = r.gameObject.AddComponent<Image>();
        img.color = color;
        if (sprite != null) img.sprite = sprite;
        var btn = r.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;
        var t = NewText("Label", r, label, 44, TextAlignmentOptions.Center, Color.white, Vector2.zero, Vector2.one);
        t.raycastTarget = false;
        return btn;
    }

    static void Set(Object target, string field, Object value)
    {
        var so = new SerializedObject(target);
        var p = so.FindProperty(field);
        if (p == null) { Debug.LogWarning("[Pimba] Campo não encontrado: " + field + " em " + target.GetType().Name); return; }
        p.objectReferenceValue = value;
        so.ApplyModifiedProperties();
    }

    static void SetBool(Object target, string field, bool value)
    {
        var so = new SerializedObject(target);
        var p = so.FindProperty(field);
        if (p == null) { Debug.LogWarning("[Pimba] Campo não encontrado: " + field + " em " + target.GetType().Name); return; }
        p.boolValue = value;
        so.ApplyModifiedProperties();
    }

    static void SetArray(Object target, string field, Object[] values)
    {
        var so = new SerializedObject(target);
        var p = so.FindProperty(field);
        if (p == null) { Debug.LogWarning("[Pimba] Campo não encontrado: " + field); return; }
        p.arraySize = values.Length;
        for (int i = 0; i < values.Length; i++) p.GetArrayElementAtIndex(i).objectReferenceValue = values[i];
        so.ApplyModifiedProperties();
    }
}
#endif