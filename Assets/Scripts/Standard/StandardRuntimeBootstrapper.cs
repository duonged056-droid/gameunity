using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class StandardRuntimeBootstrapper
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnSceneLoaded()
    {
        // If GameManager already exists, scene is already configured
        if (GameManager.instance != null || Object.FindAnyObjectByType<GameManager>() != null)
        {
            return;
        }

        Debug.Log("<color=yellow><b>[7 Viên Ngọc Rồng] Tự động khởi tạo màn chơi chuẩn 6 Giai đoạn (Runtime)...</b></color>");
        BootstrapStandardScene();
    }

    public static void BootstrapStandardScene()
    {
        // 1. Camera & AudioListener
        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camObj = new GameObject("Main Camera");
            cam = camObj.AddComponent<Camera>();
            camObj.tag = "MainCamera";
        }
        cam.orthographic = true;
        cam.orthographicSize = 5.0f;
        cam.transform.position = new Vector3(0, 0, -10f);
        cam.backgroundColor = new Color(0.08f, 0.08f, 0.12f, 1f);
        if (cam.GetComponent<AudioListener>() == null) cam.gameObject.AddComponent<AudioListener>();

        // 2. Background (GIAI ĐOẠN 2 & 4)
        GameObject bgGo = new GameObject("Background");
        SpriteRenderer bgSr = bgGo.AddComponent<SpriteRenderer>();
        bgSr.sprite = Resources.Load<Sprite>("Sprites/Background") ?? Resources.Load<Sprite>("Background");
#if UNITY_EDITOR
        if (bgSr.sprite == null) bgSr.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Background.png");
#endif
        bgSr.sortingLayerName = "Background";
        bgSr.sortingOrder = -10;
        bgGo.transform.localScale = new Vector3(1.2f, 1.2f, 1f);

        // 3. DragonBallAltar (Căn cứ bảo vệ)
        GameObject altarGo = new GameObject("DragonBallAltar");
        altarGo.transform.position = new Vector3(-7.5f, 0, 0);
        altarGo.tag = "Base";
        SpriteRenderer altarSr = altarGo.AddComponent<SpriteRenderer>();
        altarSr.sprite = Resources.Load<Sprite>("Sprites/DragonBallAltar") ?? Resources.Load<Sprite>("DragonBallAltar");
#if UNITY_EDITOR
        if (altarSr.sprite == null) altarSr.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/DragonBallAltar.png");
#endif
        altarSr.sortingLayerName = "Gameplay";
        altarSr.sortingOrder = 2;
        altarGo.transform.localScale = new Vector3(0.4f, 0.4f, 1f);

        BoxCollider2D altarCol = altarGo.AddComponent<BoxCollider2D>();
        altarCol.isTrigger = true;
        altarCol.size = new Vector2(2f, 10f); // Blocks entire left edge

        // 4. SpawnPoints & EndPoints (3 Làn đường)
        GameObject spawnRoot = new GameObject("SpawnPoints");
        Transform[] spawnPts = new Transform[3];
        spawnPts[0] = CreateEmptyChild(spawnRoot.transform, "Spawn_Top", new Vector3(8.5f, 3.0f, 0));
        spawnPts[1] = CreateEmptyChild(spawnRoot.transform, "Spawn_Mid", new Vector3(8.5f, 0.0f, 0));
        spawnPts[2] = CreateEmptyChild(spawnRoot.transform, "Spawn_Bot", new Vector3(8.5f, -3.0f, 0));

        GameObject endRoot = new GameObject("EndPoints");
        Transform[] endPts = new Transform[3];
        endPts[0] = CreateEmptyChild(endRoot.transform, "End_Top", new Vector3(-7.5f, 3.0f, 0));
        endPts[1] = CreateEmptyChild(endRoot.transform, "End_Mid", new Vector3(-7.5f, 0.0f, 0));
        endPts[2] = CreateEmptyChild(endRoot.transform, "End_Bot", new Vector3(-7.5f, -3.0f, 0));

        // 5. Player Goku
        GameObject playerGo = new GameObject("Player");
        playerGo.transform.position = new Vector3(-5.5f, 0, 0);
        playerGo.tag = "Player";

        SpriteRenderer playerSr = playerGo.AddComponent<SpriteRenderer>();
        playerSr.sprite = Resources.Load<Sprite>("Sprites/Goku") ?? Resources.Load<Sprite>("Goku");
#if UNITY_EDITOR
        if (playerSr.sprite == null) playerSr.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Goku.png");
#endif
        playerSr.sortingLayerName = "Gameplay";
        playerSr.sortingOrder = 5;
        playerGo.transform.localScale = new Vector3(1.1f, 1.1f, 1f);

#if UNITY_EDITOR
        RuntimeAnimatorController playerCtrl = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Animations/PlayerAnimator.controller");
        if (playerCtrl != null)
        {
            Animator anim = playerGo.AddComponent<Animator>();
            anim.runtimeAnimatorController = playerCtrl;
        }
#endif

        GameObject firePointGo = new GameObject("FirePoint");
        firePointGo.transform.SetParent(playerGo.transform);
        firePointGo.transform.localPosition = new Vector3(0.6f, 0.1f, 0);

        PlayerDefender playerDefender = playerGo.AddComponent<PlayerDefender>();
        playerDefender.firePoint = firePointGo.transform;
#if UNITY_EDITOR
        playerDefender.kiBlastPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/KiBlast.prefab");
