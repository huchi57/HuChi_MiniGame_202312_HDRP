using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace UrbanFox.MiniGame.Editor
{
    public class BuildIncrementor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            var buildData = ApplicationBuildData.Instance;
            buildData.CurrentBuildTime = DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss");
            switch (report.summary.platform)
            {
                case BuildTarget.StandaloneOSX:
                    buildData.MacBuildIteration++;
                    break;
                case BuildTarget.StandaloneWindows:
                    buildData.WindowsBuildIteration++;
                    break;
                case BuildTarget.iOS:
                    buildData.iOSBuildIteration++;
                    break;
                case BuildTarget.Android:
                    buildData.AndroidBuildIteration++;
                    break;
                case BuildTarget.StandaloneWindows64:
                    buildData.WindowsBuildIteration++;
                    break;
                case BuildTarget.WebGL:
                    buildData.WebGLBuildIteration++;
                    break;
                case BuildTarget.StandaloneLinux64:
                    buildData.LinuxBuildIteration++;
                    break;
                case BuildTarget.PS4:
                    buildData.PS4BuildIteration++;
                    break;
                case BuildTarget.XboxOne:
                    buildData.XboxBuildIteration++;
                    break;
                case BuildTarget.tvOS:
                    buildData.tvOSBuildIteration++;
                    break;
                case BuildTarget.Switch:
                    buildData.SwitchBuildIteration++;
                    break;
                case BuildTarget.Lumin:
                    buildData.LuminBuildIteration++;
                    break;
                case BuildTarget.Stadia:
                    buildData.StadiaBuildIteration++;
                    break;
                case BuildTarget.GameCoreXboxOne:
                    buildData.XboxBuildIteration++;
                    break;
                case BuildTarget.PS5:
                    buildData.PS5BuildIteration++;
                    break;
                case BuildTarget.EmbeddedLinux:
                    buildData.LinuxEmbededBuildIteration++;
                    break;
                case BuildTarget.NoTarget:
                default:
                    break;
            }
            EditorUtility.SetDirty(buildData);
            AssetDatabase.Refresh();
        }
    }
}
