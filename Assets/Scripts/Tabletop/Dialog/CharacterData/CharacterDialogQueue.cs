using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterDialog
{
    public CharacterID characterID;
    [TextArea] public List<string> dialogLines;
}

[System.Serializable]
public class DialogQueue
{
    public List<CharacterDialog> characterDialogs;
}

