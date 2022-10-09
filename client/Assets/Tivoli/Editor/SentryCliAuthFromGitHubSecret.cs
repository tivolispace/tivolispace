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

            var args = new List<string>(Environment.GetCommandLineArgs());

            const string arg = "-sentryAuthToken";
            var foundIndex = args.FindIndex(s => s.StartsWith(arg));

            if (foundIndex == -1)
            {
                Debug.Log("Sentry auth token not found, disabling symbol uploading");
                cliOptions.UploadSymbols = false;
            }
            else
            {
                Debug.Log("Sentry auth token found, enabling symbols uploading");

                // arguments are either: ["-sentryAuthToken", "token"] or ["-sentryAuthToken token"]
                var sentryAuthToken = args[foundIndex].Length == arg.Length
                    ? args[foundIndex + 1]
                    : args[foundIndex].Split(' ')[1];

                cliOptions.UploadSymbols = true;
                cliOptions.Auth = sentryAuthToken;
            }

            AssetDatabase.SaveAssets();
        }
    }
}