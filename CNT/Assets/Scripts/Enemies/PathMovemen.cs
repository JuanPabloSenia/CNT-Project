using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EZCameraShake;

public class PathMovemen : MonoBehaviour {

    public static int enemiesAlive = 0;

    public static int dummyCount = 4;

    public Text waveNum;
 
    GameObject activeWay;
    Vector3 lookTarget;
    public Transform HpBar;

    Vector3 auxPos;
    float randomOffset;
    public EnemyTypes enemyType;

    int wayNum = 1;
    public float speed;
    public float hp;
    public float maxHP;
    public string path;

    public Rigidbody rBody;
    public Image HPBarImage;
    public Image HPBarImage2;
    public Animator anim;
    public GameObject freezeEffect;

    public GameObject destroyedDummy;

    Queue<GameObject> axes = new Queue<GameObject>();
    Transform startPos;

    bool falling;

    public enum EnemyTypes
    {
        DUMMY,
        SKELETON,
        CAMARO
    }

    void Awake()
    {
        if (enemyType == EnemyTypes.DUMMY)
        {
            maxHP = 1;
        }
        else
            waveNum = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
        if (enemyType == EnemyTypes.SKELETON)
        {
            speed = 2;
            maxHP = (Random.Range(1, 7) > 4.5f)? 1 : 2;
        }
        if (enemyType == EnemyTypes.CAMARO)
        {
            speed = 3;
            maxHP = 1;
        }
        hp = maxHP;
        enemiesAlive = 0;
        auxPos = transform.position;
        if (enemyType == EnemyTypes.DUMMY) StartEnemy();
    }

    IEnumerator SetFloat(float delay, float value)
    {
        yield return new WaitForSeconds(delay);
        if (anim.GetFloat("SpeedMult") != 0)speed = value;
    }

    public void StartEnemy()
    {
        if (anim != null) anim.SetFloat("SpeedMult", 1f);
        if (enemyType == EnemyTypes.SKELETON)
        {
            freezeEffect.SetActive(false);
            speed = 2.05f;
            maxHP = Random.Range(1, 7);
            maxHP = (maxHP < 4.5f) ? 1 : 2;
        }
        if (enemyType == EnemyTypes.CAMARO)
        {
            speed = 3;
            maxHP = 1;
        }
        hp = maxHP;
        if (anim != null) anim.SetFloat("HP", hp);
        if (enemyType != EnemyTypes.DUMMY)
            enemiesAlive++;
        falling = false;
        //HPBarImage.fillAmount = HPBarImage2.fillAmount = hp / maxHP;
        wayNum = 1;
        randomOffset = Random.Range(0f, 20f);
        if (enemyType != EnemyTypes.DUMMY)
        {
            startPos = GameObject.Find(path + "0").transform;
            transform.position = startPos.position;
            activeWay = GameObject.Find(path + "1");
            lookTarget = GameObject.Find(path + "1").transform.position;
            transform.LookAt(lookTarget);
        }
        if (rBody != null) rBody.isKinematic = true;
        auxPos = transform.position;
	}

