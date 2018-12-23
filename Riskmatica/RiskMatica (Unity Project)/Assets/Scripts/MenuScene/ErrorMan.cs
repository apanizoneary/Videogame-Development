using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ErrorMan : MonoBehaviour {

	private Text errorText;
	private CanvasGroup epCanvasGroup;


	void Awake () {
		epCanvasGroup = GetComponentInParent<CanvasGroup>();
		epCanvasGroup.alpha = 1;
		epCanvasGroup.blocksRaycasts = epCanvasGroup.interactable = false;
		errorText = GetComponent<Text> ();
		errorText.text = "";
	}
	
	public void showError(string errorMsg, Color color){
		errorText.text = errorMsg;
		errorText.color = color;
	}

	public void remError(){
		errorText.text = "";
	}
}
