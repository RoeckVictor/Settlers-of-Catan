using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PushButton : MonoBehaviour
{
    [Header("Visual")]
    public Sprite parDefaut;
    public Sprite onHover;
    public Sprite onClick;
    public Sprite disabled;
    public Vector3 agrandisement = new Vector3(0.05f, 0.05f, 0.05f);
    public bool isEnabled = true;
    public bool isHovered = false;

    [Header("Transparence")]
    public float transparenceDefaut;
    public float transparenceHover;
    public float transparenceClicked;

    [Header("Audio")]
    public AudioSource fxSource;
    public AudioClip soundHover;
    public AudioClip soundClicked;
    public AudioClip soundExit;

    [Header("Text")]
    public bool canBeTranslated = true;
    public string[] traductions = new string[2];
    public Vector4 marginDefault;
    public Vector4 marginOnClick;

    [Header("Cursor")]
    public Texture2D defaultCursor;
    public Texture2D hoverCursor;
    public Texture2D disabledCursor;

    public void Start()
    {
        fxSource = GameObject.Find("FXSource").GetComponent<AudioSource>();
        if (isEnabled)
        {
            this.GetComponent<Image>().sprite = parDefaut;
            this.GetComponent<Button>().enabled = true;
        }
        if (!isEnabled)
        {
            this.GetComponent<Image>().sprite = disabled;
            this.GetComponent<Button>().enabled = false;
        }
    }

    public void MouseEnter()
    {
        if(isEnabled)
        {
            this.GetComponent<Image>().sprite = onHover;
            this.GetComponent<Image>().color = new Color(this.GetComponent<Image>().color.r, this.GetComponent<Image>().color.g, this.GetComponent<Image>().color.b, transparenceHover);
            this.transform.localScale += agrandisement;
            if(soundHover!=null) fxSource.PlayOneShot(soundHover);
            Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
            isHovered = true;
        }
        else
        {
            Cursor.SetCursor(disabledCursor, Vector2.zero, CursorMode.Auto);
        }
    }

    public void MouseExit()
    {
        if (isEnabled)
        {
            this.GetComponent<Image>().sprite = parDefaut;
            this.GetComponent<Image>().color = new Color(this.GetComponent<Image>().color.r, this.GetComponent<Image>().color.g, this.GetComponent<Image>().color.b, transparenceDefaut);
            this.transform.localScale -= agrandisement;
            if (soundExit != null) fxSource.PlayOneShot(soundExit);
            isHovered = false;
        }
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    public void MouseDown()
    {
        if (isEnabled)
        {
            this.GetComponent<Image>().sprite = onClick;
            this.GetComponentInChildren<TextMeshProUGUI>().margin = marginOnClick;
            this.GetComponent<Image>().color = new Color(this.GetComponent<Image>().color.r, this.GetComponent<Image>().color.g, this.GetComponent<Image>().color.b, transparenceClicked);
            if (soundClicked != null) fxSource.PlayOneShot(soundClicked);
        }
    }

    public void MouseUp()
    {
        if (isEnabled)
        {
            this.GetComponent<Image>().sprite = parDefaut;
            this.GetComponentInChildren<TextMeshProUGUI>().margin = marginDefault;
            this.GetComponent<Image>().color = new Color(this.GetComponent<Image>().color.r, this.GetComponent<Image>().color.g, this.GetComponent<Image>().color.b, transparenceDefaut);
        }
            
    }

    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;

        if(isEnabled)
        {
            this.GetComponent<Image>().sprite = parDefaut;
            this.GetComponent<Button>().enabled = true;
        }
        if(!isEnabled)
        {
            this.GetComponent<Image>().sprite = disabled;
            this.GetComponent<Button>().enabled = false;
        }
    }

    public void ChangeLangue(int langue)
    {
        if(canBeTranslated)
            this.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(traductions[langue]);
    }
    
    public void SetPositionX(float pos)
    {
        this.transform.localPosition = new Vector2(pos, this.transform.localPosition.y);
    }

    public void SetPositionY(float pos)
    {
        this.transform.localPosition = new Vector2(this.transform.localPosition.x, pos);
    }
}
