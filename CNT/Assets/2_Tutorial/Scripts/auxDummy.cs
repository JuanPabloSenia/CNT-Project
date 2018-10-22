using UnityEngine;

public class auxDummy : MonoBehaviour {

	public GameObject[] arrayParts;
	Quaternion rotBase;

	void Awake () {
		rotBase = GetComponentInChildren<Transform> ().rotation;
	}

	void OnEnable () {
		resetAux ();
	}

	void resetAux () {
		foreach (GameObject obj in arrayParts) {
			Rigidbody rBody = obj.GetComponent<Rigidbody> ();
			obj.transform.localPosition = rBody.velocity = rBody.angularVelocity = Vector3.zero;
			obj.transform.rotation = rotBase;
		}
	}


	//END
}
