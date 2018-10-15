using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour {

	AsyncOperation ao;
	int scene;
	public Text txtLoading;

	void Start () {
		txtLoading.text = "0%";
		StartCoroutine (loadScene (DATA.instance.level + 2));
	}

	IEnumerator loadScene (int scene) {
		ao = SceneManager.LoadSceneAsync (scene);
		while (!ao.isDone) {
			float _progress = Mathf.Clamp01 (ao.progress / 0.9f);
			txtLoading.text = (_progress * 100f).ToString ("F0") + "%";
			yield return null;
		}
	}

}
