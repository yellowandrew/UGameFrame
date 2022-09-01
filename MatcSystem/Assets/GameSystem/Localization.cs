using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localization
{
    public string startingLanguage = "English";
    TextAsset[] languages;
    Dictionary<string, string> mDictionary = new Dictionary<string, string>();
    string mLanguage;


    public string localString(string key) {
        return "";
    }


}
