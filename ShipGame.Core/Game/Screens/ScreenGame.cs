#region File Description
//-----------------------------------------------------------------------------
// ScreenGame.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using AssetManagementBase;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
#endregion

namespace ShipGame
{
	public class ScreenGame : Screen
	{
		// called before screen shows
		public override void SetFocus(bool focus)
		{
			// if getting focus
			var gameManager = SG.GameManager;
			if (focus == true)
			{
				// load all resources
				gameManager.LoadFiles();
			}
			else // loosing focus
			{
				// free all resources
				gameManager.UnloadFiles();
			}
		}

		// process input
		public override void ProcessInput(float elapsedTime)
		{
			var gameManager = SG.GameManager;
			gameManager.ProcessInput(elapsedTime);

			int i, j = (int)gameManager.GameMode;
			var input = SG.InputManager;
			for (i = 0; i < j; i++)
			{
				if (input.IsKeyPressed(i, Keys.Escape) || input.IsButtonPressedBack(i))
				{
					gameManager.GetPlayer(i).Score = -1;
					SG.ScreenManager.SetNextScreen(ScreenType.ScreenEnd);
					gameManager.PlaySound("menu_cancel");
				}
			}
		}

		// update screen
		public override void Update(float elapsedTime)
		{
			// update game
			var gameManager = SG.GameManager;
			gameManager.Update(elapsedTime);

			// check if any player have reached the score limit
			// if so, changes to the end screen
			int i, j = (int)gameManager.GameMode;
			for (i = 0; i < j; i++)
			{
				if (gameManager.GetPlayer(i).Score == GameOptions.MaxPoints)
				{
					SG.ScreenManager.SetNextScreen(ScreenType.ScreenEnd,
						GameOptions.FadeColor, GameOptions.FadeTime);
				}
			}
		}

		// draw 3D scene
		public override void Draw3D()
		{
			// draw the 3d game scene
			SG.GameManager.Draw3D();
		}

		// draw 2D gui
		public override void Draw2D()
		{
			// draw 2D game gui
			SG.GameManager.Draw2D();
		}
	}
}
