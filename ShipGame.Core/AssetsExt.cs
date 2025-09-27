using AssetManagementBase;
using DigitalRiseModel;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace ShipGame
{
	public enum ShipGameEffectType
	{
		Basic,
		NormalMapping
	}

	internal static class AssetsExt
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

		private class ModelSettings
		{
			public GraphicsDevice GraphicsDevice;
			public ShipGameEffectType Effect;

			public ModelSettings(GraphicsDevice device, ShipGameEffectType effect)
			{
				GraphicsDevice = device;
				Effect = effect;
			}
		}

		public static Texture2D LoadTexture2DDefault(this AssetManager manager, GraphicsDevice gd, string assetName)
		{
			return manager.LoadTexture2D(gd, assetName, premultiplyAlpha: true, colorKey: new Color(255, 0, 255, 255));
		}

		private static Texture2D LoadModelTexture(GraphicsDevice gd, AssetManager manager, string assetName, string postfix, string nullPath)
		{
			var path = $"{assetName}_{postfix}.tga";
			if (!manager.Exists(path))
			{
				path = nullPath;
			}

			return manager.LoadTexture2DDefault(gd, path);
		}

		private static AssetLoader<DrModel> _modelLoader = (manager, assetName, settings, tag) =>
		{
			// Load gltf
			var modelSettings = (ModelSettings)tag;
			var device = modelSettings.GraphicsDevice;

			var model = manager.LoadGltf(device, Path.ChangeExtension(assetName, "glb"));

			Effect effect;
			if (modelSettings.Effect == ShipGameEffectType.NormalMapping)
			{
				effect = manager.LoadEffect2(device, "/shaders/NormalMapping.efb").Clone();

				effect.Parameters["Texture"].SetValue(LoadModelTexture(device, manager, assetName, "c", "/null_color.tga"));
				effect.Parameters["Bump0"].SetValue(LoadModelTexture(device, manager, assetName, "n", "/null_normal.tga"));
				effect.Parameters["Specular0"].SetValue(LoadModelTexture(device, manager, assetName, "s", "/null_specular.tga"));
				effect.Parameters["Emissive0"].SetValue(LoadModelTexture(device, manager, assetName, "g", "/null_glow.tga"));
			}
			else
			{
				effect = new BasicEffect(device);
			}

			foreach (var mesh in model.Meshes)
			{
				foreach (var part in mesh.MeshParts)
				{
					part.Tag = effect;
				}
			}

			return model;
		};

		public static DrModel LoadModel(this AssetManager assetManager, GraphicsDevice graphicsDevice, string assetName, ShipGameEffectType effect)
		{
			return assetManager.UseLoader(_modelLoader, assetName, tag: new ModelSettings(graphicsDevice, effect));
		}

		public static Effect LoadEffect2(this AssetManager manager, GraphicsDevice graphicsDevice, string assetName)
		{
			var folder = Path.GetDirectoryName(assetName);
			var file = Path.GetFileName(assetName);

#if FNA
			var path = folder + "/FNA/" + file;
#else
			var path = folder + "/MonoGameOGL/" + file;
#endif

			return manager.LoadEffect(graphicsDevice, path);
		}

	}
}
