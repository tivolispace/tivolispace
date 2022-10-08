using System;
using Sentry.Unity.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Tivoli.Editor
{
    public class SentryCliAuthFromGitHubSecret : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var sentryAuthToken = Environment.GetEnvironmentVariable("SENTRY_AUTH_TOKEN");

            var cliOptions =
                AssetDatabase.LoadAssetAtPath<SentryCliOptions>("Assets/Plugins/Sentry/SentryCliOptions.asset");

            if (string.IsNullOrEmpty(sentryAuthToken))
            {
                cliOptions.UploadSymbols = false;
            }
            else
            {
                cliOptions.UploadSymbols = true;
                cliOptions.Auth = sentryAuthToken;
            }

            AssetDatabase.SaveAssets();
        }
    }
}