using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Label : MonoBehaviour
{
    [Header("Text")]
    public bool canBeTranslated = true;
    public string[] traductions = new string[2];

    public void ChangeText(string title)
    {
        this.GetComponentInChildren<TextMeshProUGUI>().text = title;
    }

    public void ChangeLangue(int langue)
    {
        if (canBeTranslated)
            this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(traductions[langue]);
    }
}
