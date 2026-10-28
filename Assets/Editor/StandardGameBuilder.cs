using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace DragonBallDefenders.Editor
{
    [InitializeOnLoad]
    public class StandardGameBuilder : EditorWindow
    {
        static StandardGameBuilder()
        {
            EditorApplication.delayCall += () =>
            {
                string standardScenePath = "Assets/Scenes/DragonBall_Standard.unity";
                if (!File.Exists(standardScenePath) || !File.Exists("Assets/Prefabs/NormalEnemy.prefab"))
                {
                    Debug.Log("<color=yellow><b>[Dragon Ball Z] Auto-generating Standard 6-Stage Game Scene & Assets...</b></color>");
                    BuildCompleteStandardGame();
                }
            };
        }

        [MenuItem("Dragon Ball Z/Build Standard 6-Stage Game")]
        public static void BuildCompleteStandardGame()
        {
            EnsureDirectories();
            AssetDatabase.Refresh();

            // 1. Load sub-sprites from sprite sheets
            Sprite[] gokuSprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Goku_Sheet.png").OfType<Sprite>().OrderBy(s => s.name).ToArray();
            Sprite[] rrSprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/RedRibbon_Sheet.png").OfType<Sprite>().OrderBy(s => s.name).ToArray();
            Sprite[] saibamenSprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Saibamen_Sheet.png").OfType<Sprite>().OrderBy(s => s.name).ToArray();
            Sprite[] itemSprites = AssetDatabase.LoadAllAssetsAtPath("Assets/Sprites/Items_Sheet.png").OfType<Sprite>().ToArray();

            Sprite altarSprite = itemSprites.FirstOrDefault(s => s.name.Contains("Altar")) ?? AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/DragonBallAltar.png");
            Sprite kiBlastSprite = itemSprites.FirstOrDefault(s => s.name.Contains("KiBlast")) ?? AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/KiBlast.png");
            Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Background.png") ?? AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Background.jpg");

            // 2. Generate Animation Clips
            AnimationClip gokuIdleClip = CreateSpriteAnimation("Assets/Animations/Goku_Idle.anim", gokuSprites.Where(s => s.name.Contains("Idle")).ToArray(), 0.25f, true);
            AnimationClip gokuAttackClip = CreateSpriteAnimation("Assets/Animations/Goku_Attack.anim", gokuSprites.Where(s => s.name.Contains("Attack")).ToArray(), 0.12f, false);
            AnimationClip rrRunClip = CreateSpriteAnimation("Assets/Animations/NormalEnemy_Run.anim", rrSprites, 0.15f, true);
            AnimationClip saibamenRunClip = CreateSpriteAnimation("Assets/Animations/Bomber_Run.anim", saibamenSprites, 0.12f, true);

            // 3. Generate Animator Controllers
            AnimatorController playerAnimator = CreatePlayerAnimator(gokuIdleClip, gokuAttackClip);
            AnimatorController rrAnimator = CreateSimpleAnimator("Assets/Animations/NormalEnemyAnimator.controller", "NormalEnemy_Run", rrRunClip);
            AnimatorController saibamenAnimator = CreateSimpleAnimator("Assets/Animations/BomberEnemyAnimator.controller", "Bomber_Run", saibamenRunClip);

            // 4. Create Prefabs (GIAI ĐOẠN 3)
            GameObject kiBlastPrefab = CreateKiBlastPrefab(kiBlastSprite);
            GameObject normalEnemyPrefab = CreateEnemyPrefab("Assets/Prefabs/NormalEnemy.prefab", rrSprites.FirstOrDefault(), rrAnimator, false, 2.0f, 50, 10, 15);
            GameObject bomberEnemyPrefab = CreateEnemyPrefab("Assets/Prefabs/BomberEnemy.prefab", saibamenSprites.FirstOrDefault(), saibamenAnimator, true, 3.5f, 35, 15, 25);

            // 5. Build Scene (GIAI ĐOẠN 4 & 6)
            BuildStandardScene(bgSprite, altarSprite, gokuSprites.FirstOrDefault(), playerAnimator, kiBlastPrefab, normalEnemyPrefab, bomberEnemyPrefab);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("<color=green><b>[Dragon Ball Z] Giai đoạn 1 -> 6 đã hoàn tất thành công! Màn chơi DragonBall_Standard đã sẵn sàng!</b></color>");
            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog("7 Viên Ngọc Rồng - Thủ Thành Theo Đợt", "Toàn bộ 6 giai đoạn chuẩn đã được thiết lập thành công!\n\n1. Sprite Sheet & Cắt ảnh (Goku, Saibamen, Red Ribbon, Ngọc Rồng)\n2. Sorting Layers (Background, Gameplay, UI)\n3. Animations & Animator Controller (Idle, Attack Trigger, Run)\n4. Prefabs (KiBlast, NormalEnemy, BomberEnemy)\n5. Bộ 5 Script cốt lõi (GameManager, PlayerDefender, KiBlast, Enemy, WaveSpawner)\n6. Canvas UI & Hệ thống nâng cấp sức mạnh\n\nBấm nút PLAY trên Unity để trải nghiệm ngay!", "BẮT ĐẦU CHƠI");
            }
        }

        private static void EnsureDirectories()
        {
            string[] dirs = { "Assets/Sprites", "Assets/Animations", "Assets/Prefabs", "Assets/Scripts/Standard", "Assets/UI", "Assets/Scenes" };
            foreach (var d in dirs)
            {
                if (!Directory.Exists(d)) Directory.CreateDirectory(d);
            }
        }

        private static AnimationClip CreateSpriteAnimation(string path, Sprite[] frames, float frameDuration, bool isLooping)
        {
            if (frames == null || frames.Length == 0) return null;

            AnimationClip clip = new AnimationClip();
            clip.frameRate = 30;

            EditorCurveBinding binding = new EditorCurveBinding();
            binding.type = typeof(SpriteRenderer);
            binding.path = "";
            binding.propertyName = "m_Sprite";

            ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[frames.Length];
            for (int i = 0; i < frames.Length; i++)
            {
                keyframes[i] = new ObjectReferenceKeyframe();
                keyframes[i].time = i * frameDuration;
                keyframes[i].value = frames[i];
            }

            AnimationUtility.SetObjectReferenceCurve(clip, binding, keyframes);

            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = isLooping;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            AssetDatabase.CreateAsset(clip, path);
            return clip;
        }

        private static AnimatorController CreatePlayerAnimator(AnimationClip idleClip, AnimationClip attackClip)
        {
            string path = "Assets/Animations/PlayerAnimator.controller";
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);

            controller.AddParameter("Attack", AnimatorControllerParameterType.Trigger);

            AnimatorStateMachine sm = controller.layers[0].stateMachine;

            AnimatorState idleState = sm.AddState("Goku_Idle");
            idleState.motion = idleClip;
            sm.defaultState = idleState;

            AnimatorState attackState = sm.AddState("Goku_Attack");
            attackState.motion = attackClip;

            // Idle -> Attack transition
            AnimatorStateTransition toAttack = idleState.AddTransition(attackState);
            toAttack.hasExitTime = false;
            toAttack.duration = 0f;
            toAttack.AddCondition(AnimatorConditionMode.If, 0, "Attack");

            // Attack -> Idle transition
            AnimatorStateTransition toIdle = attackState.AddTransition(idleState);
            toIdle.hasExitTime = true;
            toIdle.exitTime = 0.85f;
            toIdle.duration = 0.05f;

            return controller;
        }

        private static AnimatorController CreateSimpleAnimator(string path, string stateName, AnimationClip clip)
        {
            AnimatorController controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            AnimatorStateMachine sm = controller.layers[0].stateMachine;
            AnimatorState state = sm.AddState(stateName);
            state.motion = clip;
            sm.defaultState = state;
            return controller;
        }

        private static GameObject CreateKiBlastPrefab(Sprite sprite)
        {
            string pPath = "Assets/Prefabs/KiBlast.prefab";
            GameObject go = new GameObject("KiBlast");

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
            sr.sortingLayerName = "Gameplay";
            sr.sortingOrder = 10;
            go.transform.localScale = Vector3.one * 0.8f;

            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            CircleCollider2D col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.35f;

            KiBlast kb = go.AddComponent<KiBlast>();
            kb.speed = 12f;
            kb.lifeTime = 3f;

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, pPath);
            DestroyImmediate(go);
            return prefab;
        }

        private static GameObject CreateEnemyPrefab(string path, Sprite defaultSprite, RuntimeAnimatorController animController, bool isBomber, float speed, int hp, int dmg, int gold)
        {
            string name = isBomber ? "BomberEnemy" : "NormalEnemy";
            GameObject go = new GameObject(name);
            go.tag = "Enemy";

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = defaultSprite;
            sr.sortingLayerName = "Gameplay";
            sr.sortingOrder = 5;
            if (!isBomber) sr.flipX = true; // Faces left towards Goku

            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            CapsuleCollider2D col = go.AddComponent<CapsuleCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.8f, 0.9f);

            if (animController != null)
            {
                Animator anim = go.AddComponent<Animator>();
                anim.runtimeAnimatorController = animController;
            }

            Enemy enemy = go.AddComponent<Enemy>();
            enemy.isBomber = isBomber;
            enemy.baseSpeed = speed;
            enemy.maxHP = hp;
            enemy.damageToBase = dmg;
            enemy.rewardGold = gold;

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
            DestroyImmediate(go);
            return prefab;
        }

        private static void BuildStandardScene(Sprite bgSprite, Sprite altarSprite, Sprite gokuDefaultSprite, RuntimeAnimatorController playerController, GameObject kiBlastPrefab, GameObject normalEnemyPrefab, GameObject bomberEnemyPrefab)
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Main Camera
            GameObject camGo = new GameObject("Main Camera");
            Camera cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.0f;
            cam.backgroundColor = new Color(0.08f, 0.08f, 0.12f, 1f);
            camGo.transform.position = new Vector3(0, 0, -10f);
            camGo.AddComponent<AudioListener>();
            camGo.tag = "MainCamera";

            // 2. Background
            GameObject bgGo = new GameObject("Background");
            SpriteRenderer bgSr = bgGo.AddComponent<SpriteRenderer>();
            bgSr.sprite = bgSprite;
            bgSr.sortingLayerName = "Background";
            bgSr.sortingOrder = -10;
            bgGo.transform.position = new Vector3(0, 0, 0);
            bgGo.transform.localScale = new Vector3(1.2f, 1.2f, 1f);

            // 3. DragonBallAltar (Base)
            GameObject altarGo = new GameObject("DragonBallAltar");
            altarGo.transform.position = new Vector3(-7.5f, 0, 0);
            altarGo.tag = "Base";
            SpriteRenderer altarSr = altarGo.AddComponent<SpriteRenderer>();
            altarSr.sprite = altarSprite;
            altarSr.sortingLayerName = "Gameplay";
            altarSr.sortingOrder = 2;
            altarGo.transform.localScale = new Vector3(0.4f, 0.4f, 1f);

            BoxCollider2D altarCol = altarGo.AddComponent<BoxCollider2D>();
            altarCol.isTrigger = true;
            altarCol.size = new Vector2(2f, 10f); // Blocks entire left edge

            // 4. SpawnPoints & EndPoints (3 Lanes)
            GameObject spawnRoot = new GameObject("SpawnPoints");
            spawnRoot.transform.position = Vector3.zero;
            Transform[] spawnPts = new Transform[3];
            spawnPts[0] = CreateEmptyChild(spawnRoot.transform, "Spawn_Top", new Vector3(8.5f, 3.0f, 0));
            spawnPts[1] = CreateEmptyChild(spawnRoot.transform, "Spawn_Mid", new Vector3(8.5f, 0.0f, 0));
            spawnPts[2] = CreateEmptyChild(spawnRoot.transform, "Spawn_Bot", new Vector3(8.5f, -3.0f, 0));

            GameObject endRoot = new GameObject("EndPoints");
            endRoot.transform.position = Vector3.zero;
            Transform[] endPts = new Transform[3];
            endPts[0] = CreateEmptyChild(endRoot.transform, "End_Top", new Vector3(-7.5f, 3.0f, 0));
            endPts[1] = CreateEmptyChild(endRoot.transform, "End_Mid", new Vector3(-7.5f, 0.0f, 0));
            endPts[2] = CreateEmptyChild(endRoot.transform, "End_Bot", new Vector3(-7.5f, -3.0f, 0));

            // 5. Player Goku
            GameObject playerGo = new GameObject("Player");
            playerGo.transform.position = new Vector3(-5.5f, 0, 0);
            playerGo.tag = "Player";

            SpriteRenderer playerSr = playerGo.AddComponent<SpriteRenderer>();
            playerSr.sprite = gokuDefaultSprite;
            playerSr.sortingLayerName = "Gameplay";
            playerSr.sortingOrder = 5;
            playerGo.transform.localScale = new Vector3(1.1f, 1.1f, 1f);

            if (playerController != null)
            {
                Animator anim = playerGo.AddComponent<Animator>();
                anim.runtimeAnimatorController = playerController;
            }

            GameObject firePointGo = new GameObject("FirePoint");
            firePointGo.transform.SetParent(playerGo.transform);
            firePointGo.transform.localPosition = new Vector3(0.6f, 0.1f, 0);

            PlayerDefender playerDefender = playerGo.AddComponent<PlayerDefender>();
            playerDefender.kiBlastPrefab = kiBlastPrefab;
            playerDefender.firePoint = firePointGo.transform;
            playerDefender.moveSpeed = 7f;
            playerDefender.minY = -3.5f;
            playerDefender.maxY = 3.5f;

            // 6. UI Canvas
            GameObject canvasGo = new GameObject("Canvas");
            Canvas canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGo.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasGo.AddComponent<GraphicRaycaster>();

            // Top Status Bar Texts
            TextMeshProUGUI hpText = CreateUIText(canvasGo.transform, "HPText", "Máu Ngọc Rồng: 100", new Vector2(40, -40), new Vector2(400, 50), 28, new Color(1f, 0.5f, 0.1f, 1f));
            TextMeshProUGUI goldText = CreateUIText(canvasGo.transform, "GoldText", "Kho Zeni: 0", new Vector2(0, -40), new Vector2(400, 50), 28, Color.yellow, TextAlignmentOptions.Center);
            TextMeshProUGUI waveText = CreateUIText(canvasGo.transform, "WaveText", "Đợt quái: 1", new Vector2(-40, -40), new Vector2(400, 50), 28, Color.cyan, TextAlignmentOptions.Right);

            // Bottom Left Upgrade Buttons
            Button btnUpgradeDmg = CreateButton(canvasGo.transform, "Btn_UpgradeDamage", "Nâng Cấp Chưởng (+15 Dame)\nGiá: 50 Zeni", new Vector2(170, 110), new Vector2(300, 65), new Color(0.9f, 0.4f, 0.1f, 0.95f), out TextMeshProUGUI dmgBtnText);
            Button btnUpgradeSpeed = CreateButton(canvasGo.transform, "Btn_UpgradeSpeed", "Nâng Tốc Bắn\nGiá: 50 Zeni", new Vector2(170, 40), new Vector2(300, 65), new Color(0.15f, 0.6f, 0.9f, 0.95f), out TextMeshProUGUI spdBtnText);

            // Game Over Panel
            GameObject gameOverPanel = CreateUIPanel(canvasGo.transform, "GameOverPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(500, 320), new Color(0.15f, 0.05f, 0.05f, 0.96f));
            CreateUIText(gameOverPanel.transform, "Title", "THẤT BẠI\nNGỌC RỒNG ĐÃ BỊ PHÁ HỦY!", new Vector2(0, 70), new Vector2(460, 90), 30, Color.red, TextAlignmentOptions.Center);
            Button btnRestart = CreateButton(gameOverPanel.transform, "Btn_Restart", "CHƠI LẠI", new Vector2(0, -60), new Vector2(200, 55), new Color(0.85f, 0.2f, 0.2f, 1f), out _);
            gameOverPanel.SetActive(false);

            // 7. GameManager & WaveSpawner
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
            spawner.normalEnemyPrefab = normalEnemyPrefab;
            spawner.bomberEnemyPrefab = bomberEnemyPrefab;
            spawner.spawnPoints = spawnPts;
            spawner.endPoints = endPts;
            spawner.currentWave = 1;
            spawner.baseEnemiesPerWave = 5;
            spawner.timeBetweenSpawns = 1.2f;
            spawner.timeBetweenWaves = 4f;

            // Wire UI Click Listeners
            btnUpgradeDmg.onClick.AddListener(() => gm.UpgradeWeaponDamage());
            btnUpgradeSpeed.onClick.AddListener(() => gm.UpgradeWeaponSpeed());
            btnRestart.onClick.AddListener(() => gm.RestartGame());

            // 8. EventSystem
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // Save Scene
            string standardScene = "Assets/Scenes/DragonBall_Standard.unity";
            string sampleScene = "Assets/Scenes/SampleScene.unity";
            EditorSceneManager.SaveScene(newScene, standardScene);
            EditorSceneManager.SaveScene(newScene, sampleScene);

            // Register in Build Settings
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[] {
                new EditorBuildSettingsScene(standardScene, true),
                new EditorBuildSettingsScene(sampleScene, true)
            };
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

            // Calculate anchor based on sign of position
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
}
