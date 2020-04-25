// C# example.
using UnityEngine;
using UnityEditor;
using System.Diagnostics;

public class CustomBuild
{
    [MenuItem("BrainBlenderGames/Create build for Amazon [Paid, No Ads]")]
    public static void BuildAmazonGame()
    {
        UnityEditor.PlayerSettings.Android.keystorePass = "android";
        UnityEditor.PlayerSettings.Android.keyaliasPass = "android";
        UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "PUBLISHING_PLATFORM_AMAZON");

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new string[] { "Assets/Scenes/screens/StartGameScene.unity",
                                                     "Assets/Scenes/screens/LoadingScene.unity",
                                                     "Assets/Scenes/screens/SplashScene.unity",
                                                     "Assets/Scenes/screens/GameScene.unity" };
        buildPlayerOptions.locationPathName = "Build/Amazon/CoverTheDice.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }

    [MenuItem("BrainBlenderGames/Create build for Google Play [Free, With Ads]")]
    public static void BuildGooglePlayGame()
    {
        UnityEditor.PlayerSettings.Android.keystorePass = "android";
        UnityEditor.PlayerSettings.Android.keyaliasPass = "android";
        UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "PUBLISHING_PLATFORM_GOOGLE_PLAY");

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new string[] { "Assets/Scenes/screens/StartGameScene.unity",
                                                     "Assets/Scenes/screens/LoadingScene.unity",
                                                     "Assets/Scenes/screens/SplashScene.unity",
                                                     "Assets/Scenes/screens/GameScene.unity" };
        buildPlayerOptions.locationPathName = "Build/GooglePlay/CoverTheDice.apk";
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;
        BuildPipeline.BuildPlayer(buildPlayerOptions);
    }
}