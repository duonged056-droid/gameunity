using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace DragonBallDefenders
{
    public static class DragonBallRuntimeBootstrapper
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void OnSceneLoaded()
        {
            // If the scene already has a GameManager, everything is already configured
            if (GameManager.Instance != null || Object.FindAnyObjectByType<GameManager>() != null)
            {
                return;
            }

            Debug.Log("<color=cyan><b>[Dragon Ball Z] Initializing Runtime Defense System...</b></color>");
            BootstrapGameScene();
        }

        public static void BootstrapGameScene()
        {
            // 1. Camera & AudioListener & ScreenShake
            Camera cam = Camera.main;
            if (cam == null)
            {
                GameObject camObj = new GameObject("Main Camera");
                cam = camObj.AddComponent<Camera>();
                camObj.tag = "MainCamera";
            }
            cam.orthographic = true;
            cam.orthographicSize = 5.5f;
            cam.transform.position = new Vector3(0, 0, -10);
            cam.backgroundColor = new Color(0.08f, 0.08f, 0.12f, 1f);
            if (cam.GetComponent<AudioListener>() == null) cam.gameObject.AddComponent<AudioListener>();
            if (cam.GetComponent<ScreenShake>() == null) cam.gameObject.AddComponent<ScreenShake>();

            // 2. SoundManager
            GameObject soundGo = new GameObject("SoundManager");
            SoundManager sound = soundGo.AddComponent<SoundManager>();

            // 3. Background
            GameObject bgGo = new GameObject("Environment_Background");
            SpriteRenderer bgSr = bgGo.AddComponent<SpriteRenderer>();
            Sprite bgSprite = Resources.Load<Sprite>("Background") ?? LoadSpriteFromPath("Assets/Sprites/Background.jpg");
            if (bgSprite != null)
            {
                bgSr.sprite = bgSprite;
                bgSr.sortingOrder = -10;
                bgGo.transform.localScale = new Vector3(1.15f, 1.15f, 1f);
                bgGo.transform.position = new Vector3(0.5f, 0, 0);
            }

            // 4. DragonBallAltar
            GameObject altarGo = new GameObject("DragonBallAltar");
            altarGo.transform.position = new Vector3(-7.2f, 0, 0);
            SpriteRenderer altarSr = altarGo.AddComponent<SpriteRenderer>();
            Sprite altarSprite = LoadSpriteFromPath("Assets/Sprites/DragonBallAltar.png");
            if (altarSprite != null)
            {
                altarSr.sprite = altarSprite;
                altarSr.sortingOrder = 2;
                altarGo.transform.localScale = new Vector3(0.38f, 0.38f, 1f);
            }
            BoxCollider2D altarCol = altarGo.AddComponent<BoxCollider2D>();
            altarCol.isTrigger = true;
            altarCol.size = new Vector2(2.5f, 4.5f);
            DragonBallAltar altar = altarGo.AddComponent<DragonBallAltar>();

            // 5. Waypoints (3 Lanes)
            GameObject waypointsRoot = new GameObject("Waypoints_Lanes");
            WaypointPath[] paths = new WaypointPath[3];

            paths[0] = CreateWaypointPath(waypointsRoot.transform, "Lane_Top", Color.cyan,
                new Vector3(9.5f, 2.8f, 0), new Vector3(4f, 3.0f, 0), new Vector3(-1f, 2.4f, 0), new Vector3(-6.0f, 1.0f, 0), new Vector3(-7.2f, 0.3f, 0));

            paths[1] = CreateWaypointPath(waypointsRoot.transform, "Lane_Mid", Color.yellow,
                new Vector3(9.5f, 0.0f, 0), new Vector3(4f, -0.2f, 0), new Vector3(-1f, 0.2f, 0), new Vector3(-6.0f, 0.0f, 0), new Vector3(-7.2f, 0.0f, 0));

            paths[2] = CreateWaypointPath(waypointsRoot.transform, "Lane_Bottom", Color.magenta,
                new Vector3(9.5f, -2.8f, 0), new Vector3(4f, -3.0f, 0), new Vector3(-1f, -2.4f, 0), new Vector3(-6.0f, -1.0f, 0), new Vector3(-7.2f, -0.3f, 0));

            // 6. Defender Slots
            GameObject slotsRoot = new GameObject("DefenderSlots");
            CreateDefenderSlot(slotsRoot.transform, "Slot_Top", new Vector3(-5.8f, 2.2f, 0));
            CreateDefenderSlot(slotsRoot.transform, "Slot_MidTop", new Vector3(-4.5f, 1.1f, 0));
            CreateDefenderSlot(slotsRoot.transform, "Slot_MidBottom", new Vector3(-4.5f, -1.1f, 0));
            CreateDefenderSlot(slotsRoot.transform, "Slot_Bottom", new Vector3(-5.8f, -2.2f, 0));

            // 7. Goku Player
            GameObject gokuGo = new GameObject("SonGoku_Player");
            gokuGo.transform.position = new Vector3(-4.8f, 0, 0);
            gokuGo.tag = "Player";
            SpriteRenderer gokuSr = gokuGo.AddComponent<SpriteRenderer>();
            Sprite gokuNormal = LoadSpriteFromPath("Assets/Sprites/Goku.png");
            Sprite gokuSSJ = LoadSpriteFromPath("Assets/Sprites/GokuSSJ.png");
            if (gokuNormal != null)
            {
                gokuSr.sprite = gokuNormal;
                gokuSr.sortingOrder = 10;
                gokuGo.transform.localScale = new Vector3(0.28f, 0.28f, 1f);
            }
            BoxCollider2D gokuCol = gokuGo.AddComponent<BoxCollider2D>();
            gokuCol.isTrigger = true;
            gokuCol.size = new Vector2(3f, 4.5f);

            // Aura
            GameObject auraObj = new GameObject("SSJAura");
            auraObj.transform.SetParent(gokuGo.transform);
            auraObj.transform.localPosition = Vector3.zero;
            auraObj.transform.localScale = Vector3.one * 1.3f;
            SpriteRenderer auraSr = auraObj.AddComponent<SpriteRenderer>();
            auraSr.sprite = LoadSpriteFromPath("Assets/Sprites/SpiritBomb.png");
            auraSr.color = new Color(1f, 0.95f, 0.2f, 0.4f);
            auraSr.sortingOrder = 9;
            auraObj.SetActive(false);

            PlayerController pc = gokuGo.AddComponent<PlayerController>();
            DefenderAttack defAttack = gokuGo.AddComponent<DefenderAttack>();

            // 8. GameManager
            GameObject gmGo = new GameObject("GameManager");
            GameManager gm = gmGo.AddComponent<GameManager>();
            gm.dragonBallAltar = altar;

            // 9. UpgradeManager
            GameObject upgGo = new GameObject("UpgradeManager");
            UpgradeManager upg = upgGo.AddComponent<UpgradeManager>();
            upg.playerAttack = defAttack;

            // 10. WaveSpawner
            GameObject spawnerGo = new GameObject("WaveSpawner");
            WaveSpawner spawner = spawnerGo.AddComponent<WaveSpawner>();
            spawner.paths = paths;

            // 11. Canvas UI
            BuildRuntimeUI(gm, altar, defAttack, spawner, upg);

            // 12. EventSystem
            if (Object.FindAnyObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
            {
                GameObject esGo = new GameObject("EventSystem");
                esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
                esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
            }

            Debug.Log("<color=green><b>[Dragon Ball Z] Runtime Bootstrap Complete! Game is live!</b></color>");
        }

        private static WaypointPath CreateWaypointPath(Transform parent, string name, Color color, params Vector3[] points)
        {
            GameObject pathGo = new GameObject(name);
            pathGo.transform.SetParent(parent);
            WaypointPath wp = pathGo.AddComponent<WaypointPath>();
            wp.pathColor = color;
            wp.waypoints = new Transform[points.Length];

            for (int i = 0; i < points.Length; i++)
            {
                GameObject ptGo = new GameObject("WP_" + i);
                ptGo.transform.SetParent(pathGo.transform);
                ptGo.transform.position = points[i];
                wp.waypoints[i] = ptGo.transform;
            }

            return wp;
        }

        private static void CreateDefenderSlot(Transform parent, string name, Vector3 pos)
        {
            GameObject slotGo = new GameObject(name);
            slotGo.transform.SetParent(parent);
            slotGo.transform.position = pos;

            SpriteRenderer sr = slotGo.AddComponent<SpriteRenderer>();
            sr.sprite = LoadSpriteFromPath("Assets/Sprites/KiBlast.png");
            sr.color = new Color(0.2f, 1f, 0.4f, 0.6f);
            sr.sortingOrder = 1;
            slotGo.transform.localScale = Vector3.one * 0.9f;

            slotGo.AddComponent<DefenderSlot>();
        }

        private static Sprite LoadSpriteFromPath(string path)
        {
#if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(path);
#else
            return null;
#endif
        }

        private static void BuildRuntimeUI(GameManager gm, DragonBallAltar altar, DefenderAttack playerAttack, WaveSpawner spawner, UpgradeManager upg)
        {
            GameObject canvasGo = new GameObject("Canvas_GameHUD");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            UIManager ui = canvasGo.AddComponent<UIManager>();
            gm.uiManager = ui;
            ui.playerAttack = playerAttack;

            // Top Bar
            GameObject topBar = CreateUIPanel(canvasGo.transform, "TopBar", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -90), new Vector2(0, 90), new Color(0.05f, 0.05f, 0.1f, 0.85f));

            ui.baseHealthSlider = CreateProgressBar(topBar.transform, "AltarHP_Bar", new Vector2(20, -25), new Vector2(360, 28), new Color(1f, 0.45f, 0.05f, 1f));
            ui.baseHealthText = CreateUIText(topBar.transform, "AltarHP_Text", "Ngọc Rồng: 1000 / 1000", new Vector2(20, -25), new Vector2(360, 28), 18, Color.white);

            ui.kiSlider = CreateProgressBar(topBar.transform, "Ki_Bar", new Vector2(20, -62), new Vector2(360, 20), new Color(0.15f, 0.75f, 1f, 1f));
            ui.kiText = CreateUIText(topBar.transform, "Ki_Text", "Ki: 100 / 100", new Vector2(20, -62), new Vector2(360, 20), 15, Color.white);

            ui.waveText = CreateUIText(topBar.transform, "Wave_Text", "ĐỢT: 1 / 10", new Vector2(0, -25), new Vector2(400, 35), 26, Color.yellow, TextAlignmentOptions.Center);
            ui.countdownText = CreateUIText(topBar.transform, "Countdown_Text", "CHUẨN BỊ CHIẾN ĐẤU!", new Vector2(0, -62), new Vector2(450, 25), 18, Color.cyan, TextAlignmentOptions.Center);

            ui.zeniText = CreateUIText(topBar.transform, "Zeni_Text", "Zeni: 300 G", new Vector2(-220, -25), new Vector2(200, 30), 22, new Color(1f, 0.85f, 0.1f, 1f), TextAlignmentOptions.Right);
            ui.scoreText = CreateUIText(topBar.transform, "Score_Text", "Score: 0", new Vector2(-220, -60), new Vector2(200, 25), 18, Color.white, TextAlignmentOptions.Right);

            // Skill Hotbar
            GameObject skillBar = CreateUIPanel(canvasGo.transform, "SkillHotbar", new Vector2(1, 0), new Vector2(1, 0), new Vector2(-260, 70), new Vector2(500, 110), Color.clear);
            ui.kamehamehaButton = CreateSkillButton(skillBar.transform, "Btn_Kamehameha", "KAMEHAMEHA [Q]\n40 Ki", new Vector2(-360, 0), new Color(0.1f, 0.5f, 0.9f, 0.9f), out ui.kamehamehaCooldownFill);
            ui.spiritBombButton = CreateSkillButton(skillBar.transform, "Btn_SpiritBomb", "QUẢ CẦU KINH KHÍ [E]\n60 Ki", new Vector2(-200, 0), new Color(0.2f, 0.8f, 1f, 0.9f), out ui.spiritBombCooldownFill);
            ui.superSaiyanButton = CreateSkillButton(skillBar.transform, "Btn_SuperSaiyan", "SUPER SAIYAN [R]\n50 Ki", new Vector2(-40, 0), new Color(1f, 0.8f, 0.1f, 0.9f), out _);

            // Shop Toggle
            ui.shopToggleButton = CreateActionButton(canvasGo.transform, "Btn_ShopToggle", "CỬA HÀNG / NÂNG CẤP [B]", new Vector2(150, 60), new Vector2(260, 55), new Color(0.9f, 0.5f, 0.1f, 0.95f));

            // Shop Panel
            ui.shopPanel = CreateUIPanel(canvasGo.transform, "ShopPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(750, 520), new Color(0.08f, 0.1f, 0.15f, 0.96f));
            CreateUIText(ui.shopPanel.transform, "ShopHeader", "⚡ KHO TRANG BỊ & NÂNG CẤP CAPSULE CORP ⚡", new Vector2(0, 220), new Vector2(700, 45), 24, Color.yellow, TextAlignmentOptions.Center);

            ui.upgradeDamageButton = CreateShopItemButton(ui.shopPanel.transform, "Btn_UpgDamage", "Tăng Sát Thương Chưởng (+25%)", new Vector2(-170, 140), out ui.damageUpgradeCostText);
            ui.upgradeFireRateButton = CreateShopItemButton(ui.shopPanel.transform, "Btn_UpgFireRate", "Tăng Tốc Độ Bắn Ki Blast (+20%)", new Vector2(-170, 60), out ui.fireRateUpgradeCostText);
            ui.upgradeSsjButton = CreateShopItemButton(ui.shopPanel.transform, "Btn_UpgSSJ", "Tăng Thời Lượng Super Saiyan", new Vector2(-170, -20), out ui.ssjUpgradeCostText);
            ui.buySenzuHealButton = CreateShopItemButton(ui.shopPanel.transform, "Btn_SenzuHeal", "Đậu Thần Senzu (Hồi +250 Máu Căn Cứ)", new Vector2(-170, -100), out _);

            ui.deployBulmaTurretButton = CreateActionButton(ui.shopPanel.transform, "Btn_DeployBulma", "Đặt Pháo Tự Động Bulma\n(150 Zeni)", new Vector2(200, 130), new Vector2(240, 65), new Color(0.1f, 0.65f, 0.85f, 1f));
            ui.deployVegetaButton = CreateActionButton(ui.shopPanel.transform, "Btn_DeployVegeta", "Triệu Hồi Đồng Đội Vegeta\n(250 Zeni)", new Vector2(200, 50), new Vector2(240, 65), new Color(0.2f, 0.35f, 0.85f, 1f));
            ui.deployPiccoloButton = CreateActionButton(ui.shopPanel.transform, "Btn_DeployPiccolo", "Triệu Hồi Đồng Đội Piccolo\n(300 Zeni)", new Vector2(200, -30), new Vector2(240, 65), new Color(0.35f, 0.7f, 0.25f, 1f));

            Button closeShopBtn = CreateActionButton(ui.shopPanel.transform, "Btn_CloseShop", "ĐÓNG [B]", new Vector2(0, -200), new Vector2(200, 45), Color.red);
            closeShopBtn.onClick.AddListener(() => ui.ToggleShopPanel());

            // Boss Warning Banner
            ui.bossWarningBanner = CreateUIPanel(canvasGo.transform, "BossWarningBanner", new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.8f), Vector2.zero, new Vector2(900, 75), new Color(0.85f, 0.1f, 0.1f, 0.95f));
            ui.bossWarningText = CreateUIText(ui.bossWarningBanner.transform, "BossWarningText", "CẢNH BÁO: TRÙM CUỐI ĐANG TIẾN ĐẾN!", Vector2.zero, new Vector2(880, 60), 26, Color.yellow, TextAlignmentOptions.Center);

            // Game Over Panel
            ui.gameOverPanel = CreateUIPanel(canvasGo.transform, "GameOverPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600, 400), new Color(0.15f, 0.05f, 0.05f, 0.98f));
            CreateUIText(ui.gameOverPanel.transform, "Title", "GAME OVER\nNGỌC RỒNG ĐÃ BỊ HỦY DIỆT!", new Vector2(0, 100), new Vector2(550, 90), 30, Color.red, TextAlignmentOptions.Center);
            ui.gameOverScoreText = CreateUIText(ui.gameOverPanel.transform, "FinalScore", "Điểm: 0", new Vector2(0, 10), new Vector2(500, 40), 22, Color.white, TextAlignmentOptions.Center);
            ui.gameOverRestartButton = CreateActionButton(ui.gameOverPanel.transform, "Btn_Restart", "CHƠI LẠI NGAY", new Vector2(0, -90), new Vector2(240, 60), new Color(0.9f, 0.2f, 0.2f, 1f));

            // Victory Panel
            ui.victoryPanel = CreateUIPanel(canvasGo.transform, "VictoryPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(650, 420), new Color(0.05f, 0.15f, 0.08f, 0.98f));
            CreateUIText(ui.victoryPanel.transform, "VictoryTitle", "CHIẾN THẮNG HUY HOÀNG!\nBẢO VỆ THÀNH CÔNG 7 VIÊN NGỌC RỒNG!", new Vector2(0, 100), new Vector2(600, 90), 28, Color.yellow, TextAlignmentOptions.Center);
            ui.victoryScoreText = CreateUIText(ui.victoryPanel.transform, "VictoryScore", "Điểm: 0", new Vector2(0, 10), new Vector2(500, 40), 22, Color.cyan, TextAlignmentOptions.Center);
            ui.victoryRestartButton = CreateActionButton(ui.victoryPanel.transform, "Btn_VictoryRestart", "CHƠI LẠI MÀN MỚI", new Vector2(0, -90), new Vector2(260, 60), new Color(0.1f, 0.8f, 0.3f, 1f));
        }

        private static GameObject CreateUIPanel(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 size, Color color)
        {
            GameObject panel = new GameObject(name);
            panel.transform.SetParent(parent, false);
            RectTransform rt = panel.AddComponent<RectTransform>();
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta = size;

            if (color.a > 0.01f)
            {
                Image img = panel.AddComponent<Image>();
                img.color = color;
            }
            return panel;
        }

        private static TextMeshProUGUI CreateUIText(Transform parent, string name, string text, Vector2 pos, Vector2 size, float fontSize, Color color, TextAlignmentOptions alignment = TextAlignmentOptions.Left)
        {
            GameObject textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            RectTransform rt = textObj.AddComponent<RectTransform>();
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

        private static Slider CreateProgressBar(Transform parent, string name, Vector2 pos, Vector2 size, Color fillColor)
        {
            GameObject sliderObj = new GameObject(name);
            sliderObj.transform.SetParent(parent, false);
            RectTransform rt = sliderObj.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image bg = sliderObj.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.2f, 0.8f);

            Slider slider = sliderObj.AddComponent<Slider>();

            GameObject fillArea = new GameObject("FillArea");
            fillArea.transform.SetParent(sliderObj.transform, false);
            RectTransform fillAreaRt = fillArea.AddComponent<RectTransform>();
            fillAreaRt.anchorMin = Vector2.zero;
            fillAreaRt.anchorMax = Vector2.one;
            fillAreaRt.sizeDelta = Vector2.zero;

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillRt = fill.AddComponent<RectTransform>();
            fillRt.anchorMin = Vector2.zero;
            fillRt.anchorMax = Vector2.one;
            fillRt.sizeDelta = Vector2.zero;

            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = fillColor;

            slider.fillRect = fillRt;
            return slider;
        }

        private static Button CreateActionButton(Transform parent, string name, string label, Vector2 pos, Vector2 size, Color btnColor)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            RectTransform rt = btnObj.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = size;

            Image img = btnObj.AddComponent<Image>();
            img.color = btnColor;

            Button btn = btnObj.AddComponent<Button>();
            CreateUIText(btnObj.transform, "Label", label, Vector2.zero, size, 16, Color.white, TextAlignmentOptions.Center);
            return btn;
        }

        private static Button CreateSkillButton(Transform parent, string name, string label, Vector2 pos, Color btnColor, out Image cooldownFill)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            RectTransform rt = btnObj.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(145, 95);

            Image img = btnObj.AddComponent<Image>();
            img.color = btnColor;

            Button btn = btnObj.AddComponent<Button>();
            CreateUIText(btnObj.transform, "Label", label, Vector2.zero, new Vector2(140, 85), 14, Color.white, TextAlignmentOptions.Center);

            GameObject cdObj = new GameObject("CooldownOverlay");
            cdObj.transform.SetParent(btnObj.transform, false);
            RectTransform cdRt = cdObj.AddComponent<RectTransform>();
            cdRt.anchorMin = Vector2.zero;
            cdRt.anchorMax = Vector2.one;
            cdRt.sizeDelta = Vector2.zero;

            cooldownFill = cdObj.AddComponent<Image>();
            cooldownFill.color = new Color(0, 0, 0, 0.7f);
            cooldownFill.type = Image.Type.Filled;
            cooldownFill.fillMethod = Image.FillMethod.Radial360;
            cooldownFill.fillOrigin = (int)Image.Origin360.Top;
            cooldownFill.fillAmount = 0f;

            return btn;
        }

        private static Button CreateShopItemButton(Transform parent, string name, string label, Vector2 pos, out TextMeshProUGUI costText)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent, false);
            RectTransform rt = btnObj.AddComponent<RectTransform>();
            rt.anchoredPosition = pos;
            rt.sizeDelta = new Vector2(340, 65);

            Image img = btnObj.AddComponent<Image>();
            img.color = new Color(0.2f, 0.25f, 0.35f, 0.95f);

            Button btn = btnObj.AddComponent<Button>();
            CreateUIText(btnObj.transform, "Title", label, new Vector2(-15, 10), new Vector2(300, 30), 15, Color.white);
            costText = CreateUIText(btnObj.transform, "Cost", "100 G", new Vector2(-15, -15), new Vector2(300, 25), 14, Color.yellow);
            return btn;
        }
    }
}
