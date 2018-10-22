using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemHeadLogic : MonoBehaviour{

    bool canDealDamage = true;


	void Start ()
    {
        GameObject target = GameObject.Find("HeadTarget");
        GetComponent<Rigidbody>().AddForce((target.transform.position - transform.position + new Vector3(0, 13, 0)) * 30);
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Axes" || collision.collider.tag == "Trash")
        {
            canDealDamage = false;
        }
        if (collision.collider.tag == "Environment")
            Destroy(gameObject);
        if (collision.collider.tag == "Tower")
        {
            if (canDealDamage)
            {
                ThrowingController.currentHealth -= ThrowingController.maxHealth / 15;
                ThrowingController.INSTANCE.ActualizeHealth();
            }
            Destroy(gameObject);
        }
    }
}