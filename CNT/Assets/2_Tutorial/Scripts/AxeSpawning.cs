using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class AxeSpawning : MonoBehaviour {

    public BoxCollider Edge;
    public BoxCollider Stick;
    public Rigidbody rBody;

    public static float axeMaxDmg;
    public static float trashMaxDmg;
    public Vector3 rotOffset;

    public float axeDmg;
    public bool canDealDmg;
    public bool axeDestroy;
    public float axeTimer;
    public Vector2 axeDirection;

    void Awake()
    {
        axeMaxDmg = 1;
        axeDmg = axeMaxDmg;
        if (tag == "Trash")
            axeDmg = 0.4f;
        axeDestroy = false;
    }

	public void StartWeapon (bool left)
    {
        //rBody reseting
        {
            rBody.useGravity = true;
            rBody.drag = 0;
            rBody.angularDrag = 0.5f;
            rBody.velocity = Vector3.zero;
            rBody.angularVelocity = Vector3.zero;
            canDealDmg = true;
            if (Edge != null)
                Edge.enabled = true;
            Stick.enabled = true;
        }
        axeDirection.y += 50;
        if (left)
        {
            transform.rotation = ThrowingController.rightShootPos.transform.rotation;
            transform.Rotate(rotOffset);
            rBody.AddForce(axeDirection);
            rBody.angularVelocity = new Vector3(0, 0, -axeDirection.x/7.3f);
        }
        else
        {
            transform.rotation = ThrowingController.leftShootPos.transform.rotation;
            transform.Rotate(rotOffset);
            rBody.AddForce(new Vector3(axeDirection.x, axeDirection.y, -axeDirection.x));
            rBody.angularVelocity = new Vector3(-axeDirection.x / 4, 0, -axeDirection.x / 4);
        }
    }

    private void Update()
    {
        if (axeDestroy == true)
            axeTimer += Time.deltaTime;
        if (axeTimer > 2)
            gameObject.SetActive(false);
    }

    public void DestroyAxe()
    {
        rBody.drag = 0;
        rBody.angularDrag = 0.5f;
        rBody.velocity = Vector3.zero;
        rBody.angularVelocity = Vector3.zero;
        gameObject.SetActive(false);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Environment")
        {
            if (canDealDmg && tag != "Trash")
            {
                Debug.Log("ComboReset");
                ComboTextLogic.combo = 0;
                MobSpawning.mobSp.ComboSplashText(transform.position, false);
            }
            canDealDmg = false;
            axeDestroy = true;
        }
    }
}
