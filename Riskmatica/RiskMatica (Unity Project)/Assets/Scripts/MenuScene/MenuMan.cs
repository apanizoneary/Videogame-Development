using UnityEngine;
using System.Collections;

public class MenuMan : MonoBehaviour {

	public Menu CurrentMenu;

	public void Start(){
		ShowMenu(CurrentMenu);
	}

	public void ShowMenu(Menu menu){
		if (CurrentMenu != null) {
			CurrentMenu.IsOpen = false;
		}
		if (menu != null) {
			CurrentMenu = menu;
			CurrentMenu.IsOpen = true;				
		}
	}

	public void hideMenu(Menu menu){
		if (CurrentMenu != null) {
			CurrentMenu.IsOpen = false;
			CurrentMenu = null;
		}
	}

	public void CloseGame(){
		Application.Quit();
	}
}