    void Update()
    {
        if (falling)
        {
            transform.localScale -= new Vector3(0.7f, 0.7f, 0.7f) * Time.deltaTime;
        }
        if (rBody != null && rBody.velocity.y < 0)
            rBody.velocity = new Vector3(rBody.velocity.x, rBody.velocity.y * 1.1f, rBody.velocity.z);
        //Rotates HPBar
        if (enemyType != EnemyTypes.DUMMY) HpBar.LookAt(Camera.main.transform.position);
        if (hp <= 0)
        {
            KillEnemy(true);
        }
        if (auxPos != activeWay.transform.position)
        {
            if (Vector3.Distance(auxPos, lookTarget) < 3.5f) lookTarget = GameObject.Find(path + (wayNum + 1)).transform.position;
                auxPos = Vector3.MoveTowards(auxPos, activeWay.transform.position, speed * Time.deltaTime);
        }
        else
        {
            if ((path == "B" && wayNum == 9) || (path == "A" && wayNum == 4) || (path == "C" && wayNum == 4) || (path == "D" && wayNum == 5))
            {
                KillEnemy(false);
            }
            wayNum++;
            activeWay = GameObject.Find(path + wayNum);
        }
        if (!falling)
        {
            transform.position = auxPos;
            Vector3 targetDir = lookTarget - transform.position;
            float step = (enemyType == EnemyTypes.CAMARO) ? 0.7f * Time.deltaTime : 0.5f * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
            transform.rotation = Quaternion.LookRotation(newDir);
            if (enemyType == EnemyTypes.CAMARO)
            {
                transform.position += new Vector3(0, Mathf.Sin((Time.time * 4 + randomOffset) * speed / 3) * 0.5f + (randomOffset - 9) * 0.35f, 0);
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Weapon>().canDealDmg == true)
        {
            if (enemyType == EnemyTypes.SKELETON && hp > 1.1f)
            {
				//float auxHP = hp - other.transform.GetComponent<AxeSpawning>().axeDmg;
				float auxHP = hp - DATA.instance.dmg_axe;
				if (auxHP < 1.1f)
                {
                    speed = 0;
                    StartCoroutine(SetFloat(1.25f, 2.15f));
                }
            }
            if (other.transform.tag == "Trash")
                MobSpawning.mobSp.MakeSound(3);
            else if (enemyType == EnemyTypes.CAMARO)
                MobSpawning.mobSp.MakeSound(1);
            else
                MobSpawning.mobSp.MakeSound(2);
            //other.gameObject.GetComponent<AxeSpawning>().axeDestroy = true;
            other.gameObject.GetComponent<Weapon>().canDealDmg = false;
			//other.gameObject.GetComponent<AxeSpawning> ().canDealDmg = false;
			if (Random.Range(1, 15) == 1 && DATA.instance.selectedCharacter == DATA.Characters.PALADIN)
            {
                //ThrowingController.currentHealth += ThrowingController.maxHealth / 25;
                //ThrowingController.INSTANCE.ActualizeHealth();
				GameManager.currentHealth += GameManager.maxHealth / 25;
				GameManager.instance.ActualizeHealth ();
			}
            //hp -= other.transform.GetComponent<AxeSpawning>().axeDmg;
			hp -= DATA.instance.dmg_axe;
			//HPBarImage.fillAmount = HPBarImage2.fillAmount = hp / maxHP;
			other.gameObject.GetComponent<Weapon>().canDealDmg = false;
			//other.gameObject.GetComponent<AxeSpawning> ().canDealDmg = false;
			if (other.collider.tag != "Trash")
                MobSpawning.mobSp.ComboSplashText(other.transform.position, true);
            if (anim != null)anim.SetFloat("HP", hp);
        }

        {/*if (other.collider == other.transform.GetComponent<AxeSpawning>().Stick && other.gameObject.GetComponent<AxeSpawning>().canDealDmg == true)
        {
            if (other.transform.tag == "Trash")
                MobSpawning.mobSp.MakeSound(3);
            else if (enemyType == "Camaro")
                MobSpawning.mobSp.MakeSound(1);
            else
                MobSpawning.mobSp.MakeSound(2);
            other.gameObject.GetComponent<AxeSpawning>().axeDestroy = true;
            other.gameObject.GetComponent<AxeSpawning>().canDealDmg = false;
            hp -= other.transform.GetComponent<AxeSpawning>().axeDmg;
            HPBarImage.fillAmount = HPBarImage2.fillAmount = hp / maxHP;
            if (other.collider.tag != "Trash")
            MobSpawning.mobSp.ComboSplashText(other.transform.position);
        }
        if (other.collider == other.transform.GetComponent<AxeSpawning>().Edge && other.gameObject.GetComponentInParent<AxeSpawning>().canDealDmg == true)
        {
            if (enemyType == "Camaro")
                MobSpawning.mobSp.MakeSound(1);
            else
                MobSpawning.mobSp.MakeSound(2);
            other.transform.GetComponent<AxeSpawning>().Stick.enabled = false;
            other.transform.GetComponent<AxeSpawning>().Edge.enabled = false;
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();
            rb.drag = 100;
            rb.angularDrag = 100;
            rb.velocity = rb.angularVelocity = Vector3.zero;
            rb.useGravity = false;
            other.gameObject.transform.SetParent(transform);
            axes.Enqueue(other.gameObject);
            other.gameObject.GetComponent<AxeSpawning>().canDealDmg = false;
            other.gameObject.GetComponent<AxeSpawning>().axeDestroy = true;
            hp -= other.transform.GetComponent<AxeSpawning>().axeDmg * 1.7f;
            HPBarImage.fillAmount = hp / maxHP;
            HPBarImage2.fillAmount = hp / maxHP;
            MobSpawning.mobSp.ComboSplashText(other.transform.position);
        }*/
        }//Old Damage Detection
    }

    public void KillEnemy(bool killed)
    {
        if (enemyType == EnemyTypes.DUMMY)
        {
            GameObject aux = Instantiate(destroyedDummy, transform.position, transform.rotation);
            Destroy(aux, 2.2f);
            if (dummyCount == 0) GameObject.Find("RecargarHint").GetComponent<Animator>().SetInteger("AxesLeft", 1);
            if (dummyCount > 0)
            {
                TimeLineManager.PlayTutorial(1);
                dummyCount--;
            }
            /*else if (dummyCount == 1)
            {
                TimeLineManager.PlayTutorial(1);
                MobSpawning.mobSp.pUpCanvas.SetActive(true);
                dummyCount--;
                GameObject.Find("PowerUpCanvas").GetComponent<GraphicRaycaster>().enabled = true;
                GameObject.Find("GameCanvas").GetComponent<GraphicRaycaster>().enabled = false;
            }*/
            else
            {
                TimeLineManager.PlayTutorial(2);
                gameObject.SetActive(false);
                Camera.main.GetComponent<CameraScript>().StartLvlVoid(1, 12f);
            }
            hp = maxHP;
            //HPBarImage.fillAmount = HPBarImage2.fillAmount = hp / maxHP;
            return;
        }
        enemiesAlive--;
        if (enemiesAlive <= 0 && MobSpawning.enemiesRem <= 0)
            MobSpawning.mobSp.StartWaveRemote();
        //Debug.Log(enemiesAlive + "alive - " + MobSpawning.enemiesRem + "remaining");
        if (killed == false)
        {
            CameraShaker.Instance.ShakeOnce(2f, 5f, .1f, .5f);
			//ThrowingController.currentHealth -= 20;
			//GameObject.Find("GameCanvas").GetComponent<ThrowingController>().ActualizeHealth();
			GameManager.currentHealth -= 20;
			GameObject.Find ("GameCanvas").GetComponent<GameManager> ().ActualizeHealth ();
		}
        else
        {
            int pointsEarned = 250 + (int)ComboTextLogic.combo * 10;
            MobSpawning.score += pointsEarned;
            PowerUpsCanvasController.INSTANCE.ActualizeMana(pointsEarned);
            waveNum.text = MobSpawning.score.ToString();
			DATA.instance.score_enemiesKilledTotal++;
        }
        gameObject.SetActive(false);
    }

    IEnumerator KillEnemyDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        KillEnemy(true);
    }

    public void FreezeEnemy()
    {
        switch (enemyType)
        {
            case EnemyTypes.SKELETON:
                freezeEffect.SetActive(true);
                freezeEffect.transform.Rotate(0, 0, Random.Range(0, 360));
                anim.SetFloat("SpeedMult", 0);
                speed = 0;
                break;
            case EnemyTypes.CAMARO:
                speed = 1.5f;
                anim.SetFloat("SpeedMult", .5f);
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        switch(other.name)
        {
            case "ArrowStormCollider":
                hp -= 1;
                break;
            case "FreezeCollider":
                FreezeEnemy();
                break;
        }
    }
}