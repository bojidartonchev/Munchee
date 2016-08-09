using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// In charge of the rotation of the wheel and of the input in the game (who will stop the rotation, check the color, and start the rotation in the other direction). Attached to the game object: "PartParent".
/// </summary>
public class WheelRotator : MonoBehaviourHelper
{
	/// <summary>
	/// Two directions: left and right (1 and -1)
	/// </summary>
	[System.NonSerialized] public int direction = 1;
	/// <summary>
	/// Is it the first time we start the rotation for the level?
	/// </summary>
	bool firstStart = true;
	/// <summary>
	/// Check if success = number of moove to do = 0
	/// </summary>
	bool isSuccess
	{
		get 
		{
			bool success = gameManager.numTotalOfMove <= 0;
			return success;
		}
	}
	/// <summary>
	/// Choose the start direction randmly and set firstStart to true
	/// </summary>
	void Awake()
	{
		direction = 1;
		if(Random.Range(0,100) < 50)
			direction = -1;
		firstStart = true;
	}
	/// <summary>
	/// Listen if the player tap or click, and if the game is not game over after the click (so triangle color = part of the wheel color) launch again the rotation but in the oposite direction
	/// </summary>
	void Update()
	{
		if (
//			!gameManager.gameIsStarted || 
			gameManager.isGameOver || isSuccess) 
		{
			if(rotateTweener != null && rotateTweener.IsPlaying())
				rotateTweener.Kill();
			return;
		}
		
		if (Input.GetMouseButtonDown (0) && !gameManager.isGameOver && !isSuccess) 
		{
			if (Input.mousePosition.y > Screen.height * 0.9)
				return;

			if(rotateTweener != null && rotateTweener.IsPlaying())
				rotateTweener.Kill();

			if(!firstStart)
			{
				bool isOK = wheelLogic.DOCheck();
				if(isOK && !isSuccess && !gameManager.isGameOver)
				{
					wheelLogic.DOColorTriangle();
				}
				DORotateWheel ();
			}
			else
			{
				wheelLogic.DOColorTriangle();
				DOVirtual.DelayedCall(0.1f,()=>{
					DORotateWheel ();
				});
			}

			firstStart = false;
		}
	}
	/// <summary>
	/// Reference to the tweener who rotate the circle
	/// </summary>
	Tweener rotateTweener;
	/// <summary>
	/// Start the rotation of the wheel. Check in each updates if the triangle enter a part of the wheel with the same color of him. If we are inside a same color and we go out, that means the player doesn't tap before the triangle go out of the part with the same color, so it's game over.
	/// </summary>
	void DORotateWheel()
	{
		if (isSuccess && rotateTweener != null && rotateTweener.IsPlaying()) 
		{
			rotateTweener.Kill();
			return;
		}

		direction *= -1;

		if(rotateTweener != null && rotateTweener.IsPlaying())
			rotateTweener.Kill();

		bool isAfter = false;

		rotateTweener = transform.DORotate(Vector3.forward * (transform.rotation.eulerAngles.z + direction * 360f), wheelLogic.speedWheel, RotateMode.FastBeyond360)
			.SetEase(Ease.Linear)
			.OnUpdate( () => {
				if(!isAfter && wheelLogic.CheckIfTriangleEqualWheelColor())
				{
					isAfter = true;
				}

				if (isAfter && !wheelLogic.CheckIfTriangleEqualWheelColor()) 
				{
					if (!isSuccess) 
					{
						gameManager.GameOver ();
					}
					rotateTweener.Kill();
				}

				if (gameManager.isGameOver)
					rotateTweener.Kill();
			}).OnComplete(DORotateWheel);
	}
}
