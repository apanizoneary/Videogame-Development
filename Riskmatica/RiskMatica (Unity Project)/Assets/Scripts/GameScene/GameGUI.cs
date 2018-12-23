using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameGUI : MonoBehaviour {

	private CanvasGroup invadePanelCG;
	private CanvasGroup playerPanelCG;
	private CanvasGroup gamePanelCG;
	private CanvasGroup scorePanelCG;
	private GameManager gameMan;
	private List<Question> qDone;
	private List<Question> qList;
	private Question qCurrent;
	private Transform invadePanel;
	private Transform playerPanel;
	private Transform gamePanel;
	private Transform scorePanel;
	private Text[] qTexts;
	private Text[] pTexts;
	private Text[][] playerScoresText;
	private Text[][] playerTerritoriesText;
	private Text roundText;
	private Text timerText;
	private Transform playersGO;
	private Transform scoresGO;
	private RawImage qDisplayer;
	private Button[] qButtons;
	private bool invSuccess;
	private Slider qTimer;
	private int answerTime;
	private int timer;
	private int numPlayers;

	public Text[] PTexts {
		get {
			return this.pTexts;
		}
	}

	void Awake () {
		initializeVar();
	}

	void OnEnable(){
		numPlayers = gameMan.NumPlayers;
		qList = new List<Question>(gameMan.qManager.getQuestions());
		setupGamePanel ();
		setupInvadePanel ();
		setupPlayerPanel ();
		setupScorePanel ();
	}

	private void initializeVar(){
		invadePanel = transform.GetChild(0);
		playerPanel = transform.GetChild(1);
		gamePanel = transform.GetChild(2);
		scorePanel = transform.GetChild(3);
		gameMan = GetComponentInParent<GameManager> ();
		invSuccess = false;
		answerTime = 20;
	}

	private void setupPlayerPanel(){
		pTexts = playerPanel.GetComponentsInChildren<Text> ();
		playerPanelCG = playerPanel.GetComponent<CanvasGroup> ();
		showPanel (playerPanelCG, true);
	}

	private void setupGamePanel(){
		GameObject[] playerTerritories = new GameObject[numPlayers];
		playerTerritories[0] = gamePanel.GetChild(1).gameObject;
		playerTerritoriesText = new Text[numPlayers][];
		playerTerritoriesText[0] = playerTerritories [0].GetComponentsInChildren<Text> ();
		
		for(int i=1; i < numPlayers ;i++){
			playerTerritories[i] = Instantiate(playerTerritories[0]) as GameObject;
			playerTerritories[i].transform.SetParent(gamePanel);
			playerTerritoriesText[i] = playerTerritories[i].GetComponentsInChildren<Text>();
		}
		
		roundText = gamePanel.GetChild(0).GetComponent<Text>();
		gamePanelCG = gamePanel.GetComponent<CanvasGroup> ();
		showPanel (gamePanelCG, true);
	}

	private void setupInvadePanel(){
		qTimer = invadePanel.GetComponentInChildren<Slider>();
		timerText = qTimer.GetComponentInChildren<Text> ();
		qDisplayer = invadePanel.GetComponentInChildren<RawImage> ();
		qTexts = invadePanel.GetComponentsInChildren<Text> ();
		qButtons = invadePanel.GetComponentsInChildren<Button> ();
		qTexts [0].text = "";
		qTexts [1].text = "";
		qCurrent = null;
		qDone = new List<Question> ();

		foreach(Button btn in qButtons){
			btn.gameObject.SetActive(false);
		}
		invadePanelCG = invadePanel.GetComponent<CanvasGroup> ();
		showPanel (invadePanelCG, false);
	}

	private void setupScorePanel(){
		GameObject scoreLine = scorePanel.GetChild(1).GetChild(0).gameObject;
		GameObject playerScores;
		playerScoresText = new Text[numPlayers][];
		
		for(int i=0; i < numPlayers ;i++){
			playerScores = Instantiate(scoreLine) as GameObject;
			playerScores.transform.SetParent(scoreLine.transform.parent);
			playerScoresText[i] = playerScores.GetComponentsInChildren<Text>();
		}
		scorePanelCG = scorePanel.GetComponent<CanvasGroup> ();
		showPanel (scorePanelCG, false);
	}

	public void playerScores(){

		showPanel (playerPanelCG, false);
		showPanel (gamePanelCG, false);
		showPanel (invadePanelCG, false);
		showPanel (scorePanelCG, true);
		int i = 0;

		foreach(Player pl in gameMan.PlayerList){
			playerScoresText[i][0].text = pl.name; 
			playerScoresText[i][0].color = pl.playerColor; 
			playerScoresText[i][1].text = pl.PlayerTiles.Count.ToString();
			playerScoresText[i][2].text = pl.getIsleCount().Count.ToString();
			playerScoresText[i][3].text = pl.getPlayerStats().ToString("F1") + " % ";
			playerScoresText[i][4].text = pl.PlayerScore.ToString();
			i++;
		}
	}

	public void startInvade(){
		nextQuestion();
		for(int i = 0 ; i < qCurrent.numAnsw ; i++){
			qButtons[i].gameObject.SetActive(true);	
		}
		qButtons[4].gameObject.SetActive(false);	
		qTimer.gameObject.SetActive (true);
		StartCoroutine ("startTimer");	
		StartCoroutine("startProgress");
		showPanel (playerPanelCG, false);
		showPanel (gamePanelCG, false);
		showPanel (invadePanelCG, true);
	}

	private void nextQuestion(){
		if(qList.Count == 0){
			qList = new List<Question>(gameMan.qManager.getQuestions());
		}
		qCurrent = qList[0];
		qList.RemoveAt(0);
		qTexts[0].text = "Question about " + qCurrent.theme + " for player " + gameMan.CurrentPlayer.name;
		StartCoroutine(loadImage("file:///" + Application.dataPath + qCurrent.imgPath));
	}

	public void updateGUI(){
		Player currentPlayer = gameMan.CurrentPlayer;

		pTexts[0].text = currentPlayer.name ;
		pTexts[0].color = currentPlayer.playerColor;
		pTexts[1].text = "Controlled territories : " + currentPlayer.PlayerTiles.Count;
		pTexts[2].text = "Controlled islands : " + currentPlayer.getIsleCount().Count;
		pTexts[3].text = "Attacks available : " + currentPlayer.MovementCount;
		pTexts[4].text = "Success rate : " + currentPlayer.getPlayerStats().ToString("F1") + " % ";

		int i = 0;
		roundText.text = "Round " + gameMan.NumRounds;
		foreach (Player pl in gameMan.PlayerList) {
			playerTerritoriesText[i][0].text = pl.name + " : ";
			playerTerritoriesText[i][0].color = pl.playerColor;
			playerTerritoriesText[i][1].text = pl.PlayerTiles.Count.ToString();
			i++;
		}
	}

	public void answerBtn(int ansNum){
		StopCoroutine ("startTimer");	
		StopCoroutine("startProgress");
		qDisplayer.texture = null;	
		if (ansNum == 0){
			qTexts[1].text = "Times up! The attack was unsuccessful!";
			qTexts[1].color = Color.red;
			invSuccess = false;
		}
		else{
			if(ansNum == qCurrent.answ){
				qTexts[1].text = "Correct answer! The attack was successful!";
				qTexts[1].color = Color.green;
				invSuccess = true;
			}
			else{
				qTexts[1].text = "Incorrect answer! The attack was unsuccessful!";
				qTexts[1].color = Color.red;
				invSuccess = false;
			}
		}
		foreach (Button btn in qButtons) {
			btn.gameObject.SetActive(false);			
		}
		qButtons[4].gameObject.SetActive(true);
		qTimer.gameObject.SetActive (false);
	}

	public void continueBtn(){
		showPanel (invadePanelCG, false);
		showPanel (playerPanelCG, true);
		showPanel (gamePanelCG, true);
		qTexts[1].text = "";
		gameMan.endInvade (invSuccess);
	}

	private void showPanel(CanvasGroup panel,bool isVisible){
		int alpha = 0;
		if (isVisible) {
			alpha = 1;	
		}
		panel.alpha = alpha;
		panel.blocksRaycasts = panel.interactable = isVisible;
	}

	private IEnumerator loadImage(string url){
		Texture2D texImage = new Texture2D(512,512, TextureFormat.DXT5, false);
		WWW www = new WWW(url);
		yield return www;
		texImage = www.textureNonReadable;
		qDisplayer.texture = texImage;
	}

	private IEnumerator startTimer(){
		timer = answerTime;
		int minutes,seconds = 0;
		while(timer != 0){
			minutes = timer/60;
			seconds = timer%60;
			timerText.text = minutes.ToString("0") + " : " + seconds.ToString("00");
			timer--;
			yield return new WaitForSeconds(1);
		}
		answerBtn (0);
	}

	private IEnumerator startProgress(){
		qTimer.value = 0;
		qTimer.maxValue = answerTime+1;
		while(qTimer.value != answerTime){
			qTimer.value = answerTime - timer;
			yield return null;
		}
	}
}
