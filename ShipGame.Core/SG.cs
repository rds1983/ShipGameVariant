using AssetManagementBase;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace ShipGame
{
	public static partial class SG
	{
		public static GraphicsDevice GraphicsDevice { get; private set; }
		public static AssetManager Assets { get; private set; }


		public static void Initialize(GraphicsDevice device)
		{
			GraphicsDevice = device ?? throw new ArgumentNullException(nameof(device));

			var path = Path.Combine(Utility.ExecutingAssemblyDirectory, "Assets");
			Assets = AssetManager.CreateFileAssetManager(path);

			FontManager = new FontManagerType();
			FontManager.LoadContent();

			GameManager = new GameManagerType();
			GameManager.LoadContent();
		}

		public static void Uninitialize()
		{
			FontManager.UnloadContent();
			GameManager.UnloadContent();
		}
	}
}
