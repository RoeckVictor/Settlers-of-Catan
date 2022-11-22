using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RadioButton : MonoBehaviour
{
    [Header("Visual")]
    public Sprite parDefaut;
    public Sprite onHover;
    public Sprite whenChecked;
    public Sprite disabled;
    public Vector3 agrandisement = new Vector3(0.05f, 0.05f, 0.05f);
    public bool isEnabled = true;

    [Header("Transparence")]
    public float transparenceDefaut;
    public float transparenceHover;
    public float tranparenceChecked;

    [Header("Audio")]
    public AudioSource fxSource;
    public AudioClip soundHover;
    public AudioClip soundClicked;
    public AudioClip soundExit;

    [Header("Text")]
    public bool canBeTranslated = true;
    public TMP_ColorGradient radioColor;
    public TMP_ColorGradient radioColor2;
    public string[] traductions = new string[2];

    [Header("Radio Group")]
    public bool isChecked = false;
    public bool isHover = false;
    public Transform radioGroup;

    [Header("Cursor")]
    public Texture2D defaultCursor;
    public Texture2D hoverCursor;
    public Texture2D disabledCursor;

    void Start()
    {
        this.GetComponent<Image>().sprite = parDefaut;
        radioGroup = this.transform.parent;
        fxSource = GameObject.Find("FXSource").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isEnabled)
        {
            if(isChecked)
                this.GetComponent<Image>().sprite = whenChecked;
            else if (!isHover)
            {
                this.GetComponent<Image>().color = new Color(this.GetComponent<Image>().color.r, this.GetComponent<Image>().color.g, this.GetComponent<Image>().color.b, transparenceDefaut);
                this.GetComponent<Image>().sprite = parDefaut;
            }
        }
        else
            this.GetComponent<Image>().sprite = disabled;
    }

    public void MouseEnter()
    {
        if (isEnabled && !isChecked)
        {
            isHover = true;
            this.transform.localScale += agrandisement;
            this.GetComponent<Image>().sprite = onHover;
            this.GetComponent<Image>().color = new Color(this.GetComponent<Image>().color.r, this.GetComponent<Image>().color.g, this.GetComponent<Image>().color.b, transparenceHover);
            if (soundHover != null) fxSource.PlayOneShot(soundHover);
            Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
        }
        if (!isEnabled)
        {
            Cursor.SetCursor(disabledCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    public void MouseExit()
    {
        if (isEnabled && !isChecked)
        {
            isHover = false;
            this.transform.localScale -= agrandisement;
            this.GetComponent<Image>().sprite = parDefaut;
            this.GetComponent<Image>().color = new Color(this.GetComponent<Image>().color.r, this.GetComponent<Image>().color.g, this.GetComponent<Image>().color.b, transparenceDefaut);
            if (soundExit != null) fxSource.PlayOneShot(soundExit);
        }
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    public void MouseClick()
    {
        if (!isChecked && isEnabled)
        {
            int i;
            for (i = 0; i < radioGroup.childCount; i++)
            {
                radioGroup.GetChild(i).GetComponent<RadioButton>().isChecked = false;
                radioGroup.GetChild(i).GetComponent<RadioButton>().isHover = false;
                radioGroup.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().colorGradientPreset = radioColor;
            }
            this.transform.localScale -= agrandisement;
            this.GetComponent<Image>().color = new Color(this.GetComponent<Image>().color.r, this.GetComponent<Image>().color.g, this.GetComponent<Image>().color.b, transparenceHover);
            this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().colorGradientPreset = radioColor2;
            if (soundClicked != null) fxSource.PlayOneShot(soundClicked);
            isChecked = true;
        }
    }

    public void ChangeLangue(int langue)
    {
        if (canBeTranslated)
            this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(traductions[langue]);
    }
}
