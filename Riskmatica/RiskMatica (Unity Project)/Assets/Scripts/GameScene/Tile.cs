using UnityEngine;
using System.Collections.Generic;

public class Tile: MonoBehaviour {

	private Color highlightColor;
	public bool isHighlight;
	public List<Tile> neighbourTiles;
	private GameManager gameMan;
	private Color tileColor;
	private Ray ray;
	private RaycastHit hitInfo;
	private GameObject flag;
	private GameObject pole;
	private GameObject token;

	void Awake(){
		initializeVar();
	}

	private void initializeVar(){
		gameMan = transform.parent.GetComponentInParent<GameManager> ();
		token = transform.GetChild(0).gameObject;
		token.renderer.enabled = false;
		tileColor = renderer.material.color;
		highlightColor = Color.red;
		isHighlight = false;
	}

	void Update(){
		if (isHighlight){
			renderer.material.color = highlightColor;
			if(!gameMan.IsInvading){
				ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if(collider.Raycast(ray,out hitInfo, Mathf.Infinity) && Input.GetMouseButtonDown(0)){
					gameMan.invadeTerritory(this);
				}
			}
		}
		else{
			renderer.material.color = tileColor;
		}
	}

	public void changeTokenColor(Color color){
		token.renderer.material.color = color;
		token.renderer.enabled = true;
	}

}
