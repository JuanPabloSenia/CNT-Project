using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AimHelper : MonoBehaviour
{
    // TrajectoryPoint and Ball will be instantiated
    public GameObject TrajectoryPointPrefeb;
    public GameObject BallPrefb;
    public static bool isPressed;

    public static Vector3 shootPos;

    Vector3 maxSize = new Vector3(0.5f, 0.5f, 0.5f);

    private GameObject ball;
    private bool isBallThrown;
    private float power = 25;
    private int numOfTrajectoryPoints = 10;
    private List<GameObject> trajectoryPoints;
    //---------------------------------------    
    void Start()
    {
        //if (PlayerData.tutorialActive == true)
        {
            shootPos = transform.position;
            trajectoryPoints = new List<GameObject>();
            isPressed = isBallThrown = false;
            //   TrajectoryPoints are instatiated
            for (int i = 0; i < numOfTrajectoryPoints; i++)
            {
                GameObject dot = (GameObject)Instantiate(TrajectoryPointPrefeb);
                dot.transform.parent = GameObject.Find("DotHolder").transform;
                dot.GetComponent<Renderer>().enabled = false;
                trajectoryPoints.Insert(i, dot);
            }
        }
    }
    //---------------------------------------    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!ball)
                createBall();
        }
        // when mouse button is pressed, cannon is rotated as per mouse movement and projectile trajectory path is displayed.
        Vector3 vel = GameManager.dragDif/54;//GetForceFrom(ThrowingController.dragTemp/1350, ThrowingController.dragStart/1350);
        float angle = Mathf.Atan2(vel.y, vel.x) * Mathf.Rad2Deg;
        if (isPressed == true)
            setTrajectoryPoints(shootPos, vel / ball.GetComponent<Rigidbody>().mass);
    }
    //---------------------------------------    
    // Following method creates new ball
    //---------------------------------------    
    private void createBall()
    {
       ball = (GameObject)Instantiate(BallPrefb);
        Vector3 pos = shootPos;
        //pos.z = 1;
        ball.transform.position = pos;
        ball.SetActive(false);
    }
    //---------------------------------------    
    // Following method gives force to the ball
    //---------------------------------------    
    private void throwBall()
    {
        ball.SetActive(true);
        ball.GetComponent<Rigidbody>().useGravity = true;
        //ball.GetComponent<Rigidbody>().AddForce(GetForceFrom(ball.transform.position, Camera.main.ScreenToWorldPoint(Input.mousePosition)), ForceMode.Impulse);
        ball.GetComponent<Rigidbody>().AddForce(GameManager.dragDif, ForceMode.Impulse);
        isBallThrown = true;
    }
    //---------------------------------------    
    // Following method returns force by calculating distance between given two points
    //---------------------------------------    
    private Vector2 GetForceFrom(Vector2 fromPos, Vector2 toPos)
    {
        return (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y)) * power;
    }
    //---------------------------------------    
    // Following method displays projectile trajectory path. It takes two arguments, start position of object(ball) and initial velocity of object(ball).
    //---------------------------------------    
    void setTrajectoryPoints(Vector3 pStartPosition, Vector3 pVelocity)
    {
        float velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));
        float angle = Mathf.Rad2Deg * (Mathf.Atan2(pVelocity.y, pVelocity.x));
        float fTime = 0;

        if (GameManager.dragDif.x < 0 && DATA.instance.level == 2)
            setDotsInvisible();

        fTime += 0.1f;
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
            float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics.gravity.magnitude * fTime * fTime / 2.0f);
            Vector3 pos = new Vector3(pStartPosition.x + dx, pStartPosition.y + dy, pStartPosition.z + 0.3f);
            trajectoryPoints[i].transform.position = pos;
            float velAux = Mathf.Lerp(0.2f, 0.55f, velocity / 20);
            maxSize = new Vector3(velAux, velAux, velAux);
            if (DATA.instance.level == 2)
                trajectoryPoints[i].transform.localScale = maxSize*0.8f * ((1f - (float)i / 10));
            else
                trajectoryPoints[i].transform.localScale = maxSize * ((1f - (float)i / 10));
            if (isPressed == true)
                if (GameManager.dragDif.x > 0 || DATA.instance.level != 2)
                    trajectoryPoints[i].GetComponent<MeshRenderer>().enabled = true;
            trajectoryPoints[i].GetComponent<Transform>().eulerAngles = new Vector3(0, 0, Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude) * fTime, pVelocity.x) * Mathf.Rad2Deg);
            fTime += 0.1f;
            if (dx < 0)
            {
                trajectoryPoints[i].GetComponent<Transform>().position = new Vector3(trajectoryPoints[i].GetComponent<Transform>().position.x, trajectoryPoints[i].GetComponent<Transform>().position.y, -((trajectoryPoints[i].GetComponent<Transform>().position.x - shootPos.x)) + 2.5f);
            }
        }
    }
    public void setDotsInvisible()
    {
        isPressed = false;
        for (int i = 0; i < numOfTrajectoryPoints; i++)
        {
            trajectoryPoints[i].GetComponent<MeshRenderer>().enabled = false;
        }
    }
}