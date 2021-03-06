﻿/****************************************************************************
 * 2017 ~ 2018.7 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/

namespace QFramework
{
	using System;
	using System.Net.Security;
	using System.Security.Cryptography.X509Certificates;
	using UnityEngine;
	using UnityEditor;

	public class PreferencesWindow : EditorWindow
	{
		[MenuItem(FrameworkMenuItems.Preferences, false, FrameworkMenuItemsPriorities.Preferences)]
		private static void Open()
		{
			var frameworkConfigEditorWindow = (PreferencesWindow) GetWindow(typeof(PreferencesWindow), true);
			frameworkConfigEditorWindow.titleContent = new GUIContent("QFramework Settings");
			frameworkConfigEditorWindow.CurSettingData = FrameworkSettingData.Load();
			frameworkConfigEditorWindow.position = new Rect(100, 100, 500, 400);
			frameworkConfigEditorWindow.Init();
			frameworkConfigEditorWindow.Show();
		}



		private const string URL_GITHUB_ISSUE = "https://github.com/liangxiegame/QFramework/issues/new";

		[MenuItem(FrameworkMenuItems.CheckForUpdates, false, FrameworkMenuItemsPriorities.CheckForUpdates)]
		static void requestLatestRelease()
		{
		}

		[MenuItem(FrameworkMenuItems.Feedback, false, FrameworkMenuItemsPriorities.Feedback)]
		private static void Feedback()
		{
			Application.OpenURL(URL_GITHUB_ISSUE);
		}


		public static bool MyRemoteCertificateValidationCallback(System.Object sender,
			X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool isOk = true;
			// If there are errors in the certificate chain,
			// look at each error to determine the cause.
			if (sslPolicyErrors != SslPolicyErrors.None)
			{
				foreach (var chainStatus in chain.ChainStatus)
				{
					if (chainStatus.Status == X509ChainStatusFlags.RevocationStatusUnknown)
					{
						continue;
					}

					chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
					chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
					chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
					chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
					bool chainIsValid = chain.Build((X509Certificate2) certificate);
					if (!chainIsValid)
					{
						isOk = false;
						break;
					}
				}
			}

			return isOk;
		}

		static bool trustSource(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		public FrameworkSettingData CurSettingData;
		public FrameworkLocalVersion FrameworkLocalVersion;

		private void Init()
		{
			FrameworkLocalVersion = FrameworkLocalVersion.Get();
		}
		
		private void OnGUI()
		{
			GUILayout.Label("UI Kit Settings:");
			GUILayout.BeginVertical("box");
			CurSettingData.Namespace = EditorGUIUtils.GUILabelAndTextField("Namespace", CurSettingData.Namespace);
			CurSettingData.UIScriptDir =
				EditorGUIUtils.GUILabelAndTextField("UI Script Generate Dir", CurSettingData.UIScriptDir);
			CurSettingData.UIPrefabDir = EditorGUIUtils.GUILabelAndTextField("UI Prefab Dir", CurSettingData.UIPrefabDir);

			if (GUILayout.Button("Apply"))
			{
				CurSettingData.Save();
			}
			GUILayout.EndVertical();

			GUILayout.Label("Framework:");
			GUILayout.BeginVertical("box");
			GUILayout.Label(string.Format("Current Framework Version:{0}", FrameworkLocalVersion.Version));
			
			GUILayout.BeginHorizontal();
			
			if (GUILayout.Button("Download Latest Version"))
			{
				this.ExecuteNode(new DownloadLatestFramework());
			}

			if (GUILayout.Button("Download Demo"))
			{
				this.ExecuteNode(new DownloadLatestDemo());
			}
			
			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
		}
	}
}

//public enum UpdateState {
//        UpToDate,
//        UpdateAvailable,
//        AheadOfLatestRelease,
//        NoConnection
//    }
//
//    public class UpdateInfo
//    {
//
//        public UpdateState updateState
//        {
//            get { return _updateState; }
//        }
//
//        public readonly string localVersionString;
//        public readonly string remoteVersionString;
//
//        readonly UpdateState _updateState;
//
//        public UpdateInfo(string localVersionString, string remoteVersionString)
//        {
//            this.localVersionString = localVersionString.Trim();
//            this.remoteVersionString = remoteVersionString.Trim();
//
//            if (remoteVersionString != string.Empty)
//            {
//                var localVersion = new Version(localVersionString);
//                var remoteVersion = new Version(remoteVersionString);
//
//                switch (remoteVersion.CompareTo(localVersion))
//                {
//                    case 1:
//                        _updateState = UpdateState.UpdateAvailable;
//                        break;
//                    case 0:
//                        _updateState = UpdateState.UpToDate;
//                        break;
//                    case -1:
//                        _updateState = UpdateState.AheadOfLatestRelease;
//                        break;
//                }
//            }
//            else
//            {
//                _updateState = UpdateState.NoConnection;
//            }
//        }
//    }
//
//    public static class CheckForUpdates 
//    {
//        const string URL_GITHUB_API_LATEST_RELEASE = "https://api.github.com/repos/sschmid/Entitas-CSharp/releases/latest";
//        const string URL_GITHUB_RELEASES = "https://github.com/sschmid/Entitas-CSharp/releases";
//        const string URL_ASSET_STORE = "https://www.assetstore.unity3d.com/#!/content/87638";
//
//        [MenuItem(EntitasMenuItems.check_for_updates, false, EntitasMenuItemPriorities.check_for_updates)]
//        public static void DisplayUpdates() {
//            var info = GetUpdateInfo();
//            displayUpdateInfo(info);
//        }
//
//        public static UpdateInfo GetUpdateInfo()
//        {
////            var localVersion = GetLocalVersion();
//            var remoteVersion = GetRemoteVersion();
////            return new UpdateInfo(localVersion, remoteVersion);
//            return null;
//        }
//
////        public static string GetLocalVersion()
////        {
////            return EntitasResources.GetVersion();
////        }
//
//        public static string GetRemoteVersion() {
//            string latestRelease = null;
//            try {
//                latestRelease = requestLatestRelease();
//            } catch(Exception) {
//                latestRelease = string.Empty;
//            }
//
//            return parseVersion(latestRelease);
//        }
//

//
//        static string parseVersion(string response) {
//            const string versionPattern = @"(?<=""tag_name"":"").*?(?="")";
//            return Regex.Match(response, versionPattern).Value;
//        }
//
//        static void displayUpdateInfo(UpdateInfo info) {
//            switch (info.updateState) {
//                case UpdateState.UpdateAvailable:
//                    if (EditorUtility.DisplayDialog("Entitas Update",
//                            string.Format("A newer version of Entitas is available!\n\n" +
//                            "Currently installed version: {0}\n" +
//                            "New version: {1}", info.localVersionString, info.remoteVersionString),
//                            "Show in Unity Asset Store",
//                            "Cancel"
//                        )) {
//                        Application.OpenURL(URL_ASSET_STORE);
//                    }
//                    break;
//                case UpdateState.UpToDate:
//                    EditorUtility.DisplayDialog("Entitas Update",
//                        "Entitas is up to date (" + info.localVersionString + ")",
//                        "Ok"
//                    );
//                    break;
//                case UpdateState.AheadOfLatestRelease:
//                    if (EditorUtility.DisplayDialog("Entitas Update",
//                            string.Format("Your Entitas version seems to be newer than the latest release?!?\n\n" +
//                            "Currently installed version: {0}\n" +
//                            "Latest release: {1}", info.localVersionString, info.remoteVersionString),
//                            "Show in Unity Asset Store",
//                            "Cancel"
//                        )) {
//                        Application.OpenURL(URL_ASSET_STORE);
//                    }
//                    break;
//                case UpdateState.NoConnection:
//                    if (EditorUtility.DisplayDialog("Entitas Update",
//                            "Could not request latest Entitas version!\n\n" +
//                            "Make sure that you are connected to the internet.\n",
//                            "Try again",
//                            "Cancel"
//                        )) {
//                        DisplayUpdates();
//                    }
//                    break;
//            }
//        }
//

//    }