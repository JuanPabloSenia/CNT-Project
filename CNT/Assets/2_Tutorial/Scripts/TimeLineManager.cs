using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimeLineManager : MonoBehaviour {

    public static TimeLineManager timeLineManager;

    public static PlayableDirector dir;


    public PlayableAsset intro;
    public PlayableAsset respawnDummy;
    public PlayableAsset outro;

    void Awake () {
        if (timeLineManager == null)timeLineManager = this;
        dir = GetComponent<PlayableDirector>();
        PlayTutorial(0);
	}

    public static void PlayTutorial(int part)
    {
        switch (part)
        {
            case 0:
                dir.playableAsset = timeLineManager.intro;
                break;
            case 1:
                dir.playableAsset = timeLineManager.respawnDummy;
                break;
            case 2:
                dir.playableAsset = timeLineManager.outro;
                break;
        }
        dir.Play();
    }
}
