using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// In charge of all the wheel logic. Attached to the game object: "WheelParent". Create the colors, Spawn each element of the wheel. Check the color when the player tap the screen etc...
/// </summary>
public class WheelLogic : MonoBehaviourHelper
{
	/// <summary>
	/// Prefab of Circle. Use to create the wheel. Each part is a UI Image with a certain fillAmount
	/// </summary>
	public Circle circlePrefab;
	/// <summary>
	/// Number of parts in the wheel, for the current level
	/// </summary>
	int numOfPart = 12;
	/// <summary>
	/// Number of colors in the wheel, for the current level
	/// </summary>
	int numOfColor = 3;
	/// <summary>
	/// Sped of the wheel, in seconds (total time in seconds to make 360 degree rotation), for the current level
	/// </summary>
	[System.NonSerialized] public float speedWheel = 3f;
	/// <summary>
	/// Image of the shadow of border of the wheek
	/// </summary>
	public Image borderShadow;
	/// <summary>
	/// Image of the border of the wheek
	/// </summary>
	public Image border;
	/// <summary>
	/// Reference to the GameObject who contains all the part of the wheel we will spawn
	/// </summary>
	public RectTransform partParent;
	/// <summary>
	/// Reference to the Triangle Image on the top of the wheel
	/// </summary>
	public Image triangle;
	/// <summary>
	/// Reference to all the parts contained in the wheel, for the current level
	/// </summary>
	List<Circle> allCircles = new List<Circle>();
	/// <summary>
	/// Reference to the last color to find, to avoid duplicate check
	/// </summary>
	Color lastColor;
	/// <summary>
	/// Reference to a list of color built for a level
	/// </summary>
	public List<Color> listColorReordered = new List<Color>();
	/// <summary>
	/// Is it the first time we ask for a color in the game, for this level? If yes, don't get the color behind the triangle
	/// </summary>
	bool firstChangeColor = true;
	/// <summary>
	/// Create a new list of corlors for this level, randomly : listColorReordered and save it in PlayerPrefsX to use the same list of colors in case of game over
	/// </summary>
	void Awake()
	{
		firstChangeColor = true;

		DefineLevel();

		if(Util.RestartFromGameOver())
		{
			listColorReordered = new List<Color>();
			listColorReordered.AddRange(PlayerPrefsX.GetColorArray("_arrayColorSaved"));
		}
		else
		{
			listColorReordered.AddRange(colorManager.colors);
		}

		listColorReordered.Shuffle();
		listColorReordered.Shuffle();

		if(!Util.RestartFromGameOver())
		{
			while(listColorReordered.Count > numOfColor)
			{
				listColorReordered.RemoveAt(0);
			}
		}

		PlayerPrefsX.SetColorArray("_arrayColorSaved",listColorReordered.ToArray());
		PlayerPrefs.Save();
	}
	/// <summary>
	/// Place the border and the border shadow at the good place
	/// </summary>
	void Start()
	{
		float width = FindObjectOfType<Canvas>().GetComponent<RectTransform>().sizeDelta.x;

		borderShadow.rectTransform.sizeDelta = Vector2.one * width * 0.90f;
		border.rectTransform.sizeDelta = Vector2.one * width * 0.95f;

	
		BuildWheel();
	}
	/// <summary>
	/// IMPORTANT ==> It's here we define the levels. Change the formulas if you want. 
	/// </summary>
	void DefineLevel()
	{
		this.numOfColor = Math.Max(2,Mathf.Max(0, 1 + Util.GetCurrentLevel()/3) % (colorManager.colors.Length));
		this.numOfPart = 2 + Util.GetCurrentLevel() % 14;

//		this.numOfPart = (1 + (Util.GetCurrentLevel() - 1 ) % 5) * numOfColor;
//		this.speedWheel = 3f + 1f / ((float)(Util.GetCurrentLevel() % 20));
//		this.speedWheel = numOfPart / 2f;
	}
	/// <summary>
	/// Change the color of the triangle = color to find
	/// </summary>
	public void DOColorTriangle()
	{
		lastColor = triangle.color;

		Color newColor =  allCircles[UnityEngine.Random.Range(0, allCircles.Count)].image.color;

		if(firstChangeColor)
		{
			while(lastColor.IsEqual(allCircles[0].image.color) 
				|| lastColor.IsEqual(allCircles[1].image.color) 
				|| lastColor.IsEqual(allCircles[allCircles.Count - 1].image.color))
			{
				newColor =  allCircles[UnityEngine.Random.Range(0, allCircles.Count)].image.color;
			}
		}
		else
		{
			while(lastColor.IsEqual(newColor))
			{
				newColor =  allCircles[UnityEngine.Random.Range(0, allCircles.Count)].image.color;
			}
		}

		triangle.color = newColor;

		firstChangeColor = false;
	}
	/// <summary>
	/// Check if the player tap at the good moment on the screen, ie. check if the color of the triangle = the color of the part of the wheel below the triangle
	/// </summary>
	public bool CheckIfTriangleEqualWheelColor()
	{
		Circle selection =  allCircles[0];

		for(int i = 0; i < numOfPart; i++)
		{
			if(allCircles[i].GetMiddleAngle() <= selection.GetMiddleAngle())
			{
				selection = allCircles[i];
			}
		}

		if(selection.image.color.IsEqual(triangle.color))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	/// <summary>
	/// Call the method CheckIfTriangleEqualWheelColor. If true, move done = minus 1 moove in the total move the player have to do to clear the level. If false => game over
	/// </summary>
	public bool DOCheck()
	{
		bool check = CheckIfTriangleEqualWheelColor();
		if(check)
		{
			gameManager.MoveDone();
		}
		else
		{
			gameManager.GameOver();
		}
		return check;
	}
	/// <summary>
	/// Method to build the wheel. Each part of the wheel is an UI Image, type = fill image. We use the fill amout property to cretae the parts of the wheel
	/// </summary>
	void BuildWheel()
	{
		float countAngle = 0f;
		float sizePart = 1f/numOfPart;
	
		for(int i = 0; i < numOfPart ; i++)
		{
			float angle = i * 360 * sizePart;
			Color c = Color.white;

			int numColor = i;

			while(numColor >= listColorReordered.Count)
			{
				numColor -= listColorReordered.Count;
			}

			c = listColorReordered[numColor];

			Circle circle = InstantiateCircle(sizePart, angle, c);
			circle.name = i.ToString();

			countAngle += angle;

			allCircles.Add(circle);
		}
			
		if(wheelRotator.direction == 1)
		{
			lastColor = allCircles[1].image.color;
		}

		else if(wheelRotator.direction == -1)
		{
			lastColor = allCircles[0].image.color;
		}
	}
	/// <summary>
	/// Method to create a new circle = new part of the wheel
	/// </summary>
	Circle InstantiateCircle()
	{
		var go = Instantiate(circlePrefab.gameObject) as GameObject;
		go.transform.SetParent(partParent,false);
		var circle = go.GetComponent<Circle>();
		return circle;
	}
	/// <summary>
	/// Method to create a new circle = new part of the wheel
	/// </summary>
	Circle InstantiateCircle(float fillAmout, float angle, Color c)
	{
		return InstantiateCircle().Init(fillAmout,angle, c);
	}
}
