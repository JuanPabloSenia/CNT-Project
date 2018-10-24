using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

	public Rigidbody rBody;

	public Vector3 rotOffset; //Rotacion inicial rara FIX
	public bool canDealDmg; //Deja de hacer dano al tocar ALGO
	float timerDestroy = 1; //Para la autodestruccion del Arma
	public Vector2 axeDirection; //Fuerza del disparo

	public void StartWeapon (bool left) {
		//Reset Rigidbody
		rBody.useGravity = true;
		rBody.drag = 0;
		rBody.angularDrag = 0.5f;
		rBody.velocity = Vector3.zero;
		rBody.angularVelocity = Vector3.zero;
		canDealDmg = true;

		//Drag del Arma
		axeDirection.y += 50; //FIX para linea de puntitos
		if (left) {
			transform.rotation = GameManager.rightShootPos.transform.rotation;
			transform.Rotate (rotOffset);
			rBody.AddForce (axeDirection);
			rBody.angularVelocity = new Vector3 (0, 0, -axeDirection.x / 7.3f);
		} else {
			transform.rotation = GameManager.rightShootPos.transform.rotation;
			transform.Rotate (rotOffset);
			rBody.AddForce (new Vector3 (axeDirection.x, axeDirection.y, -axeDirection.x));
			rBody.angularVelocity = new Vector3 (-axeDirection.x / 4, 0, -axeDirection.x / 4);
		}
	}

	void autoDestroy () {
		rBody.drag = 0;
		rBody.angularDrag = 0.5f;
		rBody.velocity = Vector3.zero;
		rBody.angularVelocity = Vector3.zero;
		gameObject.SetActive (false);
	}

	void OnCollisionEnter (Collision other) {
		if (other.gameObject.tag == "Environment" && canDealDmg) {
			if (canDealDmg && tag != "Trash") {
				Debug.Log ("ComboReset");
				ComboTextLogic.combo = 0;
				//MobSpawning.mobSp.ComboSplashText (transform.position, false); //TRUE Combo, FLASE Miss
			}
			canDealDmg = false;
			Invoke ("autoDestroy", timerDestroy);
		}
	}


	//END
}
