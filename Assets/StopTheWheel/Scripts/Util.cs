using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
#if UNITY_5_3_OR_NEWER
using UnityEngine.SceneManagement;
#endif

/// <summary>
/// Utility class. This class is static, so you can use it in all your projects!
/// </summary>
public static class Util
{
	/// <summary>
	/// Compare two colors
	/// </summary>
	public static bool IsEqual(this Color c, Color o)
	{
		if(c.r != o.r)
			return false;

		if(c.g != o.g)
			return false;

		if(c.b != o.b)
			return false;
		
		return true;
	}

	private static System.Random rng = new System.Random();  
	/// <summary>
	/// Real shuffle of List
	/// </summary>
	public static void Shuffle<T>(this IList<T> list)  
	{  
		int n = list.Count;  
		while (n > 1) {  
			n--;  
			int k = rng.Next(n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}
	/// <summary>
	/// Get the current level
	/// </summary>
	public static int GetCurrentLevel()
	{
		int current = PlayerPrefs.GetInt ("CURRENT_LEVEL", 1);

		if (current <= 0) 
		{
			current = 1;
			PlayerPrefs.SetInt ("CURRENT_LEVEL", 1);
			PlayerPrefs.Save ();
		}

		return current;
	}
	/// <summary>
	/// Check if there is a previous level
	/// </summary>
	public static bool HavePreviousLevel()
	{
		int currentLevel = GetCurrentLevel ();
		if (currentLevel > 1)
			return true;

		return false;
	}
	/// <summary>
	/// Check if there is a next level, ie. if the next level is unlocked by the player
	/// </summary>
	public static bool HaveNextLevel()
	{
		int currentLevel = GetCurrentLevel ();

		int maxLevel = GetMaxLevel ();

		if (currentLevel < maxLevel)
			return true;

		return false;
	}
	/// <summary>
	/// Get the max level unlocked by the player
	/// </summary>
	public static int GetMaxLevel()
	{
		int max = PlayerPrefs.GetInt ("MAX_LEVEL", 1);

		return max;
	}
	/// <summary>
	/// Set the max level unlocked by the player
	/// </summary>
	public static void SetMaxLevel(int level)
	{
		int max = GetMaxLevel ();

		if (max < level) 
		{
			PlayerPrefs.SetInt ("MAX_LEVEL", level);
			PlayerPrefs.Save ();
		}
	}
	/// <summary>
	/// Set the current played level
	/// </summary>
	public static void SetCurrentLevel(int level)
	{
		PlayerPrefs.SetInt ("CURRENT_LEVEL", level);
		PlayerPrefs.Save ();
	}
	/// <summary>
	/// Clean the memory and reload the scene
	/// </summary>
	public static void ReloadLevel()
	{
		CleanMemory();

		#if UNITY_5_3_OR_NEWER
		SceneManager.LoadSceneAsync(0,LoadSceneMode.Single);
		#else
		Application.LoadLevel(Application.loadedLevel);
		#endif

		CleanMemory();
	}
	/// <summary>
	/// Clean the memory
	/// </summary>
	public static void CleanMemory()
	{
		DOTween.KillAll();
		GC.Collect();
		Application.targetFrameRate = 60;
	}
	/// <summary>
	/// Resturn true if last time we play we lose (= Game Over)
	/// </summary>
	public static bool RestartFromGameOver()
	{
		return PlayerPrefs.GetInt("_RestartFromGameOver",0) == 1;
	}
	/// <summary>
	/// Set restart from game over = true
	/// </summary>
	public static void SetRestartFromGameOver()
	{
		PlayerPrefs.SetInt("_RestartFromGameOver",1);
		PlayerPrefs.Save();
	}
	/// <summary>
	/// Set restart from game over = false
	/// </summary>
	public static void SetNotRestartFromGameOver()
	{
		PlayerPrefs.SetInt("_RestartFromGameOver",0);
		PlayerPrefs.Save();
	}
}
