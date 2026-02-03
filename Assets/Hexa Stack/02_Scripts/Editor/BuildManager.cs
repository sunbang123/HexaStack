#if UNITY_EDITOR
using PlasticPipe.PlasticProtocol.Lz4;
using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Logger = HexaStack.Core.Logger;
public enum BuildType
{
    DEV,
    TEST,
    REAL
}
public class BuildManager : Editor
{
    // DEV용
    public const string DEV_SCRIPTING_DEFINE_SYMBOL = "DOTWEEN;DEV_VER";
    // 출시용
    public const string REAL_SCRIPTING_DEFINE_SYMBOL = "DOTWEEN";

    private static BuildType m_BuildType = BuildType.DEV;

    [MenuItem("Build/Set AOS DEV Build Settings")]
    public static void SetAOSDEVBuildSettings()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        EditorUserBuildSettings.buildAppBundle = false;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, DEV_SCRIPTING_DEFINE_SYMBOL);

        m_BuildType = BuildType.DEV;
    }

    [MenuItem("Build/Set AOS Test Build Settings")]
    public static void SetAOSTESTBuildSettings()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        EditorUserBuildSettings.buildAppBundle = true;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, DEV_SCRIPTING_DEFINE_SYMBOL);

        m_BuildType = BuildType.TEST;
    }

    [MenuItem("Build/Set AOS REAL Build Settings")]
    public static void SetAOSREALBuildSettings()
    {
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        EditorUserBuildSettings.buildAppBundle = true;
        PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, REAL_SCRIPTING_DEFINE_SYMBOL);
    }

    [MenuItem("Build/Set AOS Build")]
    public static void StartAOSBuild()
    {
        PlayerSettings.Android.keystoreName = "Builds/AOS/sunzero.keystore";
        PlayerSettings.Android.keystorePass = "";
        PlayerSettings.Android.keyaliasName = "Cosmic Hexa Puzzle";
        PlayerSettings.Android.keyaliasPass = "";

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[]
        {
            "Assets/Hexa Stack/01_Scenes/Boot.unity",
            "Assets/Hexa Stack/01_Scenes/InGame.unity",
            "Assets/Hexa Stack/01_Scenes/Loading.unity",
            "Assets/Hexa Stack/01_Scenes/Lobby.unity",
        };
        buildPlayerOptions.target = BuildTarget.Android;
        string fileExtention = string.Empty;
        BuildOptions compressOption = BuildOptions.None;

        switch (m_BuildType)
        {
            case BuildType.DEV:
                fileExtention = "apk";
                compressOption = BuildOptions.CompressWithLz4;
                break;
            case BuildType.TEST:
            case BuildType.REAL:
                fileExtention = "aab";
                compressOption = BuildOptions.CompressWithLz4HC;
                break;
            default:
                break;
        }

        buildPlayerOptions.locationPathName = $"Builds/AOS/CosmicHexaPuzzle_{Application.version}_{DateTime.Now.ToString("yyMMdd_HHmmss")}.{fileExtention}";
        buildPlayerOptions.options = compressOption;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;
        if(summary.result == BuildResult.Succeeded)
        {
            Logger.Log($"Build succeeded. {summary.totalSize} bytes.");
        }
        else if(summary.result == BuildResult.Failed)
        {
            Logger.LogError($"Build failed");
        }
    }
}
#endif