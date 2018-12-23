using UnityEngine;
using System.Collections.Generic;
using System.Linq;


public class GameManager : MonoBehaviour {

	private int numPlayers;
	private int currentPlayerIndex;
	private int numRounds;
	private int matchRounds;
	private bool isInvading;
	private bool isFirstPlayer;
	private Tile invadedTile;
	private Player currentPlayer;
	private GameGUI GUIman;
	private Dictionary<Tile,Player> tileOwners;
	private List<Tile> allTiles;
	private List<Tile>[] tileMap;
	private List<Player> playerList;
	
	private string theme;
	private string[] playerNames;

	public GameObject playerPrefab;
	public QuestionMan qManager;

	public int NumPlayers {
		get {
			return this.numPlayers;
		}
	}

	public bool IsInvading {
		get {
			return this.isInvading;
		}
	}

	public int CurrentPlayerIndex {
		get {
			return this.currentPlayerIndex;
		}
	}

	public Player CurrentPlayer {
		get {
			return this.currentPlayer;
		}
	}

	public int NumRounds {
		get {
			return this.numRounds;
		}
	}

	public List<Tile>[] TileMap {
		get {
			return this.tileMap;
		}
	}

	public List<Player> PlayerList {
		get {
			return this.playerList;
		}
	}

	void Awake(){
		initializeVar();
		getPlayerPrefs ();
		generatePlayers ();
	}

	void Start () {
		generateTileData ();
		assignTiles ();
		nextTurn ();
	}

	private void initializeVar(){
		GUIman = GetComponentInChildren<GameGUI>();
		qManager = GameObject.Find("QManager").GetComponent<QuestionMan>();
		currentPlayerIndex = 0;
		numRounds = 1;
		isInvading = false;
		invadedTile = null;
		isFirstPlayer = true;
	}

	///////////////////////////////////////////////////////////////////////////////////
	// Obtains the player preferences from the object passed on by the Menu Scene.
	///////////////////////////////////////////////////////////////////////////////////
	private void getPlayerPrefs(){
		numPlayers = PlayerPrefs.GetInt ("numPlayers");
		matchRounds = PlayerPrefs.GetInt ("matchRounds");
		playerNames = new string[numPlayers];
		
		for(int i=0;i<numPlayers;i++){
			playerNames[i] = PlayerPrefs.GetString("player" + (i+1) + "Name");
		}
	}

	////////////////////////////////////////////////////////
	// Creates a list containing all the tiles in the map.
	////////////////////////////////////////////////////////
	private void generateTileData(){
		allTiles = GetComponentsInChildren<Tile>().ToList();
		tileOwners = allTiles.ToDictionary(x => x, x => (Player) null);
		Transform gameWorld = transform.GetChild(1);
		int numIsles = gameWorld.transform.childCount;
		tileMap = new List<Tile>[numIsles];
		for (int i = 0; i<numIsles; i++){
			tileMap[i] = gameWorld.GetChild(i).GetComponentsInChildren<Tile>().ToList();	
		}
	}


	////////////////////////////////////////////////////////////
	// Generates the players at the start of the game.
	////////////////////////////////////////////////////////////
	private void generatePlayers(){
		GameObject playerGO;
		playerList = new List<Player> ();
		List<Color> playerColors = new List<Color>();
		playerColors.Add(Color.blue);		 			
		playerColors.Add(Color.cyan);
		playerColors.Add(Color.magenta);
		playerColors.Add(Color.yellow);
		playerColors.Add(Color.grey);
		playerColors.Add(Color.green);
		
		for(int i=0;i<numPlayers;i++){
			playerGO = Instantiate(playerPrefab) as GameObject;
			Player player = playerGO.GetComponent<Player>();
			player.playerColor = playerColors[i];
			player.name = playerNames[i];
			player.gameMan = this;
			playerList.Add(player);
		}
	}

	
	////////////////////////////////////////////////////////////
	// Assigns 2 tiles to each player at the start of the game  
	////////////////////////////////////////////////////////////
	private void assignTiles(){
		int i = 0;
		int rnd = 0;
		int lastTile = allTiles.Count-1;
		List<Tile> tiles = new List<Tile> (allTiles);
		foreach(Player pl in playerList){
			while(i<2){
				rnd = Random.Range(0,lastTile);
				pl.PlayerTiles.Add(tiles[rnd]);
				tiles[rnd].changeTokenColor (pl.playerColor);
				tileOwners[tiles[rnd]] = pl;
				tiles.RemoveAt(rnd);
				lastTile--;
				i++;
			}
			i=0;
		}
	}

	////////////////////////////////////////////////////////////
	// Processes which player's turn is next. If the end of the 
	// player list is reached, the round has finished and a new 
	// one starts with player[0].
	////////////////////////////////////////////////////////////
	private void nextTurn(){
		if(!isFirstPlayer){
			if (currentPlayerIndex + 1 < numPlayers) {
				currentPlayerIndex++;		
			} else {
				currentPlayerIndex=0;
				numRounds++;
			}
			if(numRounds == matchRounds){
				endMatch();
			}
		}
		else{
			isFirstPlayer = false;
		}
		currentPlayer = playerList[currentPlayerIndex];
		setTileHighlight ();
		currentPlayer.getMovements();
		GUIman.updateGUI ();
	}

