using UnityEngine;

public class DATA {

	private static DATA _instance;
	public static DATA instance {
		get {
			if (_instance == null) {
				_instance = new DATA ();
			}
			return _instance;
		}
	}

	//Informacion de partida
	public Characters selectedCharacter;
	public int idCharacter;
	public int level;

	//Informacion del jugador
	public string player_name;
    public int player_firstLogin;

	//Informacion de la partida del jugador
    public int score_waveTotal;
    public int score_waveMax;
    public int score_enemiesKilledTotal;
    public int score_enemiesKilledMax;

    public enum Characters {
		TEST,
		PALADIN,
        WIZARD,
        BLACKSMITH
    }

	//Informacion armas
	public float dmg_axe = 1;
	public float dmg_trash = 0;

	//Informacion enemigos
	public int amount_dummys = 5;

	//END
}
