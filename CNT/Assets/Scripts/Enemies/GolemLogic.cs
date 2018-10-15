using UnityEngine;

public class GolemLogic : MonoBehaviour {

    public bool headBroken = false;
    public GameObject head;

    GameObject activeWay;
    string side;
    int wayNum = 0;

    float hp = 23;

    public Transform headStartPos;

	void Start () {
        side = (Random.Range(0, 2) == 0) ? "A":"B";
        transform.position = GameObject.Find(side + "0").transform.position;
        transform.LookAt(GameObject.Find(side + "1").transform.position);
        wayNum++;
        activeWay = GameObject.Find(side + wayNum);
    }

    void Update()
    {
        if(Vector3.Distance(GameObject.Find("HeadTarget").transform.position, transform.position) > 18)
        {
            if (transform.position != activeWay.transform.position + transform.right * 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, activeWay.transform.position + transform.right * 0.5f, 5* Time.deltaTime);
            }
            else
            {
                wayNum++;
                activeWay = GameObject.Find(side + wayNum);
            }
        }
        else
        {
            GetComponent<Animator>().SetBool("onShootingRange", true);
        }
        if (hp <= 0)
        {
            MobSpawning.enemiesRem--;
            MobSpawning.mobSp.StartWaveRemote();
            Destroy(gameObject);
        }
    }

    public void SpawnHead()
    {
        if (!headBroken)
        {
            Instantiate(head, headStartPos.position, headStartPos.rotation);
            GameObject target = GameObject.Find("HeadTarget");
        }
        headBroken = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<AxeSpawning>().canDealDmg == true)
        {
            collision.gameObject.GetComponent<AxeSpawning>().axeDestroy = true;
            collision.gameObject.GetComponent<AxeSpawning>().canDealDmg = false;
            MobSpawning.mobSp.ComboSplashText(collision.collider.transform.position, true);
            if (Random.Range(1, 15) == 1)
            {
                ThrowingController.currentHealth += ThrowingController.maxHealth / 25;
                ThrowingController.INSTANCE.ActualizeHealth();
            }
            hp -= collision.transform.GetComponent<AxeSpawning>().axeDmg;
            if (collision.transform.tag == "Trash")
                MobSpawning.mobSp.MakeSound(3);
            else
                MobSpawning.mobSp.MakeSound(2);
        }
    }
}