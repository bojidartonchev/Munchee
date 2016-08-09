using UnityEngine;
using System.Collections;

/// <summary>
/// Script to avoid duplicate code.
/// </summary>
public class MonoBehaviourHelper : MonoBehaviour 
{
	private WheelLogic _wheelLogic;
	public WheelLogic wheelLogic
	{
		get
		{
			if (_wheelLogic == null)
				_wheelLogic = FindObjectOfType<WheelLogic> ();

			return _wheelLogic;
		}
	}

	private WheelRotator _wheelRotator;
	public WheelRotator wheelRotator
	{
		get
		{
			if (_wheelRotator == null)
				_wheelRotator = FindObjectOfType<WheelRotator> ();

			return _wheelRotator;
		}
	}

	private GameManager _gameManager;
	public GameManager gameManager
	{
		get
		{
			if (_gameManager == null)
				_gameManager = FindObjectOfType<GameManager> ();

			return _gameManager;
		}
	}

	private SoundManager _soundManager;
	public SoundManager soundManager
	{
		get
		{
			if (_soundManager == null)
				_soundManager = FindObjectOfType<SoundManager> ();

			return _soundManager;
		}
	}

	private ColorManager _colorManager;
	public ColorManager colorManager
	{
		get
		{
			if (_colorManager == null)
				_colorManager = FindObjectOfType<ColorManager> ();

			return _colorManager;
		}
	}
}
