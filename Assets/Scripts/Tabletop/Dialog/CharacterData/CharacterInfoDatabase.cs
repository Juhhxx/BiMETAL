using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfoDatabase", menuName = "Scriptable Objects/CharacterInfo")]
public class CharacterInfoDatabase : ScriptableObject
{
    [SerializeField]
    private List<CharacterInfo> characterList = new List<CharacterInfo>();

    private Dictionary<CharacterID, AudioClip> _audioDictionary;
    private Dictionary<CharacterID, string> _nameDictionary;
    private Dictionary<CharacterID, Sprite> _spriteDictionary;

    public void ReadDictionaries()
    {
        _audioDictionary = new Dictionary<CharacterID, AudioClip>();
        _nameDictionary = new Dictionary<CharacterID, string>();
        _spriteDictionary = new Dictionary<CharacterID, Sprite>();

        foreach (CharacterInfo charac in characterList)
        {
            _audioDictionary[charac.CharacterID] = charac.CharacterSound;
            _nameDictionary[charac.CharacterID] = charac.CharacterName;
            _spriteDictionary[charac.CharacterID] = charac.CharacterPicture;
        }
    }

    public AudioClip GetSound(CharacterID characterID)
    {
        _audioDictionary.TryGetValue(characterID, out AudioClip sound);
        return sound;
    }

    public string GetName(CharacterID characterID)
    {
        _nameDictionary.TryGetValue(characterID, out string name);
        return name;
    }

    public Sprite GetPicture(CharacterID characterID)
    {
        _spriteDictionary.TryGetValue(characterID, out Sprite sprite);
        return sprite;
    }
}
