﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using UniGit.Utils;
using UnityEngine;

namespace UniGit
{
	public class GitResourceManager : IGitResourceManager, IDisposable
	{
		private readonly Dictionary<string,Texture2D> textures;
		private readonly ILogger logger;

		[UniGitInject]
		public GitResourceManager(ILogger logger)
		{
			this.logger = logger;
			textures = new Dictionary<string, Texture2D>();
			GitProfilerProxy.BeginSample("UniGit Resource Loading");
			try
			{
				LoadDLLResources();
				//CheckForLeaks();
			}
			finally
			{
				GitProfilerProxy.EndSample();
			}
		}

		public Texture2D GetTexture(string name, bool throwError = true)
		{
			Texture2D tex;
			if (textures.TryGetValue(name, out tex))
			{
				return tex;
			}

			if (throwError)
			{
				logger.LogFormat(LogType.Error,"Could not find texture with key: {0}",name);
			}
			return null;
		}

		private void CheckForLeaks()
		{
			var texturesCount = Resources.FindObjectsOfTypeAll<Texture2D>().Count(t => t.name.StartsWith("UniGitEditorResource"));
			if (texturesCount > 0)
			{
				Debug.LogWarningFormat("Found {0} leaked UniGit textures.",texturesCount);
			}
		}

		private void LoadDLLResources()
		{
			try
			{
				Assembly myAssembly = Assembly.Load("UniGitResources");
				var rc = new System.Resources.ResourceManager("UniGitResources.Properties.Resources", myAssembly);
				foreach (DictionaryEntry e in rc.GetResourceSet(CultureInfo.InvariantCulture, true, true))
				{
					if (e.Value.GetType().Name == "Bitmap")
					{
						textures.Add((string)e.Key, LoadTextureFromBitmap((string)e.Key, e.Value));
					}

				}
				rc.ReleaseAllResources();
			}
			catch (Exception e)
			{
				logger.LogException(e);
			}
		}

		private Texture2D LoadTextureFromBitmap(string name, object resource)
		{
			var bitmapType = resource.GetType();

			var widthProperty = bitmapType.GetProperty("Width", BindingFlags.Public | BindingFlags.Instance);
			int width = (int)widthProperty.GetValue(resource, null);
			var heightProperty = bitmapType.GetProperty("Height", BindingFlags.Public | BindingFlags.Instance);
			int height = (int)heightProperty.GetValue(resource, null);

			var imageFormatProperty = bitmapType.GetProperty("RawFormat", BindingFlags.Public | BindingFlags.Instance);
			var imageFromat = imageFormatProperty.GetValue(resource, null);

			var saveToStreamMethod = bitmapType.GetMethod("Save", BindingFlags.Instance | BindingFlags.Public, null, new Type[] { typeof(Stream), imageFromat.GetType() }, null);
			byte[] imageBytes;
			using (MemoryStream ms = new MemoryStream())
			{
				saveToStreamMethod.Invoke(resource, new object[] { ms, imageFromat });
				imageBytes = ms.ToArray();
			}

			var img = new Texture2D(width, height, TextureFormat.RGBA32, false, true)
			{
				hideFlags = HideFlags.HideAndDontSave,
				name = "UniGitEditorResource.Image." + name,
				wrapMode = TextureWrapMode.Clamp
			};
			if (!img.LoadImage(imageBytes))
			{
				logger.LogFormat(LogType.Warning,"There was a problem while loading a texture: {0}",name);
			}
			img.Apply();

			return img;
		}

		public void Dispose()
		{
			foreach (var texture in textures)
			{
				UnityEngine.Object.DestroyImmediate(texture.Value,false);
			}
			textures.Clear();
		}
	}
}