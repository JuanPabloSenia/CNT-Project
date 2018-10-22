using System.Collections;
using UnityEngine;

//UBICACION: Main Menu, en escena "0-Main Menu"
public class activateFade : MonoBehaviour {

	public Material matFalseBlur;
	bool isFading;

	void Start () {
		if (matFalseBlur != null) matFalseBlur.color = Color.clear;
	}

	public void fadeOn () {
		isFading = true;
		StartCoroutine (startFadeOn ());
		Invoke ("stopFade", 1);
	}

	public void fadeOff () {
		isFading = true;
		StartCoroutine (startFadeOff ());
		Invoke ("stopFade", 1);
	}

	IEnumerator startFadeOn () {
		while (isFading) {
			matFalseBlur.color = Color.Lerp (new Color (1, 1, 1, matFalseBlur.color.a), Color.white, 4f * Time.deltaTime);
			yield return new WaitForSeconds(0.01f);
		}
		StopCoroutine (startFadeOn ());
		yield return null;
	}

	IEnumerator startFadeOff () {
		while (isFading) {
			matFalseBlur.color = Color.Lerp (new Color (1, 1, 1, matFalseBlur.color.a), Color.clear, 4f * Time.deltaTime);
		}
		StopCoroutine (startFadeOff ());
		yield return null;
	}

	void stopFade () {
		isFading = false;
	}

}
