using UnityEngine;

namespace UrbanFox.MiniGame
{
    public class ApplicationBuildData : ScriptableObjectSingleton<ApplicationBuildData>
    {
        public string CustomBuildName;

        [Header("Current Build Time")]
        [NonEditable] public string CurrentBuildTime = "N/A";

        [Header("Build Iterations")]
        [NonEditable] public int WindowsBuildIteration = 0;
        [NonEditable] public int MacBuildIteration = 0;
        [NonEditable] public int LinuxBuildIteration = 0;

        [NonEditable] public int SwitchBuildIteration = 0;
        [NonEditable] public int XboxBuildIteration = 0;
        [NonEditable] public int PS4BuildIteration = 0;
        [NonEditable] public int PS5BuildIteration = 0;

#if !UNITY_2023_1_OR_NEWER
        [NonEditable] public int LuminBuildIteration = 0;
        [NonEditable] public int StadiaBuildIteration = 0;
#endif

        [NonEditable] public int WebGLBuildIteration = 0;
        [NonEditable] public int iOSBuildIteration = 0;
        [NonEditable] public int tvOSBuildIteration = 0;
        [NonEditable] public int AndroidBuildIteration = 0;

        [NonEditable] public int WindowsServerBuildIteration = 0;
        [NonEditable] public int OSXServerBuildIteration = 0;
        [NonEditable] public int LinuxServerBuildIteration = 0;
        [NonEditable] public int LinuxEmbededBuildIteration = 0;

        public string GetBuildInfoText()
        {
            return $"Build Iteration v{GetCurrentPlatformBuildIteration()}{(string.IsNullOrEmpty(CustomBuildName) ? "" : "(" + CustomBuildName + ")")} for {Application.platform} at {CurrentBuildTime}.";
        }

        public int GetCurrentPlatformBuildIteration()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.OSXEditor:
                    return MacBuildIteration;
                case RuntimePlatform.OSXPlayer:
                    return MacBuildIteration;
                case RuntimePlatform.WindowsPlayer:
                    return WindowsBuildIteration;
                case RuntimePlatform.WindowsEditor:
                    return WindowsBuildIteration;
                case RuntimePlatform.IPhonePlayer:
                    return iOSBuildIteration;
                case RuntimePlatform.Android:
                    return AndroidBuildIteration;
                case RuntimePlatform.LinuxPlayer:
                    return LinuxBuildIteration;
                case RuntimePlatform.LinuxEditor:
                    return LinuxBuildIteration;
                case RuntimePlatform.WebGLPlayer:
                    return WebGLBuildIteration;
                case RuntimePlatform.PS4:
                    return PS4BuildIteration;
                case RuntimePlatform.XboxOne:
                    return XboxBuildIteration;
                case RuntimePlatform.tvOS:
                    return tvOSBuildIteration;
                case RuntimePlatform.Switch:
                    return SwitchBuildIteration;
#if !UNITY_2023_1_OR_NEWER
                case RuntimePlatform.Lumin:
                    return LuminBuildIteration;
                case RuntimePlatform.Stadia:
                    return StadiaBuildIteration;
#endif
                case RuntimePlatform.GameCoreXboxSeries:
                    return XboxBuildIteration;
                case RuntimePlatform.GameCoreXboxOne:
                    return XboxBuildIteration;
                case RuntimePlatform.PS5:
                    return PS5BuildIteration;
                case RuntimePlatform.EmbeddedLinuxArm64:
                    return LinuxEmbededBuildIteration;
                case RuntimePlatform.EmbeddedLinuxArm32:
                    return LinuxEmbededBuildIteration;
                case RuntimePlatform.EmbeddedLinuxX64:
                    return LinuxEmbededBuildIteration;
                case RuntimePlatform.EmbeddedLinuxX86:
                    return LinuxEmbededBuildIteration;
                case RuntimePlatform.LinuxServer:
                    return LinuxServerBuildIteration;
                case RuntimePlatform.WindowsServer:
                    return WindowsServerBuildIteration;
                case RuntimePlatform.OSXServer:
                    return OSXServerBuildIteration;
                default:
                    return -1;
            }
        }
    }
}
