using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;


public class Player : MonoBehaviour {
	
	public Tile[,,] tileMap;
	public Dictionary<Tile,Tuple<Player,int[]>> tileData;
	public Color playerColor;
	public GameManager gameMan;
	public Dictionary<Tile,Tile> bridgeData;
	private List<Tile> playerTiles;
	private List<Tile> invadeableTiles;
	private bool isTurn;
	private int movementCount;
	private string playerName;
	private float[] playerStats;
	private int playerScore;
	private int isleCount;

	public List<Tile> PlayerTiles {
		get {
			return this.playerTiles;
		}
		set {
			playerTiles = value;
		}
	}
	
	public List<Tile> InvadeableTiles {
		get {
			return this.invadeableTiles;
		}
		set {
			invadeableTiles = value;
		}
	}

	public bool IsTurn {
		get {
			return this.isTurn;
		}
		set {
			isTurn = value;
		}
	}

	public int MovementCount {
		get {
			return this.movementCount;
		}
		set {
			movementCount = value;
		}
	}

	public float[] PlayerStats {
		get {
			return this.playerStats;
		}
		set {
			playerStats = value;
		}
	}

	public int PlayerScore {
		get {
			return this.playerScore;
		}
		set {
			playerScore = value;
		}
	}

	void Awake(){
		initializeVar ();
	}

	private void initializeVar(){
		playerTiles = new List<Tile> ();
		invadeableTiles = new List<Tile> ();
		isTurn = false;
		movementCount = 0;
		gameMan = GameObject.Find("GameManager").GetComponent<GameManager>();
		playerStats = new float[2]{0.0f,0.0f};
		playerScore = 0;
	}

	////////////////////////////////////////////////////////////
	// Adds a tile to the players tile list and updates
	// the invadeable tiles.
	////////////////////////////////////////////////////////////

	public void AddPlayerTiles(Tile tile){
		playerTiles.Add(tile);
		tile.changeTokenColor (playerColor);
	}

	////////////////////////////////////////////////////////////
	// Removes a tile from the players tile list and updates
	// the invadeable tiles.
	////////////////////////////////////////////////////////////
	
	public void RemovePlayerTiles(Tile tile){
		playerTiles.Remove(tile);
	}

	public void getMovements(){
		movementCount = 2;
		if (gameMan.NumRounds != 1) {
			if(playerTiles.Count >= 13){
				movementCount++;
			}
			movementCount=movementCount + getIsleCount().Count;
		}
	}

	public List<int> getIsleCount(){
		List<int> isleList = new List<int>();
		for(int i = 0; i < gameMan.TileMap.Length ; i++){
			if(gameMan.TileMap[i].Intersect(playerTiles).Count() == gameMan.TileMap[i].Count()){
				isleList.Add(i);
			}
		}
		return isleList;
	}

	public float getPlayerStats(){
		float result;
		if (PlayerStats [0]!= 0) {
			result = (PlayerStats[0]/PlayerStats[1])*100;
			return result;
		}
		else{
			return 0;
		}
	}
}
