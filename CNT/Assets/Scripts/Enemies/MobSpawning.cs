using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MobSpawning : MonoBehaviour {

    AudioSource[] hitSounds;
    Queue<AudioSource> flyQueue = new Queue<AudioSource>();
    Queue<AudioSource> sklQueue = new Queue<AudioSource>();
    Queue<AudioSource> trsQueue = new Queue<AudioSource>();

    public static MobSpawning mobSp;

    public static int score;

    public static float globalTimer;
    public static float skellyTimer;
    
    public static int wave;
    public static int enemiesRem;

    public static int lor;

    System.Random r = new System.Random();

    public GameObject Target;
    public GameObject Skeleton;
    public GameObject Tank;
    public GameObject Camaro;

    public GameObject pUpCanvas;
    
    public Animator waveNumAnim;
    public Text waveNumText;

    [Header("Enemy Pooling")]
    public GameObject[] skeletonPool = new GameObject[20];
    public GameObject[] tankPool = new GameObject[20];
    public GameObject[] camaroPool = new GameObject[20];
    Queue<GameObject> skelQueue = new Queue<GameObject>();
    Queue<GameObject> tankQueue = new Queue<GameObject>();
    Queue<GameObject> camaQueue = new Queue<GameObject>();

    [Header("Combo splash Pool")]
    public GameObject[] comboPool = new GameObject[20];
    Queue<GameObject> comboQueue = new Queue<GameObject>();

    public GameObject GolemGO;

    void Start() {
        Debug.Log(DATA.instance.selectedCharacter);
        wave = 0;
        if (mobSp == null)
            mobSp = this;
        else if (mobSp != this)
            Destroy(gameObject);
        hitSounds = GetComponents<AudioSource>();
        {
            int count = 0;
            foreach (AudioSource a in hitSounds)
            {
                if (count < 5)
                    flyQueue.Enqueue(a);
                else if (count < 10)
                    sklQueue.Enqueue(a);
                else
                    trsQueue.Enqueue(a);
                count++;
            }
        }
        score = 0;
        globalTimer = 0;
        skellyTimer = 0;
        lor = 1;
        if (DATA.instance.level == 0)
            return;

        for (int i = 0; i < skeletonPool.Length; i++)
        {
            skeletonPool[i] = Instantiate(Skeleton);
            skeletonPool[i].name += i;
            skeletonPool[i].SetActive(false);
            skelQueue.Enqueue(skeletonPool[i]);
            tankPool[i] = Instantiate(Tank);
            tankPool[i].name += i;
            tankPool[i].SetActive(false);
            tankQueue.Enqueue(tankPool[i]);
            camaroPool[i] = Instantiate(Camaro);
            camaroPool[i].name += i;
            camaroPool[i].SetActive(false);
            camaQueue.Enqueue(camaroPool[i]);
        }
        StartCoroutine(SpawnEnemy());
        StartCoroutine(WaveStart());

        //creates Combo Splash Pool
        foreach (GameObject j in comboPool)
            comboQueue.Enqueue(j);
    }

    void Update ()
    {
        globalTimer += Time.deltaTime;
        skellyTimer -= Time.deltaTime;
	}

    IEnumerator SpawnEnemy()
    {

        Debug.Log("TriedToSpawn-Rem:" + enemiesRem);
        if (enemiesRem <= 0)
        {
            enemiesRem = 0;
        }
        if (enemiesRem > 0 && !(wave % 4 == 0))
        {
            enemiesRem--;
            int rndEnemy = UnityEngine.Random.Range(0, 3);
            InstantiateEnemy(rndEnemy);
            yield return new WaitForSeconds(UnityEngine.Random.Range(Mathf.Clamp(2 - (wave/2), 0, 10), 1f + Mathf.Clamp(3 - (wave/3), 0, 10)));
        }
        yield return new WaitForSeconds(2f);
        StartCoroutine(SpawnEnemy());
    }

    public void MakeSound(int sound)
    {
        AudioSource a;
        switch (sound)
        {
            case 1:
                a = flyQueue.Dequeue();
                a.Play();
                flyQueue.Enqueue(a);
                break;
            case 2:
                a = sklQueue.Dequeue();
                a.Play();
                sklQueue.Enqueue(a);
                break;
            case 3:
                a = trsQueue.Dequeue();
                a.Play();
                trsQueue.Enqueue(a);
                break;
        }
    }

    void InstantiateEnemy(int type)
    {
        lor = r.Next(1, 3);
        GameObject enemy = null;
        switch (type)
        {
            case 0:
                enemy = skelQueue.Dequeue();
                skelQueue.Enqueue(enemy);
                break;
            case 1:
                enemy = tankQueue.Dequeue();
                tankQueue.Enqueue(enemy);
                break;
            case 2:
                enemy = camaQueue.Dequeue();
                camaQueue.Enqueue(enemy);
                break;
        }
        enemy.SetActive(true);
        enemy.transform.SetParent(this.transform);
        PathMovemen pMovement = enemy.GetComponent<PathMovemen>();
        if (type == 2)
            pMovement.path = (lor == 1) ? "C" : "D";
        else
            pMovement.path = (lor == 1) ? "A" : "B";
        pMovement.StartEnemy();
    }

    public void StartWaveRemote()
    {
        StartCoroutine(WaveStart());
    }

    IEnumerator WaveStart()
    {
        wave++;
        waveNumText.text = wave.ToString();
		DATA.instance.score_waveTotal++;
        if (wave%4 == 0)
        {
            enemiesRem = 1;
            Instantiate(GolemGO, new Vector3(0, -100, 0), new Quaternion(0, 0, 0, 1));
        }
        else
        {
            enemiesRem = wave * 2 + 2;
        }
        yield return new WaitForSeconds(0.5f);
        waveNumAnim.SetBool("WaveNumberOn", true);
        yield return new WaitForSeconds(1);
        waveNumAnim.SetBool("WaveNumberOn", false);
        yield return new WaitForSeconds(2);
    }

    public void ComboSplashText(Vector3 hitPos, bool enemyHit)
    {
        GameObject go = comboQueue.Dequeue();
        go.SetActive(true);
        go.GetComponent<ComboTextLogic>().StartScore(hitPos, enemyHit);
        comboQueue.Enqueue(go);
    }
}