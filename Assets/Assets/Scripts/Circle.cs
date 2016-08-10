using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Each part of the wheel is a circle. We use the fillAmount component of UI image to get "parts". All the circles are child of the Game Object PartParent (= WheelRotator). The Circle prefab is in the Prefabs folder. Each Circles are instantiate in the WheelLogic at the start of each level
/// </summary>
public class Circle : MonoBehaviour 
{
	/// <summary>
	/// The image = a simple circle
	/// </summary>
	public Image image;

	/// <summary>
	/// Init the circle = the part of the wheel. Each part is defined with a fillAmount = 1 / number of part in the wheel, an angle and a color
	/// </summary>
	public Circle Init(float fillAmout, float angle, Color color)
	{
		image.type = Image.Type.Filled;
		image.fillAmount = fillAmout;
		image.rectTransform.localPosition = Vector3.zero;
		image.rectTransform.eulerAngles = Vector3.forward * angle;
		image.rectTransform.SetSiblingIndex(0);

		float width = FindObjectOfType<Canvas>().GetComponent<RectTransform>().sizeDelta.x;

		image.rectTransform.sizeDelta = Vector2.one * width * 0.9f;
		image.color = color;
		return this;
	}
	/// <summary>
	/// Get the angle of the middle of the part of wheel
	/// </summary>
	public float GetMiddleAngle()
	{
		float midAngle = image.rectTransform.eulerAngles.z;

		while(midAngle < 0)
		{
			midAngle += 360f;
		}

		while(midAngle > 360)
		{
			midAngle -= 360f;
		}

		return midAngle;
			
	}
}
