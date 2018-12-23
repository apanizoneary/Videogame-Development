using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NewGMenu : Menu {

	public QuestionMan qManager;
	public Canvas canvas;

	private string difficulty;
	private string theme;
	private string course;
	private int numPlayers;
	private int matchRounds;
	private GameObject gameSettings1;
	private GameObject gameSettings2;
	private Slider[] sliders;
	private GameObject[] playerNameInputs;
	private List<string>[] filters;
	private GameObject errorPanel;
	private ErrorMan errorMan;

	public override void Awake(){
		base.Awake ();
		gameSettings1 = transform.GetChild (1).gameObject;
		gameSettings2 = transform.GetChild (2).gameObject;
		playerNameInputs = new GameObject[6];
		for(int i=0 ; i<6 ; i++){
			playerNameInputs[i] = gameSettings2.transform.GetChild(1).GetChild(i).gameObject;
			playerNameInputs[i].SetActive(false);
		}
		sliders = GetComponentsInChildren<Slider>();
		difficulty = "";
		course = "";
		theme = "";
		numPlayers = 0;
		matchRounds = 0;
		sliders[0].transform.parent.GetComponentInChildren<Text>().text = "2";	
		sliders[1].transform.parent.GetComponentInChildren<Text>().text = "2";	
		errorPanel = canvas.transform.GetChild (2).gameObject;
		errorMan = errorPanel.GetComponentInChildren<ErrorMan>();
		resetPanel();
	}

	void Start(){
		filters = qManager.getFilters ();
		sliders[3].maxValue = (float)filters [0].Count;
		sliders[4].maxValue = (float)filters [1].Count;
	}

	public void btnStart(){
		string[] playerNames = new string[numPlayers];
		string playerName;
		for(int i=0 ; i < numPlayers ; i++){
			playerName = playerNameInputs[i].transform.GetComponentsInChildren<Text>()[2].text;
			if(playerName==""){
				errorMan.showError("WARNING : All players must have a name in order to play!", Color.yellow);
				return;
			}
			errorMan.remError ();
			PlayerPrefs.SetString("player" + (i+1) + "Name", playerName);

		}
		PlayerPrefs.SetInt("matchRounds", matchRounds);
		PlayerPrefs.SetInt("numPlayers", numPlayers);
		if(course!="" || theme!="" || difficulty!=""){
			qManager.filterQuestions(course,theme,difficulty);
		}
		Application.LoadLevel(1);
	}

	public void btnContinue(){
		numPlayers = (int) sliders[0].value;
		matchRounds = (int) sliders[1].value;
		difficulty = getDifficulty((int)sliders[2].value);
		if((int)sliders[3].value>0){
			theme = filters[0].ToArray()[(int)sliders[3].value-1];
		}
		if((int)sliders[4].value>0){
			course = filters[1].ToArray()[(int)sliders[4].value-1];
		}

		for(int i=0;i<numPlayers;i++){
			playerNameInputs[i].SetActive(true);
		}

		gameSettings1.SetActive (false);
		gameSettings2.SetActive (true);
	}

	public void btnBack(){
		foreach(GameObject go in playerNameInputs){
			go.SetActive(false);
		}
		errorMan.remError ();
		resetPanel ();
	}

	public void btnBacktoMenu(){
		resetPanel ();
	}

	public void updateNumberSlider(Slider slider){
		Text sliderText = slider.transform.parent.GetComponentInChildren<Text>();
		sliderText.text = slider.value.ToString();
	}

	public void updateCoursesSlider(){
		Text sliderText = sliders[4].transform.parent.GetComponentInChildren<Text> ();
		if(sliders[4].value == 0){
			sliderText.text = "";
		}else{
			sliderText.text = filters[1].ToArray()[(int)sliders[4].value-1];
		}
	}

	public void updateThemesSlider(){
		Text sliderText = sliders[3].transform.parent.GetComponentInChildren<Text> ();
		if(sliders[3].value == 0){
			sliderText.text = "";
		}else{
			sliderText.text = filters[0].ToArray()[(int)sliders[3].value-1];
		}
	}

	private void resetPanel(){
		gameSettings1.SetActive (true);
		gameSettings2.SetActive (false);
		sliders[0].value = 2;	
		sliders[1].value = 2;	
		sliders[2].value = 0;	
		sliders[3].value = 0;	
		sliders[4].value = 0;	
	}
}
