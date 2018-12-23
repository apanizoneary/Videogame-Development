using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;

///////////////////////////////////////////////////////////////////////////////////////////////
// This project was designed for a maths department that worked with LaTeX. They wanted
// to include new questions into the game, so this script, BrowserMan, Menu and ThreadedJob
// together with the third party tools MikText and Ghostscript provide that functionality.
//
// I have disabled this functionality from the actual build since it is not too useful
// out of that context. To enable it, activate the QuestionsBtn gameobject from the hierarchy.
///////////////////////////////////////////////////////////////////////////////////////////////

public class NewQMenu : Menu {
	
	public QuestionMan qManager;
	public Canvas canvas;
	public Menu qMenu;

	private string imgPath;
	private string dataPath;
	private string batPath;
	private string texPath;
	private string workPath;
	private string imagesDirPath;
	private string imagePath;
	private string errorLogPath;
	private string outputLogPath;

	private BrowserMan FileBrowser;
	private ThreadedJob texJob;
	private int nextID;
	private bool isLoad;
	private bool loadingOn;
	private bool displayOn;
	private GameObject viewingPanel;
	private GameObject errorPanel;
	private ErrorMan errorMan;
	private CanvasGroup vpCanvasGroup;

	public string TexPath {
			get {
				return this.texPath;
			}
			set {
				texPath = value;
			}
		}

	public override void Awake(){

		base.Awake ();

		dataPath = Application.dataPath;
		imagesDirPath = dataPath + "/Images";
		workPath = dataPath + "/TEX";
		errorLogPath = workPath + "/errorLog.txt";
		outputLogPath = workPath + "/outputLog.txt";
		batPath = workPath + "/textopng.bat";

		if(!Directory.Exists(imagesDirPath)){    
			Directory.CreateDirectory(imagesDirPath);
		}

		displayOn = false;
		loadingOn = false;

		errorPanel = canvas.transform.GetChild (2).gameObject;
		errorMan = errorPanel.GetComponentInChildren<ErrorMan>();
		viewingPanel = canvas.transform.GetChild (1).gameObject;
		vpCanvasGroup = viewingPanel.GetComponent<CanvasGroup> ();
		vpCanvasGroup.alpha = 0;
		vpCanvasGroup.blocksRaycasts = vpCanvasGroup.interactable = false;	
		FileBrowser = this.GetComponentInChildren<BrowserMan>();
	}
	
	public override void Update(){

		if (!isOpen){
			canvasGroup.alpha = 0;
			canvasGroup.blocksRaycasts = canvasGroup.interactable = false;
		}

		if(texPath != null){
			viewingPanel.GetComponentInChildren<Text>().text = "Processing LaTeX ...";
			nextID = qManager.getNextID ();
			imgPath = imagesDirPath + "/question_" + nextID + ".png";

			if (System.IO.File.Exists(errorLogPath)) {
				System.IO.File.Delete(errorLogPath);		
			}

			if (System.IO.File.Exists(outputLogPath)) {
				System.IO.File.Delete(outputLogPath);		
			}

			texJob = new ThreadedJob();
			texJob.nextID = nextID;
			texJob.texPath = texPath;
			texJob.imgPath = imgPath;
			texJob.workPath = workPath;
			texJob.Start();
			texPath = null;

			vpCanvasGroup.alpha=1;
			canvasGroup.alpha = 1;
			canvasGroup.blocksRaycasts = canvasGroup.interactable = true;
		}

		if (texJob != null){
			if (texJob.Update() && !loadingOn){
				string wwwURL = "file:///" + imgPath;
				StartCoroutine(loadImage(wwwURL));
				loadingOn = true;
			}
		}
	}
	public void saveQuestion(){
		
		Question newQ = new Question();
		Slider diffSlider = GetComponentInChildren<Slider> ();
		InputField[] ifsQuestion = GetComponentsInChildren<InputField> ();
		
		if (!displayOn){
			errorMan.showError("WARNING : LaTeX file must be processed first before saving the question.", Color.yellow);
		}
		else {
			if(System.IO.File.Exists(errorLogPath)){
				if (new FileInfo(errorLogPath).Length != 0){
					errorMan.showError("ERROR : The LaTeX file has errors. Please fix them and try again.", Color.red);
				}
				else{
					if (ifsQuestion[0].text == "" || ifsQuestion[1].text == "" || ifsQuestion[2].text == "" || ifsQuestion[3].text == ""){
						errorMan.showError("WARNING : All fields must be filled.", Color.yellow);
					}
					else if (4 < int.Parse(ifsQuestion[2].text)){
						errorMan.showError("WARNING : The number of questions must be higher than 4.", Color.yellow);
					}
					else if (int.Parse(ifsQuestion[2].text) < int.Parse(ifsQuestion[3].text)){
						errorMan.showError("WARNING : The number for the correct answer cannot be higher than the number of questions.", Color.yellow);
					}
					else {
						newQ.qID = nextID;
						newQ.theme = ifsQuestion[0].text;
						newQ.course = ifsQuestion[1].text;
						newQ.numAnsw = int.Parse(ifsQuestion[2].text);
						newQ.answ = int.Parse(ifsQuestion[3].text);
						newQ.imgPath = "/Images/question_" + nextID + ".png";
						newQ.difficulty = getDifficulty((int)diffSlider.value);
						qManager.addQuestion (newQ);
						qManager.updateQManager();
						qManager.resetNextGameQuestions();
						resetUI ();
					}
				}
			}
			else {
				errorMan.showError("ERROR : The LaTeX file has errors. Please fix them and try again.", Color.red);
			}
		}
	}
	
	public void backBtn(){
		if (System.IO.File.Exists(imgPath)) {
			System.IO.File.Delete(imgPath);		
		}

		texJob.Abort ();
		resetUI ();
	}

	private void resetUI(){
		canvas.GetComponent<MenuMan>().ShowMenu(qMenu);
		vpCanvasGroup.alpha = 0;
		viewingPanel.transform.GetChild (0).gameObject.GetComponent<RawImage> ().texture = null;
		texJob = null;
		displayOn = false;
		loadingOn = false;

		Slider diffSlider = GetComponentInChildren<Slider> ();
		InputField[] ifsQuestion = GetComponentsInChildren<InputField> ();
		diffSlider.value = 0;
		diffSlider.transform.parent.GetComponentInChildren<Text>().text = "Low";
		ifsQuestion[0].text="";
		ifsQuestion[1].text="";
		ifsQuestion[2].text="";
		ifsQuestion[3].text="";
		errorMan.remError ();
	}

	private IEnumerator loadImage(string url){
		Texture2D texImage = new Texture2D(512,512, TextureFormat.DXT5, false);
		WWW www = new WWW(url);
		yield return www;
		texImage = www.textureNonReadable;
		viewingPanel.GetComponentInChildren<Text>().text = "";
		viewingPanel.transform.GetChild(0).gameObject.GetComponent<RawImage>().texture = texImage;
		displayOn = true;
	}
}
