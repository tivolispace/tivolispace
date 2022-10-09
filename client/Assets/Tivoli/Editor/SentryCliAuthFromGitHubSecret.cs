using System;
using System.Collections.Generic;
using Sentry.Unity.Editor;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Tivoli.Editor
{
    public class SentryCliAuthFromGitHubSecret : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            var cliOptions =
                AssetDatabase.LoadAssetAtPath<SentryCliOptions>("Assets/Plugins/Sentry/SentryCliOptions.asset");

            var commandLineArgs = new List<string>(Environment.GetCommandLineArgs());
            var foundSentryAuthToken = commandLineArgs.FindIndex(s => s == "-sentryAuthToken");

            if (foundSentryAuthToken == -1)
            {
                Debug.Log("Sentry auth token not found, disabling symbol uploading");
                cliOptions.UploadSymbols = false;
            }
            else
            {
                Debug.Log("Sentry auth token found, enabling symbols uploading");
                var sentryAuthToken = commandLineArgs[foundSentryAuthToken + 1];
                cliOptions.UploadSymbols = true;
                cliOptions.Auth = sentryAuthToken;
            }

            AssetDatabase.SaveAssets();
        }
    }
}