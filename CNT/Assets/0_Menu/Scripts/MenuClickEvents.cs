using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.EventSystems;

//UBICACION: MANAGER_Main Menu, en escena "0-Main Menu"
//FUNCION:
public class MenuClickEvents : MonoBehaviour
{
	[Header("Animator")]
    public Animator anim_MainMenu;
    public Animator anim_FadeMenuCanvas;

	[Header ("Buttons")]
	public GameObject baseBtns;
    public GameObject backBtn;
    public GameObject levelBtns;
    public GameObject contactBtns;

	[Header ("Others")]
	public bool turnAlphaUp; //PREGUNTAR
    public bool unique; //PREGUNTAR
    public Material planeMaterial; //Material para BLUR, aplicado a "screen_Fade"

    private void Start()
    {
        if (planeMaterial !=null) planeMaterial.color = Color.clear;
        ComboTextLogic.combo = 0; //PREGUNTAR
    }

    private void Update()
    {
		//Cambia el material de "screen_Fade" para BLUR con el tiempo
		if (unique) {
            if (turnAlphaUp)
                planeMaterial.color = Color.Lerp(new Color(1, 1, 1, planeMaterial.color.a), Color.white, 4f * Time.deltaTime);
            else
                planeMaterial.color = Color.Lerp(new Color(1, 1, 1, planeMaterial.color.a), Color.clear, 4f * Time.deltaTime);
        }
    }

    public void Pause()
    {
		anim_MainMenu.SetBool("PauseOn", true);
    }

    public void Unpause()
    {
		anim_MainMenu.SetBool("PauseOn", false);
        Time.timeScale = 1;
    }

    public void SetTimeScale()
    {
        Time.timeScale = 0;
    }

    public void GoToScene(int scene)
    {
        Time.timeScale = 1;
        ThrowingController.isGameOver = false;
		anim_FadeMenuCanvas.SetTrigger("GoToMenu");
        StartCoroutine(LoadSceneCoroutine(scene));
    }

    IEnumerator LoadSceneCoroutine(int scene)
    {
        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene(scene);
    }

	//Abre el libro Play
	public void menuOpenPlay () {
		anim_MainMenu.SetBool ("Play", true);
		baseBtns.SetActive (false);
		backBtn.SetActive (true);
	}

	//Abre el libro Contact Us
	public void menuOpenContactUs()
    {
		anim_MainMenu.SetBool("ContactUs", true);
        baseBtns.SetActive(false);
        backBtn.SetActive(true);
    }

	//Vuelve al Menu Principal
    public void menuBack()
    {
		anim_MainMenu.SetBool("Play", false);
		anim_MainMenu.SetBool("ContactUs", false);
        levelBtns.SetActive(false);
        contactBtns.SetActive(false);
        baseBtns.SetActive(true);
        backBtn.SetActive(false);
    }

    public void menuLevelBtnsOn()
    {
        levelBtns.SetActive(true);
    }

    public void menuOpenLevel(int sceneIndex)
    {
        turnAlphaUp = false;
        levelBtns.SetActive(false);
        backBtn.SetActive(false);
		anim_MainMenu.SetBool("PlayLevel", true);
        StartCoroutine(menuStartLevelDelay(sceneIndex));
    }

    IEnumerator menuStartLevelDelay(int sceneIndex)
    {
        yield return new WaitForSeconds(2);
		anim_FadeMenuCanvas.SetTrigger("GoToMenu");
        yield return new WaitForSeconds(1);
        if (sceneIndex == 2)
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            //levelToStart = sceneIndex;
            SceneManager.LoadScene(1);
        }
    }

    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }

    public void menuContactBtnsOn()
    {
        contactBtns.SetActive(true);
    }
}
//https://www.instagram.com/bearslapgames/