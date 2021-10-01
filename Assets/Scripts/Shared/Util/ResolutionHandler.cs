using System;
using UnityEngine;

namespace Game.Shared.Util
{
	using Calci.CommandLine;
	
	public class ResolutionHandler : MonoBehaviour
	{
		private void Awake()
		{
			int width = CommandLineParser.GetInt("width", 800);
			int height = CommandLineParser.GetInt("height", 450);
			bool isFullScreen = CommandLineParser.GetBool("fullscreen", false);
			
			Screen.SetResolution(width, height, isFullScreen);
		}
	}
}