#endif

        // 6. UI Canvas (GIAI ĐOẠN 6)
        GameObject canvasGo = new GameObject("Canvas");
        Canvas canvas = canvasGo.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasGo.AddComponent<GraphicRaycaster>();

        TextMeshProUGUI hpText = CreateUIText(canvasGo.transform, "HPText", "Máu Ngọc Rồng: 100", new Vector2(40, -40), new Vector2(400, 50), 28, new Color(1f, 0.5f, 0.1f, 1f));
        TextMeshProUGUI goldText = CreateUIText(canvasGo.transform, "GoldText", "Kho Zeni: 0", new Vector2(0, -40), new Vector2(400, 50), 28, Color.yellow, TextAlignmentOptions.Center);
        TextMeshProUGUI waveText = CreateUIText(canvasGo.transform, "WaveText", "Đợt quái: 1", new Vector2(-40, -40), new Vector2(400, 50), 28, Color.cyan, TextAlignmentOptions.Right);

        Button btnUpgradeDmg = CreateButton(canvasGo.transform, "Btn_UpgradeDamage", "Nâng Cấp Chưởng (+15 Dame)\nGiá: 50 Zeni", new Vector2(170, 110), new Vector2(300, 65), new Color(0.9f, 0.4f, 0.1f, 0.95f), out TextMeshProUGUI dmgBtnText);
        Button btnUpgradeSpeed = CreateButton(canvasGo.transform, "Btn_UpgradeSpeed", "Nâng Tốc Bắn\nGiá: 50 Zeni", new Vector2(170, 40), new Vector2(300, 65), new Color(0.15f, 0.6f, 0.9f, 0.95f), out TextMeshProUGUI spdBtnText);

        GameObject gameOverPanel = CreateUIPanel(canvasGo.transform, "GameOverPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(500, 320), new Color(0.15f, 0.05f, 0.05f, 0.96f));
        CreateUIText(gameOverPanel.transform, "Title", "THẤT BẠI\nNGỌC RỒNG ĐÃ BỊ PHÁ HỦY!", new Vector2(0, 70), new Vector2(460, 90), 30, Color.red, TextAlignmentOptions.Center);
        Button btnRestart = CreateButton(gameOverPanel.transform, "Btn_Restart", "CHƠI LẠI", new Vector2(0, -60), new Vector2(200, 55), new Color(0.85f, 0.2f, 0.2f, 1f), out _);
        gameOverPanel.SetActive(false);

        // 7. GameManager & WaveSpawner (GIAI ĐOẠN 5)
        GameObject gmGo = new GameObject("GameManager");
        GameManager gm = gmGo.AddComponent<GameManager>();
        gm.baseHP = 100;
        gm.attackDamage = 25;
        gm.fireRate = 0.4f;
        gm.upgradeDamageCost = 50;
        gm.upgradeSpeedCost = 50;
        gm.hpText = hpText;
        gm.goldText = goldText;
        gm.waveText = waveText;
        gm.upgradeDamageBtnText = dmgBtnText;
        gm.upgradeSpeedBtnText = spdBtnText;
        gm.gameOverPanel = gameOverPanel;

        WaveSpawner spawner = gmGo.AddComponent<WaveSpawner>();
#if UNITY_EDITOR
        spawner.normalEnemyPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/NormalEnemy.prefab");
        spawner.bomberEnemyPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/BomberEnemy.prefab");
#endif
        spawner.spawnPoints = spawnPts;
        spawner.endPoints = endPts;
        spawner.currentWave = 1;
        spawner.baseEnemiesPerWave = 5;
        spawner.timeBetweenSpawns = 1.2f;
        spawner.timeBetweenWaves = 4f;

        btnUpgradeDmg.onClick.AddListener(() => gm.UpgradeWeaponDamage());
        btnUpgradeSpeed.onClick.AddListener(() => gm.UpgradeWeaponSpeed());
        btnRestart.onClick.AddListener(() => gm.RestartGame());

        // 8. EventSystem
        if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        Debug.Log("<color=green><b>[7 Viên Ngọc Rồng] Khởi tạo hoàn tất! Trận chiến bắt đầu!</b></color>");
    }

    private static Transform CreateEmptyChild(Transform parent, string name, Vector3 pos)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.position = pos;
        return go.transform;
    }

    private static GameObject CreateUIPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size, Color color)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = panel.AddComponent<Image>();
        img.color = color;
        return panel;
    }

    private static TextMeshProUGUI CreateUIText(Transform parent, string name, string text, Vector2 pos, Vector2 size, float fontSize, Color color, TextAlignmentOptions alignment = TextAlignmentOptions.Left)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        RectTransform rt = textObj.AddComponent<RectTransform>();

        if (pos.x < 0)
        {
            rt.anchorMin = new Vector2(1, 1);
            rt.anchorMax = new Vector2(1, 1);
        }
        else if (Mathf.Abs(pos.x) < 0.01f)
        {
            rt.anchorMin = new Vector2(0.5f, 1);
            rt.anchorMax = new Vector2(0.5f, 1);
        }
        else
        {
            rt.anchorMin = new Vector2(0, 1);
            rt.anchorMax = new Vector2(0, 1);
        }

        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = alignment;
        tmp.fontStyle = FontStyles.Bold;
        return tmp;
    }

    private static Button CreateButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, Color btnColor, out TextMeshProUGUI labelText)
    {
        GameObject btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        RectTransform rt = btnObj.AddComponent<RectTransform>();
        rt.anchorMin = (pos.y < 0 && pos.x == 0) ? new Vector2(0.5f, 0.5f) : new Vector2(0, 0);
        rt.anchorMax = rt.anchorMin;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;

        Image img = btnObj.AddComponent<Image>();
        img.color = btnColor;

        Button btn = btnObj.AddComponent<Button>();

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRt = textObj.AddComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;

        labelText = textObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 15;
        labelText.color = Color.white;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.fontStyle = FontStyles.Bold;

        return btn;
    }
}