	////////////////////////////////////////////////////////
	// Brings up player scores.
	////////////////////////////////////////////////////////
	private void endMatch(){
		processPlayerScores ();
		GUIman.playerScores();
	}

	////////////////////////////////////////////////////////
	// Restarts Menu Scene
	////////////////////////////////////////////////////////
	public void endGame(){
		qManager.resetNextGameQuestions ();
		Application.LoadLevel(0);
	}

	////////////////////////////////////////////////////////
	// Gives basic scores to each player. Each player gets
	// 1 point for each Island and 1 point for 10+ territories.
	////////////////////////////////////////////////////////
	private void getBasicScores(Player player){
		if(player.PlayerTiles.Count>10){
			player.PlayerScore+=1;
		}
		player.PlayerScore+=(int)player.getIsleCount().Count;
		if(player.getPlayerStats() > 65f){
			player.PlayerScore+=1;
		}
	}
	////////////////////////////////////////////////////////////
	// Gives final scores for each player.
	//
	// Compares players to check which player had the most territories,
	// the most islands and the highest success rate. Best player
	// in each category gets the following points :
	//
	//				Most territories : +1 point
	//				Most islands : +2 points
	//				Highest Success : +1 point
	//
	////////////////////////////////////////////////////////////
	private void processPlayerScores(){

		Player pl = playerList [0];
		List<Player> mostTiles = new List<Player>(){pl};
		List<Player> mostIsles = new List<Player>(){pl};
		List<Player> bestStats = new List<Player>(){pl};
		float A, B;

		getBasicScores (pl);

		for (int i = 1; i<numPlayers; i++) {
			A = (float)playerList[i].PlayerTiles.Count;
			B = (float)mostTiles[0].PlayerTiles.Count;
			if(A > B){
				mostTiles.Clear();
				mostTiles.Add(playerList[i]);
			}	
			else if (A == B){
				mostTiles.Add(playerList[i]);
			}

			A = (float)playerList[i].getIsleCount().Count;
			B = (float)mostIsles[0].getIsleCount().Count;
			if(A > B){
				mostIsles.Clear();
				mostIsles.Add(playerList[i]);
			}	
			else if (A == B){
				mostIsles.Add(playerList[i]);
			}

			A = playerList[i].getPlayerStats();
			B = bestStats[0].getPlayerStats();
			if(A > B){
				bestStats.Clear();
				bestStats.Add(playerList[i]);
			}	
			else if (A == B){
				bestStats.Add(playerList[i]);
			}

			getBasicScores(playerList[i]);
		}
		
		foreach(Player player in mostTiles){
			player.PlayerScore+=1;
		}
		foreach(Player player in mostIsles){
			if(player.getIsleCount().Count != 0){
				player.PlayerScore+=2;
			}
		}
		foreach(Player player in bestStats){
			player.PlayerScore+=1;
		}
	}

	////////////////////////////////////////////////////////////
	// Highlights in red the tiles the current player can invade
	////////////////////////////////////////////////////////////
	private void setTileHighlight(){
		List <Tile> highlightTiles = new List<Tile> ();
		foreach(Tile t in allTiles){
			t.isHighlight = false;
		}
		foreach (Tile playerTile in currentPlayer.PlayerTiles){
			foreach (Tile neighbourTile in playerTile.neighbourTiles){
				if(!highlightTiles.Contains(neighbourTile)){
					if(tileOwners[neighbourTile]!=null){
						if(tileOwners[neighbourTile]!=currentPlayer){
							if(tileOwners[neighbourTile].PlayerTiles.Count > 2){
								highlightTiles.Add(neighbourTile);
								neighbourTile.isHighlight=true;
							}
						}
					}
					else{
						highlightTiles.Add(neighbourTile);
						neighbourTile.isHighlight=true;
					}
				}
			}		
		}
	}


	////////////////////////////////////////////////////////////
	//  Activates invade GUI and sets invading status to true.
	////////////////////////////////////////////////////////////
	public void invadeTerritory(Tile tile){
		isInvading = true;
		invadedTile = tile;
		GUIman.startInvade ();
	}

	//////////////////////////////////////////////////////////////
	// Ends the invade and processes the outcome. If the invade 
	// succeded, adds the tile to the invading player's tile list
	// and removes it from the player's list it belonged to, if any.
	//////////////////////////////////////////////////////////////
	public void endInvade(bool success){
		if(success){
			if(tileOwners[invadedTile] != null){
				tileOwners[invadedTile].RemovePlayerTiles(invadedTile);
			}
			tileOwners[invadedTile] = currentPlayer;
			currentPlayer.AddPlayerTiles(invadedTile);
			currentPlayer.PlayerStats[0]++;
		}
		currentPlayer.PlayerStats[1]++;
		currentPlayer.MovementCount--;
		if (currentPlayer.MovementCount == 0) {
			nextTurn();		
		}
		else{
			GUIman.updateGUI();
			setTileHighlight ();
		}
		isInvading = false;
	}
}
