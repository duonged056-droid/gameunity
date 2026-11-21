#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using System.IO;

public class BuildScript
{
    [MenuItem("Build/Build Windows 64")]
    public static void PerformBuild()
    {
        string buildDir = Path.Combine(Directory.GetCurrentDirectory(), "Builds");
        if (!Directory.Exists(buildDir))
        {
            Directory.CreateDirectory(buildDir);
        }

        string exePath = Path.Combine(buildDir, "DragonBall.exe");

        string[] scenes = new string[]
        {
            "Assets/Scenes/Menu.unity",
            "Assets/Scenes/Map.unity",
            "Assets/Scenes/Level1.unity",
            "Assets/Scenes/Level2.unity",
            "Assets/Scenes/Level3.unity",
            "Assets/Scenes/Level4.unity"
        };

        PlayerSettings.productName = "Dragon Ball";
        PlayerSettings.companyName = "duongviet";
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.runInBackground = true;

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.locationPathName = exePath;
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.None;

        Debug.Log("Starting Dragon Ball Windows Build to: " + exePath);
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("DragonBall Build succeeded: " + summary.totalSize + " bytes");
            EditorApplication.Exit(0);
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.LogError("DragonBall Build failed!");
            EditorApplication.Exit(1);
        }
    }
}
#endif
