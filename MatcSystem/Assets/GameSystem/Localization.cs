using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Localization
{
    public string startingLanguage = "English";
    TextAsset[] languages;
    Dictionary<string, string> mDictionary = new Dictionary<string, string>();
    string mLanguage;
    public static void LoadLanguage(string lan = "English") {

    }

    public static string LocalString(string key) {
        return "";
    }


}
