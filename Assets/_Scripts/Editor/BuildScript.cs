using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildScript
{
    [MenuItem("Build/Build All")]
    public static void BuildAll()
    {
        BuildWindowsServer();
        BuildLinuxServer();
        BuildWindowsClient();
        BuildWebClient();
    }
    [MenuItem("Build/Build Linux and HTML5")]
    public static void BuildLinuxHtml5()
    {
        BuildLinuxServer();
        BuildWebClient();
    }

    [MenuItem("Build/Build Server (Windows)")]
    public static void BuildWindowsServer()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/_Scenes/Offline.unity" , "Assets/_Scenes/Lobby.unity"};
        buildPlayerOptions.locationPathName = "Builds/Windows/Server/HaphAngel-Server.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC | BuildOptions.EnableHeadlessMode;

        Console.WriteLine("Building Server (Windows)...");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Console.WriteLine("Built Server (Windows).");
    }

    [MenuItem("Build/Build Server (Linux)")]
    public static void BuildLinuxServer()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/_Scenes/Offline.unity" , "Assets/_Scenes/Lobby.unity"};
        buildPlayerOptions.locationPathName = "Builds/Linux/Server/HaphAngel-Server.x86_64";
        buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC | BuildOptions.EnableHeadlessMode;

        Console.WriteLine("Building Server (Linux)...");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Console.WriteLine("Built Server (Linux).");
    }

    [MenuItem("Build/Build Client (Windows)")]
    public static void BuildWindowsClient()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/_Scenes/Offline.unity", "Assets/_Scenes/Lobby.unity", "Assets/_Scenes/main.unity" };
        buildPlayerOptions.locationPathName = "Builds/Windows/Client/HaphAngel-Windows-Client.exe";
        buildPlayerOptions.target = BuildTarget.StandaloneWindows64;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;

        Console.WriteLine("Building Client (Windows)...");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Console.WriteLine("Built Client (Windows).");
    }

    [MenuItem("Build/Build Client (HTML5)")]
    public static void BuildWebClient()
    {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = new[] { "Assets/_Scenes/Offline.unity", "Assets/_Scenes/Lobby.unity", "Assets/_Scenes/main.unity" };
        buildPlayerOptions.locationPathName = "Builds/WebGL/Client/HaphAngel-WebGL-Client";
        buildPlayerOptions.target = BuildTarget.WebGL;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;

        Console.WriteLine("Building Client (Web)...");
        BuildPipeline.BuildPlayer(buildPlayerOptions);
        Console.WriteLine("Built Client (Web).");
    }
}