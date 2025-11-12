using AssetManagementBase;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace ShipGame
{
	public static partial class SG
	{
		public static GraphicsDevice GraphicsDevice { get; private set; }
		public static AssetManager Assets { get; private set; }


		public static FontManagerType FontManager { get; private set; }

		public static GameManagerType GameManager { get; private set; }

		public static ScreenManagerType ScreenManager { get; private set; }


		public static void Initialize(GraphicsDevice device)
		{
			GraphicsDevice = device ?? throw new ArgumentNullException(nameof(device));

			var path = Path.Combine(Utility.ExecutingAssemblyDirectory, "Assets");
			Assets = AssetManager.CreateFileAssetManager(path);

			FontManager = new FontManagerType();
			FontManager.LoadContent();

			GameManager = new GameManagerType();
			GameManager.LoadContent();

			ScreenManager = new ScreenManagerType();
			ScreenManager.LoadContent();
		}

		public static void Uninitialize()
		{
			FontManager.UnloadContent();
			FontManager.Dispose();
			FontManager = null;
			
			GameManager.UnloadContent();
			GameManager.Dispose();
			GameManager = null;

			ScreenManager.UnloadContent();
			ScreenManager.Dispose();
			ScreenManager = null;
		}

		public static void Update(GameTime gameTime)
		{
			float elapsedTimeFloat = (float)gameTime.ElapsedGameTime.TotalSeconds;

			ScreenManager.ProcessInput(elapsedTimeFloat);
			ScreenManager.Update(elapsedTimeFloat);
		}

		public static void Draw(GameTime gameTime)
		{
			ScreenManager.Draw();
		}
	}
}
