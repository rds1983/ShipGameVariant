using AssetManagementBase;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace ShipGame
{
	internal static class Utility
	{
		public static void UnloadAsset(this AssetManager content, string name)
		{
			object asset;
			if (!content.Cache.TryGetValue(name, out asset))
			{
				return;
			}

			var asdisp = asset as IDisposable;
			if (asdisp != null)
			{
				asdisp.Dispose();
			}

			content.Cache.Remove(name);
		}

		public static void Dispose(this AssetManager content)
		{
			foreach (var pair in content.Cache)
			{
				var asdisp = pair.Value as IDisposable;
				if (asdisp != null)
				{
					asdisp.Dispose();
				}
			}

			content.Cache.Clear();
		}

		private class FontSystemLoadingSettings : IAssetSettings
		{
			public Texture2D ExistingTexture { get; set; }
			public Rectangle ExistingTextureUsedSpace { get; set; }
			public string[] AdditionalFonts { get; set; }

			public string BuildKey() => string.Empty;
		}

		private static AssetLoader<FontSystem> _fontSystemLoader = (manager, assetName, settings, tag) =>
		{
			var fontSystemSettings = new FontSystemSettings();

			var fontSystemLoadingSettings = (FontSystemLoadingSettings)settings;
			if (fontSystemLoadingSettings != null)
			{
				fontSystemSettings.ExistingTexture = fontSystemLoadingSettings.ExistingTexture;
				fontSystemSettings.ExistingTextureUsedSpace = fontSystemLoadingSettings.ExistingTextureUsedSpace;
			}
			;

			var fontSystem = new FontSystem(fontSystemSettings);
			var data = manager.ReadAsByteArray(assetName);
			fontSystem.AddFont(data);
			if (fontSystemLoadingSettings != null && fontSystemLoadingSettings.AdditionalFonts != null)
			{
				foreach (var file in fontSystemLoadingSettings.AdditionalFonts)
				{
					data = manager.ReadAsByteArray(file);
					fontSystem.AddFont(data);
				}
			}

			return fontSystem;
		};

		private static AssetLoader<StaticSpriteFont> _staticFontLoader = (manager, assetName, settings, tag) =>
		{
			var fontData = manager.ReadAsString(assetName);
			var graphicsDevice = (GraphicsDevice)tag;

			return StaticSpriteFont.FromBMFont(fontData,
						name =>
						{
							var imageData = manager.ReadAsByteArray(name);
							return new MemoryStream(imageData);
						},
						graphicsDevice);
		};

		public static FontSystem LoadFontSystem(this AssetManager assetManager, string assetName, string[] additionalFonts = null, Texture2D existingTexture = null, Rectangle existingTextureUsedSpace = default(Rectangle))
		{
			FontSystemLoadingSettings settings = null;
			if (additionalFonts != null || existingTexture != null)
			{
				settings = new FontSystemLoadingSettings
				{
					AdditionalFonts = additionalFonts,
					ExistingTexture = existingTexture,
					ExistingTextureUsedSpace = existingTextureUsedSpace
				};
			}

			return assetManager.UseLoader(_fontSystemLoader, assetName, settings);
		}

		public static StaticSpriteFont LoadStaticSpriteFont(this AssetManager assetManager, GraphicsDevice graphicsDevice, string assetName)
		{
			return assetManager.UseLoader(_staticFontLoader, assetName, tag: graphicsDevice);
		}
	}
}
