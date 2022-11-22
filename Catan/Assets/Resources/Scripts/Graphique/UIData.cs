using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIData : MonoBehaviour
{
    public (string, int, bool)[] players = new (string, int, bool)[4];
    public (string, int, int) selfInfo = ("Guest", 0, 1);
    public bool IsOnline;

    public int effetsSonores = 50; // Valeur par défaut des effets sonores
    public int musique = 50; // Valeur par défaut de la musique
    public int langue = 0;
    public bool fullscreen = true;

    public void LoadOptions()
    {
        effetsSonores = PlayerPrefs.GetInt("effetsSonores");

        musique = PlayerPrefs.GetInt("musique");

        langue = PlayerPrefs.GetInt("langue");

        if (PlayerPrefs.GetInt("fullscreen") == 1) fullscreen = true;
        else fullscreen = false;
    }

    public void SaveOptions(int ef, int mus, int lang, bool screen)
    {
        PlayerPrefs.SetInt("effetsSonores", ef);

        PlayerPrefs.SetInt("musique", mus);

        PlayerPrefs.SetInt("langue", lang);

        if (screen) PlayerPrefs.SetInt("fullscreen", 1);
        else PlayerPrefs.SetInt("fullscreen", 0);

        PlayerPrefs.Save();
    }

    public void ChangeOnline(bool online)
    {
        IsOnline = online;
    }
}
