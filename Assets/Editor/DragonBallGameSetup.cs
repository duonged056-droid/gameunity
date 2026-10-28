using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;
using DragonBallDefenders;

namespace DragonBallDefenders.Editor
{
    public class DragonBallGameSetup : EditorWindow
    {
        [MenuItem("Dragon Ball Z/Setup Complete Game Scene")]
        public static void SetupGame()
        {
            EnsureTagsExist();
            ConfigureImportedSprites();
            GenerateMissingProceduralSprites();
            AssetDatabase.Refresh();

            GameObject[] prefabs = CreateAllPrefabs();
            BuildGameScene(prefabs);

            Debug.Log("<color=green><b>[Dragon Ball Z] Setup Complete! The game scene is 100% configured and ready to play!</b></color>");
            EditorUtility.DisplayDialog("Dragon Ball: Bảo Vệ Ngọc Rồng", "Game scene has been completely generated and configured!\n\nPress PLAY in Unity to start defending the 7 Dragon Balls!", "OK");
        }

        private static void EnsureTagsExist()
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            string[] requiredTags = { "Enemy", "Base", "Projectile", "DropItem", "Defender", "Player" };
            foreach (string t in requiredTags)
            {
                bool found = false;
                for (int i = 0; i < tagsProp.arraySize; i++)
                {
                    SerializedProperty p = tagsProp.GetArrayElementAtIndex(i);
                    if (p.stringValue.Equals(t)) { found = true; break; }
                }
                if (!found)
                {
                    tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
                    tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = t;
                }
            }
            tagManager.ApplyModifiedProperties();
        }

        private static void ConfigureImportedSprites()
        {
            string[] paths = {
                "Assets/Sprites/Background.jpg",
                "Assets/Sprites/DragonBallAltar.png",
                "Assets/Sprites/Goku.png"
            };

            foreach (string path in paths)
            {
                if (File.Exists(path))
                {
                    TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
                    if (importer != null)
                    {
                        importer.textureType = TextureImporterType.Sprite;
                        importer.spritePixelsPerUnit = 100;
                        importer.alphaIsTransparency = true;
                        importer.SaveAndReimport();
                    }
                }
            }
        }

