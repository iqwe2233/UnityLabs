using UnityEngine;
using UnityEngine.SceneManagement;

public class ServiceManager : MonoBehaviour
{
	#region  Singleton

	public static ServiceManager Instance;
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}
	#endregion

	void Start()
	{
		Time.timeScale = 1;

		if (SceneManager.GetActiveScene().buildIndex != 0)
		{
			PlayerPrefs.SetInt(GamePrefs.LastPlayedLvl.ToString(), SceneManager.GetActiveScene().buildIndex);
			PlayerPrefs.SetInt(GamePrefs.LvlPlayed.ToString() + SceneManager.GetActiveScene().buildIndex, 1);
		}
	}

	public void Restart()
	{
		ChangeLvl(SceneManager.GetActiveScene().buildIndex);
	}

	public void EndLevel()
	{
		ChangeLvl(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void ChangeLvl(int lvl)
	{
		SceneManager.LoadScene(lvl);

	}

	public void Quit()
	{
		Application.Quit();
		Debug.Log("Quit");
	}

	public void ResetProgress()
	{
		PlayerPrefs.DeleteAll();

	}
}