using System;
using Calci.Windows;
using UnityEngine;

namespace Game.Shared.Util
{
	using Calci.CommandLine;
	
	public class ResolutionHandler : MonoBehaviour
	{
		private void Awake()
		{
			Application.targetFrameRate = 60;
			
			int width = CommandLineParser.GetInt("width", 800);
			int height = CommandLineParser.GetInt("height", 450);
			bool isFullScreen = CommandLineParser.GetBool("fullscreen", false);

			int posX = CommandLineParser.GetInt("posX", 100);
			int posY = CommandLineParser.GetInt("posY", 100);
			
			Screen.SetResolution(width, height, isFullScreen);
			WindowControl.Init();
			
			StartCoroutine(WindowControl.SetWindowPosition(posX, posY));
		}
	}
}