        private static void GenerateMissingProceduralSprites()
        {
            string dir = "Assets/Sprites/";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

            // Super Saiyan Goku sprite
            CreateProceduralTextureIfMissing(dir + "GokuSSJ.png", 64, 64, (x, y) => {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));
                if (dist < 26)
                {
                    if (y > 36) return new Color(1f, 0.9f, 0.1f, 1f); // Golden spiky hair
                    if (y > 24) return new Color(1f, 0.85f, 0.7f, 1f); // Face
                    return new Color(1f, 0.5f, 0.05f, 1f); // Orange Gi
                }
                if (dist < 31) return new Color(1f, 0.95f, 0.3f, 0.6f); // Golden Aura flame
                return Color.clear;
            });

            // Saibamen Bomber sprite
            CreateProceduralTextureIfMissing(dir + "Saibamen.png", 48, 48, (x, y) => {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(24, 24));
                if (dist < 18)
                {
                    // Eyes
                    if (y >= 26 && y <= 29 && (x == 18 || x == 30)) return Color.red;
                    // Body
                    return new Color(0.35f, 0.75f, 0.25f, 1f); // Green skin
                }
                // Claws
                if ((x < 8 || x > 40) && y < 20 && y > 10) return new Color(0.2f, 0.5f, 0.15f, 1f);
                return Color.clear;
            });

            // Red Ribbon Robot sprite
            CreateProceduralTextureIfMissing(dir + "RRRobot.png", 52, 52, (x, y) => {
                if (x >= 12 && x <= 40 && y >= 10 && y <= 42)
                {
                    // Visor
                    if (y >= 28 && y <= 33 && x >= 18 && x <= 34) return Color.cyan;
                    // Red Ribbon logo chest
                    if (y >= 18 && y <= 24 && x >= 20 && x <= 32) return Color.red;
                    return new Color(0.6f, 0.65f, 0.7f, 1f); // Metallic armor
                }
                return Color.clear;
            });

            // Red Ribbon Heavy Tank sprite
            CreateProceduralTextureIfMissing(dir + "RRTank.png", 64, 48, (x, y) => {
                // Treads
                if (y <= 14 && x >= 6 && x <= 58) return new Color(0.2f, 0.2f, 0.25f, 1f);
                // Body
                if (y > 14 && y <= 32 && x >= 12 && x <= 52) return new Color(0.8f, 0.15f, 0.15f, 1f); // Red army color
                // Turret Cannon
                if (y > 22 && y <= 28 && x < 14) return Color.gray;
                return Color.clear;
            });

            // Boss Frieza sprite
            CreateProceduralTextureIfMissing(dir + "BossFrieza.png", 64, 64, (x, y) => {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));
                if (dist < 24)
                {
                    // Purple patches
                    if (y > 42 || (x >= 24 && x <= 40 && y >= 20 && y <= 28)) return new Color(0.6f, 0.1f, 0.75f, 1f);
                    return new Color(0.95f, 0.95f, 1f, 1f); // White sleek body
                }
                return Color.clear;
            });

            // Boss Cell sprite
            CreateProceduralTextureIfMissing(dir + "BossCell.png", 64, 64, (x, y) => {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));
                if (dist < 26)
                {
                    // Wings/Crown
                    if (y > 44 && (x < 18 || x > 46)) return new Color(0.1f, 0.1f, 0.15f, 1f);
                    // Spots
                    if ((x % 8 == 0) && (y % 8 == 0)) return new Color(0.05f, 0.25f, 0.05f, 1f);
                    return new Color(0.2f, 0.7f, 0.25f, 1f); // Green bio-android
                }
                return Color.clear;
            });

            // Boss Buu sprite
            CreateProceduralTextureIfMissing(dir + "BossBuu.png", 64, 64, (x, y) => {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));
                if (dist < 27)
                {
                    if (y < 16) return Color.white; // White pants
                    return new Color(1f, 0.55f, 0.7f, 1f); // Pink Majin skin
                }
                return Color.clear;
            });

            // Boss Vegeta sprite
            CreateProceduralTextureIfMissing(dir + "BossVegeta.png", 60, 60, (x, y) => {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(30, 30));
                if (dist < 24)
                {
                    if (y > 36) return Color.black; // Flame hair
                    if (y < 24) return new Color(0.1f, 0.2f, 0.6f, 1f); // Blue suit
                    return new Color(0.9f, 0.9f, 0.9f, 1f); // White Saiyan armor
                }
                return Color.clear;
            });

            // Bulma Capsule Turret sprite
            CreateProceduralTextureIfMissing(dir + "BulmaTurret.png", 48, 48, (x, y) => {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(24, 20));
                if (dist < 16) return new Color(0.2f, 0.7f, 0.9f, 1f); // Capsule Corp Cyan
                // Cannon barrel
                if (y >= 18 && y <= 24 && x > 24) return Color.yellow;
                return Color.clear;
            });

            // Vegeta Ally sprite
            CreateProceduralTextureIfMissing(dir + "VegetaAlly.png", 52, 52, (x, y) => {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(26, 26));
                if (dist < 22)
                {
                    if (y > 32) return Color.black; // Black Saiyan hair
                    if (y < 20) return new Color(0.15f, 0.25f, 0.7f, 1f); // Blue suit
                    return Color.white; // Armor
                }
                return Color.clear;
            });

            // Piccolo Ally sprite
            CreateProceduralTextureIfMissing(dir + "PiccoloAlly.png", 52, 52, (x, y) => {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(26, 26));
                if (dist < 22)
                {
                    if (y > 34) return Color.white; // Turban
                    if (y > 22) return new Color(0.3f, 0.8f, 0.3f, 1f); // Namekian green
                    return new Color(0.45f, 0.15f, 0.6f, 1f); // Purple gi
                }
                return Color.clear;
            });

            // Ki Blast projectile sprite
            CreateProceduralTextureIfMissing(dir + "KiBlast.png", 32, 32, (x, y) => {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(16, 16));
                if (dist < 8) return Color.white; // Core
                if (dist < 14) return new Color(1f, 0.9f, 0.2f, 0.9f); // Golden Ki glow
                if (dist < 16) return new Color(1f, 0.6f, 0.1f, 0.4f);
                return Color.clear;
            });

            // Kamehameha beam sprite
            CreateProceduralTextureIfMissing(dir + "KamehamehaBeam.png", 128, 32, (x, y) => {
                float dY = Mathf.Abs(y - 16);
                if (dY < 4) return Color.white; // Intense white center
                if (dY < 10) return new Color(0.2f, 0.85f, 1f, 0.95f); // Vibrant cyan
                if (dY < 15) return new Color(0.1f, 0.5f, 1f, 0.5f); // Blue aura edge
                return Color.clear;
            });

            // Spirit Bomb sprite
            CreateProceduralTextureIfMissing(dir + "SpiritBomb.png", 64, 64, (x, y) => {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(32, 32));
                if (dist < 16) return Color.white; // Brilliant white core
                if (dist < 26) return new Color(0.3f, 0.9f, 1f, 0.95f); // Radiant cyan
                if (dist < 31) return new Color(0.15f, 0.6f, 1f, 0.5f); // Energy ring
                return Color.clear;
            });

            // Zeni Gold Coin sprite
            CreateProceduralTextureIfMissing(dir + "ZeniCoin.png", 32, 32, (x, y) => {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(16, 16));
                if (dist < 14)
                {
                    if (dist > 11) return new Color(0.85f, 0.65f, 0.05f, 1f); // Rim
                    if (Mathf.Abs(x - 16) < 3 && Mathf.Abs(y - 16) < 8) return new Color(0.95f, 0.75f, 0.1f, 1f); // 'Z' mark
                    return new Color(1f, 0.88f, 0.2f, 1f); // Gold body
                }
                return Color.clear;
            });

            // Senzu Bean sprite
            CreateProceduralTextureIfMissing(dir + "SenzuBean.png", 32, 32, (x, y) => {
                float dx = (x - 16) * 1.3f;
                float dy = (y - 16);
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                if (dist < 12) return new Color(0.25f, 0.85f, 0.35f, 1f); // Emerald green bean
                return Color.clear;
            });

            // Ki Orb sprite
            CreateProceduralTextureIfMissing(dir + "KiOrb.png", 32, 32, (x, y) => {
                float dist = Vector2.Distance(new Vector2(x, y), new Vector2(16, 16));
                if (dist < 8) return Color.white;
                if (dist < 13) return new Color(0.2f, 0.7f, 1f, 0.9f);
                return Color.clear;
            });
        }

        private static void CreateProceduralTextureIfMissing(string path, int width, int height, System.Func<int, int, Color> pixelFunc)
        {
            if (File.Exists(path)) return;

            Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    tex.SetPixel(x, y, pixelFunc(x, y));
                }
            }
            tex.Apply();
            byte[] bytes = ImageConversion.EncodeToPNG(tex);
            File.WriteAllBytes(path, bytes);

            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spritePixelsPerUnit = 32;
                importer.filterMode = FilterMode.Point; // Crisp pixel art
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }
        }

        private static GameObject[] CreateAllPrefabs()
        {
            string pDir = "Assets/Prefabs/";
            if (!Directory.Exists(pDir)) Directory.CreateDirectory(pDir);

            // 1. KiBlast Prefab
            GameObject kiBlastGo = new GameObject("KiBlastPrefab");
            SpriteRenderer kiSr = kiBlastGo.AddComponent<SpriteRenderer>();
            kiSr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/KiBlast.png");
            kiSr.sortingOrder = 10;
            kiBlastGo.AddComponent<CircleCollider2D>().radius = 0.35f;
            kiBlastGo.AddComponent<KiBlast>();
            GameObject kiBlastPrefab = PrefabUtility.SaveAsPrefabAsset(kiBlastGo, pDir + "KiBlastPrefab.prefab");
            DestroyImmediate(kiBlastGo);

            // 2. Kamehameha Prefab
            GameObject kamGo = new GameObject("KamehamehaPrefab");
            SpriteRenderer kamSr = kamGo.AddComponent<SpriteRenderer>();
            kamSr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/KamehamehaBeam.png");
            kamSr.sortingOrder = 15;
            kamGo.transform.localScale = new Vector3(14f, 1.4f, 1f);
            kamGo.AddComponent<BoxCollider2D>().size = new Vector2(1f, 0.8f);
            kamGo.AddComponent<KamehamehaBeam>();
            GameObject kamPrefab = PrefabUtility.SaveAsPrefabAsset(kamGo, pDir + "KamehamehaPrefab.prefab");
            DestroyImmediate(kamGo);

            // 3. SpiritBomb Prefab
            GameObject sbGo = new GameObject("SpiritBombPrefab");
            SpriteRenderer sbSr = sbGo.AddComponent<SpriteRenderer>();
            sbSr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/SpiritBomb.png");
            sbSr.sortingOrder = 20;
            sbGo.transform.localScale = Vector3.one * 1.8f;
            sbGo.AddComponent<SpiritBomb>();
            GameObject sbPrefab = PrefabUtility.SaveAsPrefabAsset(sbGo, pDir + "SpiritBombPrefab.prefab");
            DestroyImmediate(sbGo);

            // 4. Drops Prefabs
            GameObject zeniDropPrefab = CreateDropPrefab(pDir, "ZeniDropPrefab", "Assets/Sprites/ZeniCoin.png", DropType.Zeni, 25);
            GameObject senzuDropPrefab = CreateDropPrefab(pDir, "SenzuDropPrefab", "Assets/Sprites/SenzuBean.png", DropType.SenzuBean, 150);
            GameObject kiOrbDropPrefab = CreateDropPrefab(pDir, "KiOrbDropPrefab", "Assets/Sprites/KiOrb.png", DropType.KiOrb, 25);

            // 5. Enemies Prefabs
            GameObject saibamenPrefab = CreateEnemyPrefab(pDir, "SaibamenPrefab", "Assets/Sprites/Saibamen.png", 65, 3.2f, 120, 20, 50, true, zeniDropPrefab, senzuDropPrefab, kiOrbDropPrefab);
            GameObject rrRobotPrefab = CreateEnemyPrefab(pDir, "RRRobotPrefab", "Assets/Sprites/RRRobot.png", 140, 1.9f, 60, 35, 100, false, zeniDropPrefab, senzuDropPrefab, kiOrbDropPrefab);
            GameObject rrTankPrefab = CreateEnemyPrefab(pDir, "RRTankPrefab", "Assets/Sprites/RRTank.png", 380, 1.1f, 100, 60, 200, false, zeniDropPrefab, senzuDropPrefab, kiOrbDropPrefab);

            // 6. Boss Prefabs
            GameObject bossVegetaPrefab = CreateBossPrefab(pDir, "BossVegetaPrefab", "Assets/Sprites/BossVegeta.png", "Prince Vegeta", 1400, 1.3f, 40, zeniDropPrefab, senzuDropPrefab, kiOrbDropPrefab);
            GameObject bossFriezaPrefab = CreateBossPrefab(pDir, "BossFriezaPrefab", "Assets/Sprites/BossFrieza.png", "Lord Frieza", 2800, 1.2f, 55, zeniDropPrefab, senzuDropPrefab, kiOrbDropPrefab);
            GameObject bossCellPrefab = CreateBossPrefab(pDir, "BossCellPrefab", "Assets/Sprites/BossCell.png", "Perfect Cell", 4500, 1.1f, 70, zeniDropPrefab, senzuDropPrefab, kiOrbDropPrefab);
            GameObject bossBuuPrefab = CreateBossPrefab(pDir, "BossBuuPrefab", "Assets/Sprites/BossBuu.png", "Majin Buu", 7500, 1.0f, 90, zeniDropPrefab, senzuDropPrefab, kiOrbDropPrefab);

            // 7. Ally Turrets Prefabs
            GameObject bulmaTurretPrefab = CreateAllyPrefab(pDir, "BulmaTurretPrefab", "Assets/Sprites/BulmaTurret.png", AllyType.BulmaTurret, "Bulma Laser Turret", 25, 2.2f, 7.5f, kiBlastPrefab);
            GameObject vegetaAllyPrefab = CreateAllyPrefab(pDir, "VegetaAllyPrefab", "Assets/Sprites/VegetaAlly.png", AllyType.Vegeta, "Vegeta (Big Bang)", 50, 1.5f, 8.5f, kiBlastPrefab);
            GameObject piccoloAllyPrefab = CreateAllyPrefab(pDir, "PiccoloAllyPrefab", "Assets/Sprites/PiccoloAlly.png", AllyType.Piccolo, "Piccolo (Makanko)", 80, 0.9f, 9.5f, kiBlastPrefab);

            return new GameObject[] {
                kiBlastPrefab, kamPrefab, sbPrefab,
                zeniDropPrefab, senzuDropPrefab, kiOrbDropPrefab,
                saibamenPrefab, rrRobotPrefab, rrTankPrefab,
                bossVegetaPrefab, bossFriezaPrefab, bossCellPrefab, bossBuuPrefab,
                bulmaTurretPrefab, vegetaAllyPrefab, piccoloAllyPrefab
            };
        }

        private static GameObject CreateDropPrefab(string pDir, string name, string spritePath, DropType type, int amount)
        {
            GameObject go = new GameObject(name);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            sr.sortingOrder = 8;
            go.transform.localScale = Vector3.one * 0.9f;

            CircleCollider2D col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.4f;

            DropItem drop = go.AddComponent<DropItem>();
            drop.dropType = type;
            drop.amount = amount;

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, pDir + name + ".prefab");
            DestroyImmediate(go);
            return prefab;
        }

        private static GameObject CreateEnemyPrefab(string pDir, string name, string spritePath, int hp, float speed, int baseDmg, int zeni, int score, bool isBomber, GameObject zeniPrefab, GameObject senzuPrefab, GameObject kiOrbPrefab)
        {
            GameObject go = new GameObject(name);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            sr.sortingOrder = 5;

            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            CircleCollider2D col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.45f;

            EnemyMovement mv = go.AddComponent<EnemyMovement>();
            mv.moveSpeed = speed;

            EnemyHealth eh = go.AddComponent<EnemyHealth>();
            eh.maxHealth = hp;
            eh.currentHealth = hp;
            eh.baseDamage = baseDmg;
            eh.zeniReward = zeni;
            eh.scoreReward = score;
            eh.zeniDropPrefab = zeniPrefab;
            eh.senzuDropPrefab = senzuPrefab;
            eh.kiOrbDropPrefab = kiOrbPrefab;

            // Health bar fill child
            GameObject hpBarObj = new GameObject("HealthBarFill");
            hpBarObj.transform.SetParent(go.transform);
            hpBarObj.transform.localPosition = new Vector3(0, 0.6f, 0);
            hpBarObj.transform.localScale = new Vector3(1f, 0.12f, 1f);
            SpriteRenderer barSr = hpBarObj.AddComponent<SpriteRenderer>();
            barSr.color = Color.red;
            barSr.sortingOrder = 6;

            if (isBomber)
            {
                BomberEnemy bomber = go.AddComponent<BomberEnemy>();
                bomber.explosionDamage = 180;
                bomber.blastRadius = 3.0f;
            }

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, pDir + name + ".prefab");
            DestroyImmediate(go);
            return prefab;
        }

        private static GameObject CreateBossPrefab(string pDir, string name, string spritePath, string bossName, int hp, float speed, int attackDmg, GameObject zeniPrefab, GameObject senzuPrefab, GameObject kiOrbPrefab)
        {
            GameObject go = new GameObject(name);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            sr.sortingOrder = 5;
            go.transform.localScale = Vector3.one * 1.5f;

            Rigidbody2D rb = go.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Kinematic;

            CircleCollider2D col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.6f;

            EnemyMovement mv = go.AddComponent<EnemyMovement>();
            mv.moveSpeed = speed;

            EnemyHealth eh = go.AddComponent<EnemyHealth>();
            eh.maxHealth = hp;
            eh.currentHealth = hp;
            eh.baseDamage = 250;
            eh.zeniReward = 300;
            eh.scoreReward = 2000;
            eh.zeniDropPrefab = zeniPrefab;
            eh.senzuDropPrefab = senzuPrefab;
            eh.kiOrbDropPrefab = kiOrbPrefab;

            BossEnemy boss = go.AddComponent<BossEnemy>();
            boss.bossName = bossName;
            boss.attackDamage = attackDmg;

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, pDir + name + ".prefab");
            DestroyImmediate(go);
            return prefab;
        }

        private static GameObject CreateAllyPrefab(string pDir, string name, string spritePath, AllyType type, string allyName, int damage, float rate, float range, GameObject projectilePrefab)
        {
            GameObject go = new GameObject(name);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            sr.sortingOrder = 5;

            BoxCollider2D col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.8f, 0.8f);

            TurretAlly ally = go.AddComponent<TurretAlly>();
            ally.allyType = type;
            ally.allyName = allyName;
            ally.attackDamage = damage;
            ally.attackRate = rate;
            ally.attackRange = range;
            ally.projectilePrefab = projectilePrefab;

            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, pDir + name + ".prefab");
            DestroyImmediate(go);
            return prefab;
        }

        private static void BuildGameScene(GameObject[] prefabs)
        {
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // 1. Camera & ScreenShake
            GameObject camGo = new GameObject("Main Camera");
            Camera cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5.5f;
            cam.backgroundColor = new Color(0.08f, 0.08f, 0.12f, 1f);
            camGo.transform.position = new Vector3(0, 0, -10f);
            camGo.AddComponent<ScreenShake>();
            camGo.AddComponent<AudioListener>();
            camGo.tag = "MainCamera";

            // 2. Background Scenery
            GameObject bgGo = new GameObject("Environment_Background");
            SpriteRenderer bgSr = bgGo.AddComponent<SpriteRenderer>();
            Sprite bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Background.jpg");
            if (bgSprite != null)
            {
                bgSr.sprite = bgSprite;
                bgSr.sortingOrder = -10;
                // Scale to cover 16:9 11-unit camera
                bgGo.transform.localScale = new Vector3(1.15f, 1.15f, 1f);
                bgGo.transform.position = new Vector3(0.5f, 0, 0);
            }

            // 3. Dragon Ball Altar (Left side base)
            GameObject altarGo = new GameObject("DragonBallAltar");
            altarGo.transform.position = new Vector3(-7.2f, 0, 0);
            SpriteRenderer altarSr = altarGo.AddComponent<SpriteRenderer>();
            Sprite altarSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/DragonBallAltar.png");
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

            // 4. Waypoints (3 Lanes: Top, Mid, Bottom)
            GameObject waypointsRoot = new GameObject("Waypoints_Lanes");
            WaypointPath[] paths = new WaypointPath[3];

            paths[0] = CreateWaypointPath(waypointsRoot.transform, "Lane_Top", Color.cyan,
                new Vector3(9.5f, 2.8f, 0), new Vector3(4f, 3.0f, 0), new Vector3(-1f, 2.4f, 0), new Vector3(-6.0f, 1.0f, 0), new Vector3(-7.2f, 0.3f, 0));

            paths[1] = CreateWaypointPath(waypointsRoot.transform, "Lane_Mid", Color.yellow,
                new Vector3(9.5f, 0.0f, 0), new Vector3(4f, -0.2f, 0), new Vector3(-1f, 0.2f, 0), new Vector3(-6.0f, 0.0f, 0), new Vector3(-7.2f, 0.0f, 0));

            paths[2] = CreateWaypointPath(waypointsRoot.transform, "Lane_Bottom", Color.magenta,
                new Vector3(9.5f, -2.8f, 0), new Vector3(4f, -3.0f, 0), new Vector3(-1f, -2.4f, 0), new Vector3(-6.0f, -1.0f, 0), new Vector3(-7.2f, -0.3f, 0));

            // 5. Defender Slots
            GameObject slotsRoot = new GameObject("DefenderSlots");
            CreateDefenderSlot(slotsRoot.transform, "Slot_Top", new Vector3(-5.8f, 2.2f, 0));
            CreateDefenderSlot(slotsRoot.transform, "Slot_MidTop", new Vector3(-4.5f, 1.1f, 0));
            CreateDefenderSlot(slotsRoot.transform, "Slot_MidBottom", new Vector3(-4.5f, -1.1f, 0));
            CreateDefenderSlot(slotsRoot.transform, "Slot_Bottom", new Vector3(-5.8f, -2.2f, 0));

            // 6. Goku Player
            GameObject gokuGo = new GameObject("SonGoku_Player");
            gokuGo.transform.position = new Vector3(-4.8f, 0, 0);
            gokuGo.tag = "Player";
            SpriteRenderer gokuSr = gokuGo.AddComponent<SpriteRenderer>();
            Sprite gokuNormalSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/Goku.png");
            Sprite gokuSSJSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/GokuSSJ.png");
            if (gokuNormalSprite != null)
            {
                gokuSr.sprite = gokuNormalSprite;
                gokuSr.sortingOrder = 10;
                gokuGo.transform.localScale = new Vector3(0.28f, 0.28f, 1f);
            }
            BoxCollider2D gokuCol = gokuGo.AddComponent<BoxCollider2D>();
            gokuCol.isTrigger = true;
            gokuCol.size = new Vector2(3f, 4.5f);

            // Aura child object
            GameObject auraObj = new GameObject("SSJAura");
            auraObj.transform.SetParent(gokuGo.transform);
            auraObj.transform.localPosition = Vector3.zero;
            auraObj.transform.localScale = Vector3.one * 1.3f;
            SpriteRenderer auraSr = auraObj.AddComponent<SpriteRenderer>();
            auraSr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/SpiritBomb.png");
            auraSr.color = new Color(1f, 0.95f, 0.2f, 0.4f);
            auraSr.sortingOrder = 9;
            auraObj.SetActive(false);

            PlayerController pc = gokuGo.AddComponent<PlayerController>();
            SerializedObject pcSo = new SerializedObject(pc);
            pcSo.FindProperty("spriteRenderer").objectReferenceValue = gokuSr;
            pcSo.FindProperty("ssjAuraObject").objectReferenceValue = auraObj;
            pcSo.FindProperty("normalSprite").objectReferenceValue = gokuNormalSprite;
            pcSo.FindProperty("ssjSprite").objectReferenceValue = gokuSSJSprite;
            pcSo.ApplyModifiedProperties();

            DefenderAttack defAttack = gokuGo.AddComponent<DefenderAttack>();
            defAttack.kiBlastPrefab = prefabs[0]; // KiBlast
            defAttack.kamehamehaPrefab = prefabs[1]; // Kamehameha
            defAttack.spiritBombPrefab = prefabs[2]; // SpiritBomb

            // 7. Managers & Spawners
            GameObject gmGo = new GameObject("GameManager");
            GameManager gm = gmGo.AddComponent<GameManager>();
            gm.dragonBallAltar = altar;

            GameObject soundGo = new GameObject("SoundManager");
            soundGo.AddComponent<SoundManager>();

            GameObject upgGo = new GameObject("UpgradeManager");
            UpgradeManager upg = upgGo.AddComponent<UpgradeManager>();
            upg.playerAttack = defAttack;
            upg.bulmaTurretPrefab = prefabs[13]; // BulmaTurret
            upg.vegetaPrefab = prefabs[14];      // VegetaAlly
            upg.piccoloPrefab = prefabs[15];     // PiccoloAlly

            GameObject spawnerGo = new GameObject("WaveSpawner");
            WaveSpawner spawner = spawnerGo.AddComponent<WaveSpawner>();
            spawner.paths = paths;
            spawner.saibamenPrefab = prefabs[6]; // Saibamen
            spawner.rrRobotPrefab = prefabs[7];  // RRRobot
            spawner.rrTankPrefab = prefabs[8];   // RRTank
            spawner.bossVegetaPrefab = prefabs[9]; // BossVegeta
            spawner.bossFriezaPrefab = prefabs[10]; // BossFrieza
            spawner.bossCellPrefab = prefabs[11];   // BossCell
            spawner.bossBuuPrefab = prefabs[12];    // BossBuu

            // 8. Canvas UI
            BuildCanvasUI(gm, altar, defAttack, spawner, upg);

            // 9. EventSystem
            GameObject esGo = new GameObject("EventSystem");
            esGo.AddComponent<UnityEngine.EventSystems.EventSystem>();
            esGo.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

            // Save Scene
            string scenePath = "Assets/Scenes/DragonBallDefenders.unity";
            EditorSceneManager.SaveScene(newScene, scenePath);
            EditorSceneManager.SaveScene(newScene, "Assets/Scenes/SampleScene.unity");

            // Register in Build Settings
            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[] {
                new EditorBuildSettingsScene(scenePath, true),
                new EditorBuildSettingsScene("Assets/Scenes/SampleScene.unity", true)
            };
            EditorBuildSettings.scenes = buildScenes;
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
            sr.sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/KiBlast.png");
            sr.color = new Color(0.2f, 1f, 0.4f, 0.6f);
            sr.sortingOrder = 1;
            slotGo.transform.localScale = Vector3.one * 0.9f;

            slotGo.AddComponent<DefenderSlot>();
        }

        private static void BuildCanvasUI(GameManager gm, DragonBallAltar altar, DefenderAttack playerAttack, WaveSpawner spawner, UpgradeManager upg)
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

            // 1. Top Bar Container
            GameObject topBar = CreateUIPanel(canvasGo.transform, "TopBar", new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -90), new Vector2(0, 90), new Color(0.05f, 0.05f, 0.1f, 0.85f));

            // Dragon Ball Altar HP Bar
            ui.baseHealthSlider = CreateProgressBar(topBar.transform, "AltarHP_Bar", new Vector2(20, -25), new Vector2(360, 28), new Color(1f, 0.45f, 0.05f, 1f));
            ui.baseHealthText = CreateUIText(topBar.transform, "AltarHP_Text", "Ngọc Rồng: 1000 / 1000", new Vector2(20, -25), new Vector2(360, 28), 18, Color.white);

            // Ki Bar
            ui.kiSlider = CreateProgressBar(topBar.transform, "Ki_Bar", new Vector2(20, -62), new Vector2(360, 20), new Color(0.15f, 0.75f, 1f, 1f));
            ui.kiText = CreateUIText(topBar.transform, "Ki_Text", "Ki: 100 / 100", new Vector2(20, -62), new Vector2(360, 20), 15, Color.white);

            // Wave & Countdown in center
            ui.waveText = CreateUIText(topBar.transform, "Wave_Text", "ĐỢT: 1 / 10", new Vector2(0, -25), new Vector2(400, 35), 26, Color.yellow, TextAlignmentOptions.Center);
            ui.countdownText = CreateUIText(topBar.transform, "Countdown_Text", "CHUẨN BỊ CHIẾN ĐẤU!", new Vector2(0, -62), new Vector2(450, 25), 18, Color.cyan, TextAlignmentOptions.Center);

            // Zeni & Score on right
            ui.zeniText = CreateUIText(topBar.transform, "Zeni_Text", "Zeni: 300 G", new Vector2(-220, -25), new Vector2(200, 30), 22, new Color(1f, 0.85f, 0.1f, 1f), TextAlignmentOptions.Right);
            ui.scoreText = CreateUIText(topBar.transform, "Score_Text", "Score: 0", new Vector2(-220, -60), new Vector2(200, 25), 18, Color.white, TextAlignmentOptions.Right);

            // 2. Skill Hotbar (Bottom Right)
            GameObject skillBar = CreateUIPanel(canvasGo.transform, "SkillHotbar", new Vector2(1, 0), new Vector2(1, 0), new Vector2(-260, 70), new Vector2(500, 110), Color.clear);
            ui.kamehamehaButton = CreateSkillButton(skillBar.transform, "Btn_Kamehameha", "KAMEHAMEHA [Q]\n40 Ki", new Vector2(-360, 0), new Color(0.1f, 0.5f, 0.9f, 0.9f), out ui.kamehamehaCooldownFill);
            ui.spiritBombButton = CreateSkillButton(skillBar.transform, "Btn_SpiritBomb", "QUẢ CẦU KINH KHÍ [E]\n60 Ki", new Vector2(-200, 0), new Color(0.2f, 0.8f, 1f, 0.9f), out ui.spiritBombCooldownFill);
            ui.superSaiyanButton = CreateSkillButton(skillBar.transform, "Btn_SuperSaiyan", "SUPER SAIYAN [R]\n50 Ki", new Vector2(-40, 0), new Color(1f, 0.8f, 0.1f, 0.9f), out _);

            // 3. Shop Toggle Button (Bottom Left)
            ui.shopToggleButton = CreateActionButton(canvasGo.transform, "Btn_ShopToggle", "CỬA HÀNG / NÂNG CẤP [B]", new Vector2(150, 60), new Vector2(260, 55), new Color(0.9f, 0.5f, 0.1f, 0.95f));

            // 4. Shop / Upgrade Modal Panel
            ui.shopPanel = CreateUIPanel(canvasGo.transform, "ShopPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(750, 520), new Color(0.08f, 0.1f, 0.15f, 0.96f));
            CreateUIText(ui.shopPanel.transform, "ShopHeader", "⚡ KHO TRANG BỊ & NÂNG CẤP CAPSULE CORP ⚡", new Vector2(0, 220), new Vector2(700, 45), 24, Color.yellow, TextAlignmentOptions.Center);

            // Shop buttons inside panel
            ui.upgradeDamageButton = CreateShopItemButton(ui.shopPanel.transform, "Btn_UpgDamage", "Tăng Sát Thương Chưởng (+25%)", new Vector2(-170, 140), out ui.damageUpgradeCostText);
            ui.upgradeFireRateButton = CreateShopItemButton(ui.shopPanel.transform, "Btn_UpgFireRate", "Tăng Tốc Độ Bắn Ki Blast (+20%)", new Vector2(-170, 60), out ui.fireRateUpgradeCostText);
            ui.upgradeSsjButton = CreateShopItemButton(ui.shopPanel.transform, "Btn_UpgSSJ", "Tăng Thời Lượng Super Saiyan", new Vector2(-170, -20), out ui.ssjUpgradeCostText);
            ui.buySenzuHealButton = CreateShopItemButton(ui.shopPanel.transform, "Btn_SenzuHeal", "Đậu Thần Senzu (Hồi +250 Máu Căn Cứ)", new Vector2(-170, -100), out _);

            ui.deployBulmaTurretButton = CreateActionButton(ui.shopPanel.transform, "Btn_DeployBulma", "Đặt Pháo Tự Động Bulma\n(150 Zeni)", new Vector2(200, 130), new Vector2(240, 65), new Color(0.1f, 0.65f, 0.85f, 1f));
            ui.deployVegetaButton = CreateActionButton(ui.shopPanel.transform, "Btn_DeployVegeta", "Triệu Hồi Đồng Đội Vegeta\n(250 Zeni)", new Vector2(200, 50), new Vector2(240, 65), new Color(0.2f, 0.35f, 0.85f, 1f));
            ui.deployPiccoloButton = CreateActionButton(ui.shopPanel.transform, "Btn_DeployPiccolo", "Triệu Hồi Đồng Đội Piccolo\n(300 Zeni)", new Vector2(200, -30), new Vector2(240, 65), new Color(0.35f, 0.7f, 0.25f, 1f));

            Button closeShopBtn = CreateActionButton(ui.shopPanel.transform, "Btn_CloseShop", "ĐÓNG [B]", new Vector2(0, -200), new Vector2(200, 45), Color.red);
            closeShopBtn.onClick.AddListener(() => ui.ToggleShopPanel());

            // 5. Boss Warning Banner
            ui.bossWarningBanner = CreateUIPanel(canvasGo.transform, "BossWarningBanner", new Vector2(0.5f, 0.8f), new Vector2(0.5f, 0.8f), Vector2.zero, new Vector2(900, 75), new Color(0.85f, 0.1f, 0.1f, 0.95f));
            ui.bossWarningText = CreateUIText(ui.bossWarningBanner.transform, "BossWarningText", "CẢNH BÁO: TRÙM CUỐI ĐANG TIẾN ĐẾN!", Vector2.zero, new Vector2(880, 60), 26, Color.yellow, TextAlignmentOptions.Center);

            // 6. Game Over Panel
            ui.gameOverPanel = CreateUIPanel(canvasGo.transform, "GameOverPanel", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600, 400), new Color(0.15f, 0.05f, 0.05f, 0.98f));
            CreateUIText(ui.gameOverPanel.transform, "Title", "GAME OVER\nNGỌC RỒNG ĐÃ BỊ HỦY DIỆT!", new Vector2(0, 100), new Vector2(550, 90), 30, Color.red, TextAlignmentOptions.Center);
            ui.gameOverScoreText = CreateUIText(ui.gameOverPanel.transform, "FinalScore", "Điểm: 0", new Vector2(0, 10), new Vector2(500, 40), 22, Color.white, TextAlignmentOptions.Center);
            ui.gameOverRestartButton = CreateActionButton(ui.gameOverPanel.transform, "Btn_Restart", "CHƠI LẠI NGAY", new Vector2(0, -90), new Vector2(240, 60), new Color(0.9f, 0.2f, 0.2f, 1f));

            // 7. Victory Panel
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

            // Background
            Image bg = sliderObj.AddComponent<Image>();
            bg.color = new Color(0.15f, 0.15f, 0.2f, 0.8f);

            Slider slider = sliderObj.AddComponent<Slider>();

            // Fill area
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

            // Cooldown overlay
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
