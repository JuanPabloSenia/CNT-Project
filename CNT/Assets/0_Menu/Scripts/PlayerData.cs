using UnityEngine;

public class PlayerData : MonoBehaviour {

	void Awake () {
		DontDestroyOnLoad (gameObject);
	}

	void Start () {
		DATA.instance.player_name = PlayerPrefs.GetString ("player_name");
		if (DATA.instance.player_name == null) DATA.instance.player_firstLogin = 1;
		LoadScores ();
	}

	public void CreateNeWPlayer () {
		PlayerPrefs.SetString ("player_name", DATA.instance.player_name);
		PlayerPrefs.SetInt ("score_waveTotal", 0);
		PlayerPrefs.SetInt ("score_waveMax", 0);
		PlayerPrefs.SetInt ("score_enemiesKilledTotal", 0);
		PlayerPrefs.SetInt ("score_enemiesKilledMax", 0);
		DATA.instance.score_waveTotal = 0;
		DATA.instance.score_waveMax = 0;
		DATA.instance.score_enemiesKilledTotal = 0;
		DATA.instance.score_enemiesKilledMax = 0;
	}

	public void SaveName () {
		PlayerPrefs.SetString ("player_Name", DATA.instance.player_name);
	}
	public void SaveScores () {
		PlayerPrefs.SetInt ("score_waveTotal", DATA.instance.score_waveTotal);
		PlayerPrefs.SetInt ("score_waveMax", DATA.instance.score_waveMax);
		PlayerPrefs.SetInt ("score_enemiesKilledTotal", DATA.instance.score_enemiesKilledTotal);
		PlayerPrefs.SetInt ("score_enemiesKilledMax", DATA.instance.score_enemiesKilledMax);
	}
	public void LoadScores () {
		DATA.instance.score_waveTotal = PlayerPrefs.GetInt ("score_waveTotal");
		DATA.instance.score_waveMax = PlayerPrefs.GetInt ("score_waveMax");
		DATA.instance.score_enemiesKilledTotal = PlayerPrefs.GetInt ("score_enemiesKilledTotal");
		DATA.instance.score_enemiesKilledMax = PlayerPrefs.GetInt ("score_enemiesKilledMax");
	}

}
