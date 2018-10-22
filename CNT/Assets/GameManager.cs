using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {

	private static GameManager _instance;
	public static GameManager instance {
		get {
			if (_instance == null) {
				_instance = new GameManager ();
			}
			return _instance;
		}
	}


	AudioSource[] screamSounds;
	Queue<AudioSource> screamQueue = new Queue<AudioSource> ();

	public static bool isShooting;
	public static bool aimRight;
	public static Vector2 screenSizePixels;

	public bool isGameOver = false; // Para que se setee la anim de GameOver una sola vez... capaz hay que hacerlo de otra forma
	public static float currentHealth; // (¡Siempre modificar este valor para el daño!) currentHealth es para setear un target de HP de la torre, al recibir un golpe se modifica por el valor del daño y auxHealth se mueve lentamente hacia este valor
	public static float maxHealth;
	public static float dragPow;
	private float auxHealth; // Se mueve lentamente hacia el valor de vida actual de la torre, para el efecto de la barra de vida (se usa para detectar el Game Over)

	[Header ("GameOver Objects")]
	public Animator gameOverAnim;
	public Text finalScore;

	bool smithing;

	//-----------------------------------------------------------

	[Header ("HP")]
	public Image HPBar;

	[Header("Shoot Positions")] // Referencia al empty desde el que se instancian hachas
	public static GameObject leftShootPos;
	public static GameObject rightShootPos;

	[Header ("Object Pool")]
	public GameObject[] pool_weapon;
	[HideInInspector]public GameObject[] arrayWeapon;
	public int amountPoolWeapon;
	[Space]
	public GameObject[] pool_trash;
	[HideInInspector]public GameObject[] arrayTrash;
	public int amountPoolTrash;

	[Header ("OTHERS")]
	public GraphicRaycaster interactable;
	public CameraScript scriptCameraScript;

	public static int axeCount;
	//[HideInInspector]
	public static Vector2 dragStart, dragEnd, dragDif; //Vectores para calcular la fuerza del disparo




	void Awake () {
		arrayWeapon = new GameObject[amountPoolWeapon];
		arrayTrash = new GameObject[amountPoolTrash];
		leftShootPos = GameObject.Find ("LeftShootPos");
		rightShootPos = GameObject.Find ("RightShootPos");
	}

	void Start () {
		createPool (pool_weapon[Random.Range(0, pool_weapon.Length)], arrayWeapon, amountPoolWeapon);
		createPool (pool_trash[Random.Range (0, pool_trash.Length)], arrayTrash, amountPoolTrash);

		//Seteo de variables al empezar un nivel
		MobSpawning.score = 0;
		maxHealth = 99;
		auxHealth = maxHealth;
		currentHealth = maxHealth;
		isGameOver = false;
		screenSizePixels.x = Screen.width;
		screenSizePixels.y = Screen.height;

		isShooting = false;

		// Setea la cantidad de armas, si es el tutorial, da pocas
		if (DATA.instance.level == 2) {
			axeCount = 3;
		} else {
			axeCount = 20;
		}

		scriptCameraScript.ActualizeAxeCount ();

		screamSounds = GetComponents<AudioSource> ();
		foreach (AudioSource a in screamSounds) {
			screamQueue.Enqueue (a);
		}

		aimRight = true;
	}

	void Update () {
		//Actualización de HP y barra de HP
		{
			if (currentHealth > maxHealth) currentHealth = maxHealth;
			if (auxHealth > currentHealth) {
				auxHealth -= 30 * Time.deltaTime;
				if (auxHealth < currentHealth) auxHealth = currentHealth;
				ActualizeHealth ();
			} else if (auxHealth < currentHealth) {
				auxHealth += 30 * Time.deltaTime;
				if (auxHealth > currentHealth) auxHealth = currentHealth;
				ActualizeHealth ();
			}
			if (auxHealth <= 0) {
				currentHealth = 0;
				if (!isGameOver) GameOver ();
			}
		}
	}

	public void GameOver () {
		finalScore.text = MobSpawning.score.ToString ();
		isGameOver = true;
		gameOverAnim.SetBool ("isGameOver", true);
		Invoke ("PauseOn", 1.3f);
	}

	public void PauseOn () {
		Time.timeScale = (Time.timeScale == 2) ? 1f : 0f;
	}

	public virtual void OnDrag (PointerEventData ped) {
		Vector2 dragTemp = RelativePedPosition (ped.position);
		dragDif = dragStart - dragTemp;
		if (dragDif.y > 0) dragDif.y *= 1.25f;
		dragDif.x *= 1.15f * 1.5f;
		if (dragDif.x > 0) dragDif.x *= 1.1f;
		if (DATA.instance.level == 2) dragDif *= 0.4f;
		if (dragDif.x > 0) dragDif.x += Mathf.Pow (dragDif.x / 70, 2);
		AimHelper.shootPos = (dragDif.x > 0) ? rightShootPos.transform.position : leftShootPos.transform.position;
		if (!smithing) AimHelper.isPressed = true;
		if (aimRight && dragDif.x < -120 && !smithing) {
			aimRight = !aimRight;
		}
		if (!aimRight && dragDif.x > 120 && !smithing) {
			aimRight = !aimRight;
		}
	}

	public virtual void OnPointerDown (PointerEventData ped) {
		smithing = Camera.main.GetComponent<CameraScript> ().CastRay (ped.position);
		if (isShooting == false && !smithing) {
			isShooting = true;
			dragStart = RelativePedPosition (ped.position);
		}
	}

	public virtual void OnPointerUp (PointerEventData ped) {
		rightShootPos.GetComponent<AimHelper> ().setDotsInvisible ();
		dragEnd = RelativePedPosition (ped.position);
		dragPow = Vector2.Distance (dragStart, dragEnd);
		dragDif = dragStart - dragEnd;
		dragDif.x *= 1.5f;
		if (dragDif.x > 0) dragDif.x *= 1.28f;
		if (DATA.instance.level == 2) dragDif *= 0.4f;
		isShooting = false;
		//from here :P
		if (dragDif.magnitude > 75 && !smithing) {
			bool normalWeapon = axeCount > 0; //Usa la basura al quedarse sin armas
			if (dragDif.x > 0 || DATA.instance.level != 2)
				if (normalWeapon)
					axeCount--;
			if (normalWeapon && DATA.instance.level == 2 && PathMovemen.dummyCount > 1) GameObject.Find ("Cartel_Recargar").GetComponent<Animator> ().SetInteger ("AxesLeft", axeCount);
			if (dragDif.x > 0) {
				if (normalWeapon) {
					activatePool (arrayWeapon, rightShootPos.transform.position, new Quaternion (), true);
				} else {
					activatePool (arrayTrash, rightShootPos.transform.position, new Quaternion (), true);
				}
			} else if (DATA.instance.level != 2) {
				if (normalWeapon) {
					activatePool (arrayWeapon, leftShootPos.transform.position, leftShootPos.transform.rotation, false);
				} else {
					activatePool (arrayTrash, leftShootPos.transform.position, leftShootPos.transform.rotation, false);
				}
			}
			//Audio Things
			if (dragDif.x > 0 || DATA.instance.level != 2) {
				AudioSource aud = screamQueue.Dequeue ();
				aud.Play ();
				screamQueue.Enqueue (aud);
			}
		}
		rightShootPos.GetComponent<AimHelper> ().setDotsInvisible ();
		scriptCameraScript.ActualizeAxeCount ();
	}

	public static Vector2 RelativePedPosition (Vector2 pedOriginal) //Devuelve un valor igual sin importar el tamaño de la pantalla en pixeles
	{
		Vector2 relativePos = new Vector2 (pedOriginal.x / screenSizePixels.x * 1900, pedOriginal.y / screenSizePixels.x * 1900);
		return relativePos;
	}

	public void ActualizeHealth () //Actualiza la barra de vida y la limita a la vida máxima
	{
		if (currentHealth > maxHealth)
			currentHealth = maxHealth;
		HPBar.fillAmount = auxHealth / maxHealth;
	}

	public void PUpSelectionMode (bool aux) //Enciende y apaga las utilidades de este script para el modo de seleccion de power up (se llama desde PowerUpController)
	{
		if (aux) {
			interactable.enabled = false;
			PowerUpsCanvasController.INSTANCE.PUPCloseBtn.SetActive (true);
			PowerUpsCanvasController.INSTANCE.PUPButtons.SetActive (true);
			PowerUpsCanvasController.INSTANCE.openPUPImage.enabled = false;
			PowerUpsCanvasController.INSTANCE.activePowerUp = PowerUpsCanvasController.powerUp.SELECTING;
		} else {
			interactable.enabled = true;
			PowerUpsCanvasController.INSTANCE.openPUPImage.enabled = true;
			PowerUpsCanvasController.INSTANCE.PUPButtons.SetActive (false);
		}
	}


	void createPool (GameObject objectPool, GameObject[] arrayPool, int amountPool) {
		for (int i = 0; i < amountPool; i++) {
			GameObject obj = Instantiate (objectPool) as GameObject;
			obj.SetActive (false);
			arrayPool[i] = obj;
		}
	}

	public void activatePool (GameObject[] arrayPool, Vector3 _position, Quaternion _rotation, bool isLeft) {
		for (int i = 0; i < arrayPool.Length; i++) {
			if (!arrayPool[i].activeInHierarchy) {
				arrayPool[i].SetActive (true);
				Weapon w = arrayPool[i].GetComponent<Weapon> ();
				arrayPool[i].transform.position = _position;
				arrayPool[i].transform.rotation = _rotation;
				w.axeDirection = dragDif;
				w.StartWeapon (isLeft);
				break;
			}
		}
	}








	//END
}
