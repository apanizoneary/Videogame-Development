using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

	protected CanvasGroup canvasGroup;
	protected bool isOpen;

	public bool IsOpen {
		get {
			return isOpen;
		}
		set {
			isOpen = value;
		}
	}
	
	public virtual void Awake(){
		canvasGroup = GetComponent<CanvasGroup>();
		canvasGroup.alpha = 0;

		var rect = GetComponent<RectTransform> ();
		rect.offsetMax = rect.offsetMin = new Vector2 (0,0);
	}
	
	public virtual void Update(){
		if (isOpen) {
			canvasGroup.alpha = 1;
			canvasGroup.blocksRaycasts = canvasGroup.interactable = true;
		}
		else {
			canvasGroup.alpha = 0;
			canvasGroup.blocksRaycasts = canvasGroup.interactable = false;
		}
	}

	public void updateDifficultyValue(Slider slider){
		Text sliderText = slider.transform.parent.GetComponentInChildren<Text> ();
		sliderText.text = getDifficulty((int)slider.value);
	}

	protected string getDifficulty(int value){
		string diff = "";
		switch (value) {
		case 0:
			diff = "";
			break;
		case 1: 
			diff = "Low";	
			break;
		case 2: 
			diff = "Medium";
			break;
		case 3: 
			diff = "High";
			break;
		}
		return diff;
	}
}
