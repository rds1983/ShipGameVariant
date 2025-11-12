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

			GameManager.LoadContent();
		}

		public static void Uninitialize()
		{
			GameManager.UnloadContent();
		}
	}
}
