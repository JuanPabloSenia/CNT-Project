using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//UBICACION: MANAGER_Main Menu, en escena "0-Main Menu"
public class MenuManager : MonoBehaviour {

	[Header ("Animator")]
	public Animator anim_MainMenu;
	public Animator anim_ScreenFade;

	[Header ("Buttons")]
	public GameObject buttonsMenu;
	public GameObject btnBack;
	public GameObject btnBookPlay;
	public GameObject btnBookContactUs;
	public Button btnPlayLevel;
	public ToggleGroup toggleGroupCharacters;

	[Header ("GameObjects")]
	public Canvas canvas_Menu;
	public GameObject canvas_CharacterSelection;

    [Header("Others")]
    public activateFade _activateFade;
	public Text txtVersion;

	string link;


	void Start () {
		ComboTextLogic.combo = 0;
		txtVersion.text = "v" + Application.version;
		canvas_CharacterSelection.SetActive (false);
		btnPlayLevel.interactable = false;
	}

	//Abre links para medios de contacto
	public void menuOpenLink (string name) {
		switch (name) {
			case "Facebook":
				link = "https://www.facebook.com/bearslapgames/";
				break;
			case "Instagram":
				link = "https://www.instagram.com/bearslapgames/";
				break;
			case "Email":
				link = "https://www.facebook.com/bearslapgames/";
				break;
		}
		Application.OpenURL (link);
	}

	//Interacciones con todos los botones del Menu Principal
	public void menuButtons (string menu) {
		switch (menu) {
			case "Play":
				anim_MainMenu.SetBool ("Play", true);
				btnBookPlay.SetActive (true);
				buttonsMenu.SetActive (false);
				btnBack.SetActive (true);
				break;
			case "ContactUs":
				anim_MainMenu.SetBool ("ContactUs", true);
				btnBookContactUs.SetActive (true);
				buttonsMenu.SetActive (false);
				btnBack.SetActive (true);
				break;
			case "Back":
				anim_MainMenu.SetBool ("Play", false);
				anim_MainMenu.SetBool ("ContactUs", false);
				btnBookPlay.SetActive (false);
				btnBookContactUs.SetActive (false);
				buttonsMenu.SetActive (true);
				btnBack.SetActive (false);
				break;
		}
	}

	//Seleccion de nivel
	public void menuSelectCharacter (int _level) {
		DATA.instance.level = _level;
		btnBookPlay.SetActive (false);
		btnBack.SetActive (false);
        canvas_Menu.enabled = false;
		canvas_CharacterSelection.SetActive (true);
	}

	//Nos lleva a la pantalla de carga y luego al nivel seleccionado
	public void menuPlay () {
		switch (DATA.instance.idCharacter) {
			case 0:
				DATA.instance.selectedCharacter = DATA.Characters.TEST;
				break;
			case 1:
				DATA.instance.selectedCharacter = DATA.Characters.PALADIN;
				break;
			case 2:
				DATA.instance.selectedCharacter = DATA.Characters.WIZARD;
				break;
			case 3:
				DATA.instance.selectedCharacter = DATA.Characters.BLACKSMITH;
				break;
		}
        canvas_Menu.enabled = true;
        canvas_CharacterSelection.SetActive (false);
		anim_MainMenu.SetBool ("PlayLevel", true);
        StartCoroutine (goToLevel ());
	}

	//Nos lleva a la pantalla de carga
	IEnumerator goToLevel () {
        yield return new WaitForSeconds(1);
        _activateFade.fadeOff();
        yield return new WaitForSeconds (1);
        anim_ScreenFade.SetTrigger ("GoToMenu");
		yield return new WaitForSeconds (1);
		SceneManager.LoadScene (1); 
	}

	//Selecciona el tutorial
	public void menuGoToTutorial () {
		DATA.instance.level = 0;
		btnBookPlay.SetActive (false);
		btnBack.SetActive (false);
		anim_MainMenu.SetBool ("PlayLevel", true);
		StartCoroutine (goToLevel ());
	}

	//Check de los personajes para habilitar boton de Play
	public void toggleCharacters (int id) {
		btnPlayLevel.interactable = false;
		DATA.instance.idCharacter = id;
		foreach (Toggle t in toggleGroupCharacters.ActiveToggles ()) {
			if (t.isOn) {
				btnPlayLevel.interactable = true;
				break;
			}
		}
	}


	

	//END
}