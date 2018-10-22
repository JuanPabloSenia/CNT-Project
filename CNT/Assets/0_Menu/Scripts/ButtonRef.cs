using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonRef : MonoBehaviour {

    public static float time;
    public static int log;
    public static int mis;
    public static int cre;
    public static int pla;

    public static string ActiveMenu;

    public static GameObject MainMenuLog;
    public static GameObject MisMenu;
    public static GameObject CreMenu;
    public static GameObject PlayBtn;
    public static GameObject CreditsBtn;
    public static GameObject MissionsBtn;

    void Awake () {
        time = 0;
        log = 0;
        cre = 0;
        mis = 0;
        pla = 0;
        ActiveMenu = "MainMenu";
        MisMenu = GameObject.Find("MissionsMenu");
        CreMenu = GameObject.Find("CreditsMenu");
        MainMenuLog = GameObject.Find("Log");
        MisMenu.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
        CreMenu.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
        MisMenu.GetComponentInChildren<Image>().raycastTarget = false;
        CreMenu.GetComponentInChildren<Image>().raycastTarget = false;
        OpenMainMenu();
	}

    void Update()
    {
        if (time < 1)
        time += Time.deltaTime;
        if (log == 1)
        {
            MainMenuLog.transform.rotation = Quaternion.Lerp(new Quaternion(0, 0, 0, 90), new Quaternion(0, 0, 45, 90), time);
        }
        if (log == 2)
        {
            MainMenuLog.transform.rotation = Quaternion.Lerp(new Quaternion(0, 0, 45, 90), new Quaternion(0, 0, 0, 90), time);
        }
        if (mis == 1)
        {
            MisMenu.GetComponentInChildren<Image>().color = new Color(1, 1, 1, Mathf.Lerp(0, 1, time * 1.5f));
        }
        if (mis == 2)
        {
            MisMenu.GetComponentInChildren<Image>().color = new Color(1, 1, 1, Mathf.Lerp(1, 0, time * 1.5f));
        }
        if (cre == 1)
        {
            CreMenu.GetComponentInChildren<Image>().color = new Color(1, 1, 1, Mathf.Lerp(0, 1, time * 1.5f));
        }
        if (cre == 2)
        {
            CreMenu.GetComponentInChildren<Image>().color = new Color(1, 1, 1, Mathf.Lerp(1, 0, time * 1.5f));
        }
    }

    public void CloseMainMenu()
    {
        time = 0;
        log = 1;
    }

    public void OpenMainMenu()
    {
        log = 2;
    }

    public void CloseMissionsMenu()
    {
        time = 0;
        MisMenu.GetComponentInChildren<Image>().raycastTarget = false;
        mis = 2;
    }

    public void OpenMissionsMenu()
    {
        cre = 0;
        pla = 0;
        MisMenu.GetComponentInChildren<Image>().raycastTarget = true;
        mis = 1;
    }

    public void CloseCreditsMenu()
    {
        time = 0;
        CreMenu.GetComponentInChildren<Image>().raycastTarget = false;
        cre = 2;
    }

    public void OpenCreditsMenu()
    {
        mis = 0;
        pla = 0;
        CreMenu.GetComponentInChildren<Image>().raycastTarget = true;
        cre = 1;
    }

    public void CloseGameMenu()
    {
        time = 0;
        pla = 2;
    }

    public void OpenGameMenu()
    {
        cre = 0;
        mis = 0;
        pla = 1;
    }
}
