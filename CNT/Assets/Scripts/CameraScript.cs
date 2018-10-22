using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;
using UnityEngine.SceneManagement;

public class CameraScript : MonoBehaviour {

    AudioSource[] smithSounds;
    Queue<AudioSource> smithQueue = new Queue<AudioSource>();
    AudioSource bridgeAud;

    bool isOnTutorial;

    public ParticleSystem pSys;
    public Image axeHint;
    public Text axeHintText;

    public int thisLevelIndex;

    public Text bridgeText;
    public GameObject bridgeCollider;
    public Animator bridgeAnimator;
    bool bridgeReady = true;

    Vector3 originalCameraPos;

    private void Awake()
    {
        isOnTutorial = SceneManager.GetActiveScene().buildIndex == 2;
		DATA.instance.level = thisLevelIndex;
    }

    private void Start()
    {
        originalCameraPos = transform.parent.position;
        smithSounds = GetComponents<AudioSource>();
        foreach (AudioSource a in smithSounds)
            smithQueue.Enqueue(a);
        AudioSource aud = smithQueue.Dequeue();
        bridgeAud = smithQueue.Dequeue();
        //axeHint = GameObject.Find("imgSinMunicion").GetComponent<Image>();
        //axeHintText = GameObject.Find("txtSinMunicion").GetComponent<Text>();
    }

    private void Update()
    {
		Vector3 auxVector = new Vector3 (originalCameraPos.x + GameManager.dragDif.x / 1400, originalCameraPos.y + GameManager.dragDif.y / 1250, originalCameraPos.z);
		if (!isOnTutorial) transform.parent.position = Vector3.Lerp(transform.parent.position, auxVector, Time.deltaTime/2);
    }

    public bool CastRay (Vector2 pos) {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(pos);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.name == "Forja" && GameManager.axeCount < 20)
            {
                if (DATA.instance.level == 2) GameObject.Find("Cartel_Recargar").GetComponent<Animator>().SetInteger("AxesLeft", GameManager.axeCount);
                AudioSource aud = smithQueue.Dequeue();
                aud.Play();
                smithQueue.Enqueue(aud);
                StartCoroutine(PSysControl());
				GameManager.axeCount++;
                if (DATA.instance.selectedCharacter == DATA.Characters.BLACKSMITH) GameManager.axeCount++;
                ActualizeAxeCount();
                return true;
            }
            else
                return false;
        }
        else
            return false;
    }

    IEnumerator PSysControl()
    {
        if (DATA.instance.level == 2)
        {
            pSys.emissionRate += 10;
            yield return new WaitForSeconds(0.4f);
            pSys.emissionRate -= 10;
        }
        else
        {
            pSys.emissionRate += 22;
            yield return new WaitForSeconds(0.4f);
            pSys.emissionRate -= 22;
        }
    }

    public void ActualizeAxeCount()
    {
        bool aux = false;
        if (GameManager.axeCount == 0) aux = true; else aux = false;
        axeHint.enabled = aux;
        axeHintText.enabled = aux;
        GameObject.Find("txtAmmo").GetComponent<Text>().text = GameManager.axeCount.ToString();
    }

    public void StartLvlVoid(int lvlIndex, float delay)
    {
        StartCoroutine(StartLvlWithDelay(lvlIndex, delay));
    }

    public static IEnumerator StartLvlWithDelay(int lvlIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(lvlIndex);
    }
}