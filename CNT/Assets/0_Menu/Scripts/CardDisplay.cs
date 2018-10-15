using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour {

    public CharacterCard characterCard;

    public Text cardName;
    public Image cardImage;
    public Text cardDescription;

	void Start () {
        cardName.text = characterCard.characterName;
        cardImage.sprite = characterCard.artWork;
        cardDescription.text = characterCard.description;
	}
}
