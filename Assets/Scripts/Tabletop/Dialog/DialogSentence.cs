using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DialogSentence
{
    public string dialogName;
    [TextArea] public string dialogSentence;
}
