using UnityEngine;

public class Dummy : MonoBehaviour {

	int amountDummy;
	public GameObject destroyedDummy;
	public Animator animRecharge;

	void Start () {
		destroyedDummy.SetActive (false);
		amountDummy = DATA.instance.amount_dummys;
	}
	
	public void destroyDummy () {
		spawnAuxDummy ();
		if (amountDummy == 0) animRecharge.SetInteger ("AxesLeft", 1);
		if (amountDummy > 0) {
			TimeLineManager.PlayTutorial (1);
			amountDummy--;
		}
		else {
			TimeLineManager.PlayTutorial (2);
			gameObject.SetActive (false);
			Camera.main.GetComponent<CameraScript> ().StartLvlVoid (1, 12f);
		}
		gameObject.SetActive (false);
	}

	void OnCollisionEnter (Collision other) {
		if (other.gameObject.tag == "Axes") {
			destroyDummy ();
		}
	}

	void spawnAuxDummy () {
		destroyedDummy.transform.position = transform.position;
		destroyedDummy.transform.rotation = transform.rotation;
		destroyedDummy.SetActive (true);
		Invoke ("destroyAuxDummy", 2.2f);
	}

	void destroyAuxDummy () {
		destroyedDummy.SetActive (false);
	}



	//END
}
