using UnityEngine;
using System.Collections;

public class BrowserMan : MonoBehaviour {
	
	string message = "";
	float alpha = 1.0f;
	char pathChar = '/';
	private bool isLoad;
	private bool msgRcv;
	public Canvas canvas;
	public Menu qMenu;
	public NewQMenu nqMenu;

	public bool IsLoad{
		get{return isLoad;}		
		set{isLoad = value;}
	}		

	void Awake(){
		isLoad = false;
		msgRcv = false;
	}

	void OnGUI () {
		getRect();
		if (isLoad) {
			UniFileBrowser.use.OpenFileWindow (OpenFile);
		}
		isLoad=false;
	}

	private void getRect(){
		RectTransform rectTransform = GetComponentInParent<RectTransform>();
		RectTransform canvasRT = canvas.GetComponent<RectTransform>();
		Vector2 canvasSize = canvasRT.sizeDelta;
		Vector2 VPminAnc = rectTransform.anchorMin;
		Vector2 VPmaxAnc = rectTransform.anchorMax;

		Vector2 windowSize = new Vector2(Mathf.Abs((canvasSize.x*VPminAnc.x)-(canvasSize.x*VPmaxAnc.x)),(Mathf.Abs((canvasSize.y*VPminAnc.y)-(canvasSize.y*VPmaxAnc.y))));
		Vector2 windowsPos = new Vector2(VPminAnc.x*canvasSize.x,(1-VPmaxAnc.y)*canvasSize.y);

		UniFileBrowser.use.SetFileWindowPosition(windowsPos);
		UniFileBrowser.use.SetFileWindowSize(windowSize);
	}

	public void closeBrowser(){
		isLoad = false;
		UniFileBrowser.use.CloseFileWindow ();
	}

	void OpenFile (string pathToFile) {
		message = pathToFile;
		if (message != null) {
			canvas.GetComponent<MenuMan>().ShowMenu(nqMenu);
			nqMenu.GetComponent<NewQMenu>().TexPath = message;
		}
		else{
			canvas.GetComponent<MenuMan>().ShowMenu(qMenu);
		}
		Fade();
	}

	void Fade () {
		StopCoroutine ("FadeAlpha");	
		StartCoroutine ("FadeAlpha");
	}
	
	IEnumerator FadeAlpha () {
		alpha = 1.0f;
		yield return new WaitForSeconds (5.0f);
		for (alpha = 1.0f; alpha > 0.0f; alpha -= Time.deltaTime/4) {
			yield return null;
		}
		message = "";
	}
}