using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PowerUpsCanvasController : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public static GraphicRaycaster interactable;
    public static PowerUpsCanvasController INSTANCE;

    public GameObject PUPCloseBtn;
    public GameObject PUPButtons;
    public GameObject BackBtn;
    public float mana;
    public Material manaCrystalOn;
    public Material manaCrystalOff;
    public MeshRenderer[] manaCrystalsRenderers;
    int manaValue = 2000;

    [Space(10)]
    public Image openPUPImage;

    public powerUp activePowerUp;

    public Vector2 dragStart;
    public Vector2 dragDifference;

    [Space(10)]
    [Header("Arrow Storm")]
    public GameObject[] ArrowStormOptions;
    public GameObject ArrowStormGO;
    public ParticleSystem ArrowStormPSys;
    public Transform ArrowStormTrigger;
    public Rigidbody ArrowStormTriggerRB;

    [Space(10)]
    [Header("Ice Potion")]
    public IcePotionLogic icePotionInstance;
    public Transform[] icePotionPathA;
    public Transform[] icePotionPathB;
    public Transform[] icePotionPathC;

    public enum powerUp
    {
        SELECTING,
        ARROW_STORM,
        ICE_POTION
    }

    void Start () {
        if (INSTANCE == null)
            INSTANCE = this;
        interactable = GetComponent<GraphicRaycaster>();
        mana = 0;
        ActualizeMana(0);
    }

    void Update()
    {
        //Para testear la oleada que congela
        if (Input.GetKeyDown(KeyCode.A))
        {
            icePotionInstance.gameObject.SetActive(true);
            icePotionInstance.StartIcePotion(icePotionPathA);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            icePotionInstance.gameObject.SetActive(true);
            icePotionInstance.StartIcePotion(icePotionPathB);
        }
    }

    public void ActualizeMana(float points)
    {
        //Suma-Resta el mana por el valor "points", con min de 0 y max de 6 * calor de mana
        mana = Mathf.Clamp(mana += points*1.5f, 0, manaValue * 6 + 1);

        if (DATA.instance.selectedCharacter == DATA.Characters.WIZARD && points > 0) // si se usa al mago, da el doble de mana
            mana = Mathf.Clamp(mana += points * 1.5f, 0, manaValue * 6 + 1);


        //Actualiza los cristales de mana en la torre

        for (int i = 0; i < 6; i++)
        {
            if (mana > (i+1) * manaValue)
            {
                manaCrystalsRenderers[i].material = manaCrystalOn;
            }
            else
            {
                manaCrystalsRenderers[i].material = manaCrystalOff;
            }
        }
    }

    public virtual void OnPointerDown(PointerEventData ped)
    {
        dragStart = ThrowingController.RelativePedPosition(ped.position);
        if (activePowerUp == powerUp.SELECTING)
        {
            ActivatePowerUp(-1);
            ThrowingController.INSTANCE.PUpSelectionMode(false);
            return;
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(ped.position);
        Physics.Raycast(ray, out hit);
        if (hit.collider != null)
        if (hit.collider.tag == "PowerUpDisplay")
        {
            switch (activePowerUp)
            {
                case powerUp.ARROW_STORM:
                        if (ArrowStormOptions.Length != 1) //fix thissssssssssssssss its stupid af
                        {
                            BackBtn.SetActive(false);
                            ActualizeMana(-manaValue * 2);
                        }
                        ArrowStormGO.SetActive(true);
                        ArrowStormGO.transform.position = hit.collider.transform.position;
                        Debug.Log(hit.collider.transform.position);
                        Debug.Log(ArrowStormGO.transform.position);
                        StartCoroutine(ArrowStormCoroutine());
                        ActivatePowerUp(-1);
                        break;
                    case powerUp.ICE_POTION:
                        icePotionInstance.gameObject.SetActive(true);
                        break;
            }
        }
    }

    public virtual void OnDrag(PointerEventData ped)
    {

    }

    public void ActivatePowerUp(int ind)
    {
        switch (ind)
        {
            case 1:
                if (mana > manaValue * 2)
                {
                    PUPCloseBtn.SetActive(false);
                    BackBtn.SetActive(true);
                    activePowerUp = powerUp.ARROW_STORM;
                    foreach (GameObject j in ArrowStormOptions)
                        j.SetActive(true);
                }
                break;
            case -1:
                if (ArrowStormOptions.Length != 1) //fix thissssssssssssssss its stupid af
                    ThrowingController.INSTANCE.PUpSelectionMode(false);
                else//fix thissssssssssssssss its stupid af
                    GameObject.Find("PowerUpAux").SetActive(false);
                foreach (GameObject j in ArrowStormOptions)
                    j.SetActive(false);
                break;
            case 2:
                if (mana >= manaValue * 3)
                {
                    PUPButtons.SetActive(false);
                    ThrowingController.INSTANCE.PUpSelectionMode(false);
                    ActualizeMana(-manaValue * 3);
                    ThrowingController.currentHealth += 50;
                    ThrowingController.INSTANCE.ActualizeHealth();
                }
                break;
            case -100:
                BackBtn.SetActive(false);
                PUPCloseBtn.SetActive(true);
                activePowerUp = powerUp.SELECTING;
                foreach (GameObject j in ArrowStormOptions)
                    j.SetActive(false);
                break;
        }
    }

    public void CancelPowerUp()
    {
        ThrowingController.INSTANCE.PUpSelectionMode(false);
    }
    
    IEnumerator ArrowStormCoroutine()
    {
        ArrowStormTrigger.transform.position = ArrowStormPSys.transform.position + Vector3.up*14f;
        ArrowStormTriggerRB.velocity = new Vector3(0, -40, 0);
        yield return new WaitForSeconds(0.1f);
        ArrowStormPSys.emissionRate = 180;
        yield return new WaitForSeconds(0.2f);
        ArrowStormPSys.emissionRate = 0;
    }
}
