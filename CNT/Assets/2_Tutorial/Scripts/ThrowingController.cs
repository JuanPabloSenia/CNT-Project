using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ThrowingController : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public static GraphicRaycaster interactable;
    public static ThrowingController INSTANCE;

    AudioSource[] screamSounds;
    Queue<AudioSource> screamQueue = new Queue<AudioSource>();

    public static int axeCount;

    // Vectores para calcular la fuerza del disparo
    public static Vector2 dragStart;
    public static Vector2 dragEnd;
    public static Vector2 dragDif;

    public static bool isShooting;
    public static bool aimRight;
    public static Vector2 screenSizePixels;

    public static bool isGameOver = false; // Para que se setee la anim de GameOver una sola vez... capaz hay que hacerlo de otra forma
    public static float currentHealth; // (¡Siempre modificar este valor para el daño!) currentHealth es para setear un target de HP de la torre, al recibir un golpe se modifica por el valor del daño y auxHealth se mueve lentamente hacia este valor
    public static float maxHealth;
    public static float dragPow;
    private float auxHealth; // Se mueve lentamente hacia el valor de vida actual de la torre, para el efecto de la barra de vida (se usa para detectar el Game Over)

    public static GameObject rightShootPos; // Referencia al empty desde el que se instancian hachas
    public static GameObject leftShootPos;

    [Header("GameOver Objects")]
    public Animator gameOverAnim;
    public Text finalScore;
    [Space(10)]

    [Header("HP")]
    public Image HPBar;
    [Space(10)]

    bool smithing;
    
    [Header("Weapon Pooling")]
    public GameObject[] weaponPool = new GameObject[100];
    public Queue<GameObject> weaponQueue = new Queue<GameObject>();
    public GameObject[] weaponTypes;

    public GameObject[] trashPool = new GameObject[2];
    public Queue<GameObject> trashQueue = new Queue<GameObject>();
    public GameObject[] trashTypes;

    void Start()
    {
        //Singleton
        if (INSTANCE == null)
            INSTANCE = this;

        //Seteo de variables al empezar un nivel
        MobSpawning.score = 0;
        maxHealth = 99;
        auxHealth = maxHealth;
        currentHealth = maxHealth;
        isGameOver = false;
        screenSizePixels.x = Screen.width;
        screenSizePixels.y = Screen.height;
        interactable = GetComponent<GraphicRaycaster>();
        isShooting = false;

        // Setea la cantidad de armas, si es el tutorial, da pocas
        if (DATA.instance.level == 2)
            axeCount = 3;
        else
            axeCount = 20;

        //CameraScript.ActualizeAxeCount();


        screamSounds = GetComponents<AudioSource>();
        foreach (AudioSource a in screamSounds)
        {
            screamQueue.Enqueue(a);
        }

        //Busca los empty GameObjects para disparar
        leftShootPos = GameObject.Find("LeftShootPos");
        rightShootPos = GameObject.Find("RightShootPos");
        aimRight = true;
        
        // Instancia la pool de Armas
        for (int i = 0; i < weaponPool.Length; i++)
        {
            weaponPool[i] = Instantiate(weaponTypes[Random.Range(0, weaponTypes.Length)]);
            weaponPool[i].SetActive(false);
            weaponQueue.Enqueue(weaponPool[i]);
        }

        // Instancia la pool de Armas Basura
        for (int i = 0; i < trashPool.Length; i++)
        {
            trashPool[i] = Instantiate(trashTypes[Random.Range(0, trashTypes.Length)]);
            trashPool[i].SetActive(false);
            trashQueue.Enqueue(trashPool[i]);
        }
    }

    void Update()
    {
        //Actualización de HP y barra de HP
        {
            if (currentHealth > maxHealth) currentHealth = maxHealth;
            if (auxHealth > currentHealth)
            {
                auxHealth -= 30 * Time.deltaTime;
                if (auxHealth < currentHealth) auxHealth = currentHealth;
                ActualizeHealth();
            }
            else if (auxHealth < currentHealth)
            {
                auxHealth += 30 * Time.deltaTime;
                if (auxHealth > currentHealth) auxHealth = currentHealth;
                ActualizeHealth();
            }
            if (auxHealth <= 0)
            {
                currentHealth = 0;
                if (!isGameOver) GameOver();
            }
        }
    }

    public void GameOver()
    {
        finalScore.text = MobSpawning.score.ToString();
        isGameOver = true;
        gameOverAnim.SetBool("isGameOver", true);
        Invoke("PauseOn", 1.3f);
    }

    public void PauseOn()
    {
        Time.timeScale = (Time.timeScale==2) ? 1f : 0f;
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        Vector2 dragTemp = RelativePedPosition(ped.position);
        dragDif = dragStart - dragTemp;
        if (dragDif.y > 0) dragDif.y *= 1.25f;
        dragDif.x *= 1.15f*1.5f;
        if (dragDif.x > 0) dragDif.x *= 1.1f;
        if (DATA.instance.level == 2) dragDif *= 0.4f;
        if (dragDif.x > 0) dragDif.x += Mathf.Pow(dragDif.x / 70, 2);
        AimHelper.shootPos = (dragDif.x > 0) ? rightShootPos.transform.position : leftShootPos.transform.position;
        if (!smithing) AimHelper.isPressed = true;
        if (aimRight && dragDif.x < -120 && !smithing)
        {
            aimRight = !aimRight;
        }
        if (!aimRight && dragDif.x > 120 && !smithing)
        {
            aimRight = !aimRight;
        }
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        smithing = Camera.main.GetComponent<CameraScript>().CastRay(ped.position);
        if (isShooting == false && !smithing)
        {
            isShooting = true;
            dragStart = RelativePedPosition(ped.position);
        }
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        rightShootPos.GetComponent<AimHelper>().setDotsInvisible();
        dragEnd = RelativePedPosition(ped.position);
        dragPow = Vector2.Distance(dragStart, dragEnd);
        dragDif = dragStart - dragEnd;
        dragDif.x *= 1.5f;
        if (dragDif.x > 0) dragDif.x *= 1.28f;
        if (DATA.instance.level == 2) dragDif *= 0.4f;
        isShooting = false;
        //from here :P
        if (dragDif.magnitude > 75 && !smithing)
        {
            bool normalWeapon = axeCount > 0;
            if (dragDif.x > 0 || DATA.instance.level != 2)
                if (normalWeapon)
                    axeCount--;
            if (normalWeapon && DATA.instance.level == 2 && PathMovemen.dummyCount > 1) GameObject.Find("Cartel_Recargar").GetComponent<Animator>().SetInteger("AxesLeft", axeCount);
            GameObject weapon = null;
            if (dragDif.x > 0)
            {
                weapon = InstantiateWeapon(rightShootPos.transform.position, new Quaternion(), normalWeapon, true);
            }
            else if (DATA.instance.level != 2)
            {
                weapon = InstantiateWeapon(leftShootPos.transform.position, leftShootPos.transform.rotation, normalWeapon, false);
            }
            //Audio Things
            if (dragDif.x > 0 || DATA.instance.level != 2)
            {
                AudioSource aud = screamQueue.Dequeue();
                aud.Play();
                screamQueue.Enqueue(aud);
            }
        }
        rightShootPos.GetComponent<AimHelper>().setDotsInvisible();
        //CameraScript.ActualizeAxeCount();
    }

    public static Vector2 RelativePedPosition(Vector2 pedOriginal) //Devuelve un valor igual sin importar el tamaño de la pantalla en pixeles
    {
        Vector2 relativePos = new Vector2(pedOriginal.x / screenSizePixels.x * 1900, pedOriginal.y / screenSizePixels.x * 1900);
        return relativePos;
    }

    public GameObject InstantiateWeapon(Vector3 _position, Quaternion _rotation, bool weap, bool left) //Toma un arma de la pool de objetos correspondiente y la spawnea
    {
        GameObject weapon;
        if (weap)
            weapon = weaponQueue.Dequeue();
        else
            weapon = trashQueue.Dequeue();
        weapon.SetActive(true);
        weapon.transform.position = _position;
        weapon.GetComponent<Weapon> ().axeDirection = dragDif;
        //weapon.GetComponent<AxeSpawning>().axeTimer = 0;
        //weapon.GetComponent<AxeSpawning>().axeDestroy = false;
        weapon.GetComponent<Weapon> ().StartWeapon(left);
        if (weap)
            weaponQueue.Enqueue(weapon);
        else
            trashQueue.Enqueue(weapon);
        return weapon;
    }

    public void ActualizeHealth() //Actualiza la barra de vida y la limita a la vida máxima
    {
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
        HPBar.fillAmount = auxHealth / maxHealth;
    }

    public void PUpSelectionMode(bool aux) //Enciende y apaga las utilidades de este script para el modo de seleccion de power up (se llama desde PowerUpController)
    {
        if (aux)
        {
            interactable.enabled = false;
            PowerUpsCanvasController.INSTANCE.PUPCloseBtn.SetActive(true);
            PowerUpsCanvasController.INSTANCE.PUPButtons.SetActive(true);
            PowerUpsCanvasController.INSTANCE.openPUPImage.enabled = false;
            PowerUpsCanvasController.INSTANCE.activePowerUp = PowerUpsCanvasController.powerUp.SELECTING;
        }
        else
        {
            interactable.enabled = true;
            PowerUpsCanvasController.INSTANCE.openPUPImage.enabled = true;
            PowerUpsCanvasController.INSTANCE.PUPButtons.SetActive(false);
        }
    }
}
