using Game.Shared.CommandLine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Shared.Util
{
	public class Switcher : MonoBehaviour
	{
		private void Start()
		{
			int sceneIndex = CommandLineParser.GetInt("scene", 1);
			
			Debug.LogError(sceneIndex);
			SceneManager.LoadScene(sceneIndex);
		}
	}
}