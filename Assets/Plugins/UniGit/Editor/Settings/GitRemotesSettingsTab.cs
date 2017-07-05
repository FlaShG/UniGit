﻿using System.Linq;
using LibGit2Sharp;
using UniGit.Status;
using UniGit.Utils;
using UnityEditor;
using UnityEngine;

namespace UniGit.Settings
{
	public class GitRemotesSettingsTab : GitSettingsTab
	{
		private Rect addRepositoryButtonRect;
		private RemoteCollection remotes;
		private RemoteEntry[] remoteCacheList = new RemoteEntry[0];
		private Vector2 scroll;

		internal override void OnGUI(Rect rect, Event current)
		{
			if(remotes == null) return;
			int remoteCount = remotes.Count();
			if (remoteCount <= 0)
			{
				EditorGUILayout.HelpBox("No Remotes", MessageType.Info);
			}

			scroll = EditorGUILayout.BeginScrollView(scroll);
			foreach (var remote in remoteCacheList)
			{
				GUILayout.Label(GitGUI.GetTempContent(remote.Name), "ShurikenModuleTitle");
				EditorGUILayout.Space();
				EditorGUILayout.BeginVertical();
				EditorGUI.BeginChangeCheck();
				GUI.enabled = false;
				GUI.SetNextControlName(remote.GetHashCode() + " Remote Name");
				EditorGUILayout.TextField(GitGUI.GetTempContent("Name"), remote.Name);
				GUI.enabled = true;
				GUI.SetNextControlName(remote.GetHashCode() + " Remote URL");
				remote.Url = EditorGUILayout.DelayedTextField(GitGUI.GetTempContent("URL"), remote.Url);
				//remote.PushUrl = EditorGUILayout.DelayedTextField(new GUIContent("Push URL"), remote.PushUrl, "ShurikenValue");
				remote.TagFetchMode = (TagFetchMode)EditorGUILayout.EnumPopup(GitGUI.GetTempContent("Tag Fetch Mode"), remote.TagFetchMode);
				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(GitGUI.GetTempContent("Save"), "minibuttonleft"))
				{
					remote.Update(remotes);
					UpdateRemotes();
					GUI.FocusControl("");
				}
				if (GUILayout.Button(GitGUI.GetTempContent("Open", "Show the repository in browser."), "minibuttonmid"))
				{
					Application.OpenURL(remote.Url);
				}
				if (GUILayout.Button(GitGUI.GetTempContent("Remove"), "minibuttonright"))
				{
					remotes.Remove(remote.Name);
					UpdateRemotes();
					GUI.FocusControl("");
				}
				GUILayout.FlexibleSpace();
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndScrollView();

			GUILayout.FlexibleSpace();
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button(GitGUI.IconContent("ol plus","Add Remote"), "AC Button"))
			{
				PopupWindow.Show(addRepositoryButtonRect, new AddRepositoryPopup(remotes));
			}
			if (current.type == EventType.Repaint) addRepositoryButtonRect = GUILayoutUtility.GetLastRect();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

		public override void OnGitUpdate(GitRepoStatus status, string[] paths)
		{
			base.OnGitUpdate(status, paths);
			if(GitManager.Repository == null) return;
			UpdateRemotes();
		}

		private void UpdateRemotes()
		{
			remotes = GitManager.Repository.Network.Remotes;
			remoteCacheList = remotes.Select(r => new RemoteEntry(r)).ToArray();
		}

		internal class RemoteEntry
		{
			private readonly Remote remote;
			private string name;
			public string Url { get; set; }
			public string PushUrl { get; set; }
			public TagFetchMode TagFetchMode { get; set; }

			public RemoteEntry(Remote remote)
			{
				this.remote = remote;
				Update();
			}

			private void Update()
			{
				name = remote.Name;
				Url = remote.Url;
				PushUrl = remote.PushUrl;
				TagFetchMode = remote.TagFetchMode;
			}

			public void Update(RemoteCollection remotes)
			{
				remotes.Update(remote, UpdateAction);
				Update();
			}

			private void UpdateAction(RemoteUpdater updater)
			{
				updater.Url = Url;
				updater.PushUrl = Url;
				updater.TagFetchMode = TagFetchMode;
			}

			public override bool Equals(object obj)
			{
				if (obj is Remote)
				{
					return remote.Equals(obj);
				}
				return base.Equals(obj);
			}

			public override int GetHashCode()
			{
				return remote.GetHashCode();
			}

			public Remote Remote
			{
				get { return remote; }
			}

			public string Name
			{
				get { return name; }
			}
		}

		private class AddRepositoryPopup : PopupWindowContent
		{
			private RemoteCollection remoteCollection;
			private string name = "origin";
			private string url;

			public AddRepositoryPopup(RemoteCollection remoteCollection)
			{
				this.remoteCollection = remoteCollection;
			}

			public override Vector2 GetWindowSize()
			{
				return new Vector2(300, 80);
			}

			public override void OnGUI(Rect rect)
			{
				EditorGUILayout.Space();
				name = EditorGUILayout.TextField(GitGUI.GetTempContent("Name"), name);
				url = EditorGUILayout.TextField(GitGUI.GetTempContent("URL"), url);
				GUI.enabled = !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url);
				if (GUILayout.Button(GitGUI.GetTempContent("Add Remote")))
				{
					remoteCollection.Add(name, url);
					GitManager.MarkDirty();
					EditorWindow.GetWindow<GitSettingsWindow>().Focus();
				}
				GUI.enabled = true;
				EditorGUILayout.Space();
			}
		}
	}
}