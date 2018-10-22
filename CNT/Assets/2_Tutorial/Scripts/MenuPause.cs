using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuPause : MonoBehaviour {

	[Header ("Animator")]
	public Animator anim_PauseMenu;
	public Animator anim_ScreenFade;

	public void Pause (bool isPausing) {
		if (isPausing) {
			anim_PauseMenu.SetBool ("PauseOn", true);
		} else {
			anim_PauseMenu.SetBool ("PauseOn", false);
			Time.timeScale = 1;
		}
	}

	public void MainMenu () {
		Time.timeScale = 1;
		ThrowingController.isGameOver = false;
		anim_ScreenFade.SetTrigger ("GoToMenu");
		StartCoroutine (goToMenu ());
	}

	IEnumerator goToMenu () {
		yield return new WaitForSeconds (1.2f);
		SceneManager.LoadScene (0);
	}

	public void SetTimeScale () {
		Time.timeScale = 0;
	}

}
