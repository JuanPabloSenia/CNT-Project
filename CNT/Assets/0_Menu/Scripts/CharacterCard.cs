using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "New Character")]
public class CharacterCard : ScriptableObject{
    public string characterName;
    public Sprite artWork;
    public string description;
}
