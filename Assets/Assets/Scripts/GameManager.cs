using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using DG.Tweening;
#if UNITY_5_3
using UnityEngine.SceneManagement;
#endif

#if APPADVISORY_ADS
using AppAdvisory.Ads;
#endif

/// <summary>
/// In charge of the game logic: Game Start, Game Over, Score, Ads etc... Attached to the Canvas game object
/// </summary>
public class GameManager : MonoBehaviourHelper
{
	/// <summary>
	/// If you want to monetize this game, get VERY SIMPLE ADS at this URL: http://u3d.as/oWD
	/// </summary>
	public string VerySimpleAdsURL = "http://u3d.as/oWD";
	/// <summary>
	/// Number of "play" to show an interstitial. If you want to monetize this game, get VERY SIMPLE ADS at this URL: http://u3d.as/oWD
	/// </summary>
	public int numberOfPlayToShowInterstitial = 5;
	/// <summary>
	/// to reset the player pref. Use if for debug only!!
	/// </summary>
	public bool RESET_PLAYER_PREF = false;
	/// <summary>
	/// True if game over
	/// </summary>
	public bool isGameOver = false;
	/// <summary>
	/// Text in the center of the screen = number of colors to find to clear the level
	/// </summary>
	public Text levelCenterScreen;
	/// <summary>
	/// Text in the center of the screen = number of colors to find to clear the level
	/// </summary>
	public Text levelTopScreen;
	/// <summary>
	/// Reference to wheel parent, to do the animation in and out for transition between level 
	/// </summary>
	public RectTransform wheelParent;
	/// <summary>
	/// Reference to the button to go to the previous level
	/// </summary>
	[SerializeField] private Button buttonPreviousLevel;
	/// <summary>
	/// Reference to the button to go to the next level, if the next level is already unlocked
	/// </summary>
	[SerializeField] private Button buttonNextLevel;
	/// <summary>
	/// The number of move we have to do to clear this level = the level number
	/// </summary>
	public int numTotalOfMove = 0;
	/// <summary>
	/// Clean the memory and place the wheelparent at the good place
	/// </summary>
	void Awake()
	{
		Util.CleanMemory();


		if(!Util.RestartFromGameOver())
		{
			float width = FindObjectOfType<Canvas>().GetComponent<RectTransform>().sizeDelta.x;
			wheelParent.anchoredPosition = new Vector3(width,0,0);
		}
	}
	/// <summary>
	/// Clean the memory and place the wheelparent at the good place
	/// </summary>
	void Start()
	{
		if(Application.isEditor)
		{
			if (RESET_PLAYER_PREF)
				PlayerPrefs.DeleteAll ();
		}

		RESET_PLAYER_PREF = false;

		if(!Util.RestartFromGameOver())
		{
			float width = FindObjectOfType<Canvas>().GetComponent<RectTransform>().sizeDelta.x;
			wheelParent.anchoredPosition = new Vector3(width,0,0);
		}

		SetNewGame ();
	}
	/// <summary>
	/// Called when player tap the previous button
	/// </summary>
	void OnClickedPreviousLevel()
	{
		OnClick (false);
	}
	/// <summary>
	/// Called when player tap the next button
	/// </summary>
	void OnClickedNextLevel()
	{
		OnClick (true);
	}
	/// <summary>
	/// Called when player tap the next or previous button
	/// </summary>
	void OnClick(bool isNext)
	{

		int current = Util.GetCurrentLevel ();

		if(isNext)
			current++;
		else
			current--;

		Util.SetCurrentLevel (current);

		DOMoveLevelOut(() => {
			Util.ReloadLevel();
		});
	}
	/// <summary>
	/// Create a new game: Set the texts, the numTotalOfMove and if the last game was not a game over : do the animation in
	/// </summary>
	void SetNewGame()
	{
		UpdateButton();

		isGameOver = false;

		levelCenterScreen.text = Util.GetCurrentLevel ().ToString ();

		levelTopScreen.text = "LEVEL: " + Util.GetCurrentLevel ().ToString ();

		numTotalOfMove = Util.GetCurrentLevel ();
	
		if(!Util.RestartFromGameOver())
			DOMoveLevelIn(null);
	
		Util.SetNotRestartFromGameOver();
	}
	/// <summary>
	/// Update the button previous and next
	/// </summary>
	void UpdateButton()
	{
		buttonPreviousLevel.onClick.RemoveListener (OnClickedPreviousLevel);
		buttonNextLevel.onClick.RemoveListener (OnClickedNextLevel);
		buttonPreviousLevel.gameObject.SetActive (Util.HavePreviousLevel ());
		buttonNextLevel.gameObject.SetActive (Util.HaveNextLevel ());
		buttonPreviousLevel.onClick.AddListener (OnClickedPreviousLevel);
		buttonNextLevel.onClick.AddListener (OnClickedNextLevel);
	}
	/// <summary>
	/// When a move is done, ie. player tap at the good moment, we decrease the numTotalOfMove ( -1 ) and we check if success (numTotalOfMove = 0). If success, we call the function LevelClear. If not, play a sound
	/// </summary>
	public void MoveDone()
	{
		numTotalOfMove--;

		levelCenterScreen.text = numTotalOfMove.ToString ();

		bool success = numTotalOfMove <= 0;

		if (success)
			LevelCleared ();
		else
			soundManager.PlayTouch ();
	}
	/// <summary>
	/// When a move is done, ie. player tap on the screen and the color of the triangle is not equal of the color of the part of the wheel below => Game Over. We restart the game and show interstitial. If you want to monetize this game, get VERY SIMPLE ADS at this URL: http://u3d.as/oWD
	/// </summary>
	public void GameOver()
	{
		soundManager.PlayFail ();
		isGameOver = true;

		StopAllCoroutines ();

		ShowAds();

		wheelParent.DOShakePosition(0.30f,10,100,90).OnComplete(() => {
			Util.SetRestartFromGameOver();
			Util.ReloadLevel();
		});
	}
	/// <summary>
	/// If the level is cleared (numTotalOfMove = 0), this function is called. We will animate out the wheel, increase the current level ( +1 ) and go to the next level. We we call to an interstitial. If you want to monetize this game, get VERY SIMPLE ADS at this URL: http://u3d.as/oWD
	/// </summary>
	public void LevelCleared()
	{
		soundManager.PlaySuccess ();

		int current = Util.GetCurrentLevel ();

		current++;

		Util.SetCurrentLevel (current);

		Util.SetMaxLevel (current);

		DOVirtual.Float(0f, 50f, 0.3f, 
			(float f) => {
				wheelLogic.triangle.rectTransform.anchoredPosition = new Vector3(0,f,0);
			})
			.SetEase(Ease.InQuad)
			.OnComplete(()=>{
				DOMoveLevelOut( () => {
					DOVirtual.DelayedCall(0.1f, () => {
						Util.ReloadLevel();
					});
				});
			});
	
		ShowAds();
	}
	/// <summary>
	/// Animation out of the wheel (from center to left)
	/// </summary>
	void DOMoveLevelOut(Action callback)
	{
		float width = FindObjectOfType<Canvas>().GetComponent<RectTransform>().sizeDelta.x;

		DOVirtual.Float(0f, -width, 0.3f, 
			(float f) => {
				wheelParent.anchoredPosition = new Vector3(f,0,0);
			})
			.OnComplete(()=>{
				if(callback != null)
					callback();
			});
	}
	/// <summary>
	/// Animation in of the wheel (from right to center)
	/// </summary>
	void DOMoveLevelIn(Action callback)
	{
		float width = FindObjectOfType<Canvas>().GetComponent<RectTransform>().sizeDelta.x;

		DOVirtual.Float(+width, 0f, 0.3f, 
			(float f) => {
		
				wheelParent.anchoredPosition = new Vector3(f,0,0);
			})
			.SetDelay(0.1f)
			.OnComplete(()=>{
				if(callback != null)
					callback();
			});
	}
	/// <summary>
	/// Show Ads - Interstitial. If you want to monetize this game, get VERY SIMPLE ADS at this URL: http://u3d.as/oWD
	/// </summary>
	public void ShowAds()
	{
		int count = PlayerPrefs.GetInt("GAMEOVER_COUNT",0);
		count++;
		PlayerPrefs.SetInt("GAMEOVER_COUNT",count);
		PlayerPrefs.Save();

		#if APPADVISORY_ADS
		if(count > numberOfPlayToShowInterstitial)
		{
		#if UNITY_EDITOR
		print("count = " + count + " > numberOfPlayToShowINterstitial = " + numberOfPlayToShowInterstitial);
		#endif
		if(AdsManager.instance.IsReadyInterstitial())
		{
		#if UNITY_EDITOR
			print("AdsManager.instance.IsReadyInterstitial() == true ----> SO ====> set count = 0 AND show interstial");
		#endif
			PlayerPrefs.SetInt("GAMEOVER_COUNT",0);
			AdsManager.instance.ShowInterstitial();
		}
		else
		{
		#if UNITY_EDITOR
			print("AdsManager.instance.IsReadyInterstitial() == false");
		#endif
		}

	}
	else
	{
		PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
	}
	PlayerPrefs.Save();
		#else
	if(count >= numberOfPlayToShowInterstitial)
	{
		Debug.LogWarning("To show ads, please have a look to Very Simple Ad on the Asset Store, or go to this link: " + VerySimpleAdsURL);
		Debug.LogWarning("Very Simple Ad is already implemented in this asset");
		Debug.LogWarning("Just import the package and you are ready to use it and monetize your game!");
		Debug.LogWarning("Very Simple Ad : " + VerySimpleAdsURL);
		PlayerPrefs.SetInt("GAMEOVER_COUNT",0);
	}
	else
	{
		PlayerPrefs.SetInt("GAMEOVER_COUNT", count);
	}
	PlayerPrefs.Save();
		#endif

	}
}
