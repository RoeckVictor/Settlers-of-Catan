using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/* Est utilise en complement des composants UI de base d'Unity pour fabriquer un checkbox (cf le Prefab CheckBox pour voir comment il fonctionne) */
public class CheckBox : MonoBehaviour
{
    [Header("Visual")]
    public Sprite parDefaut;
    public Sprite onHover;
    public Sprite whenChecked;
    public Sprite whenCheckedHover;
    public Sprite disabled;
    public Vector3 agrandisement = new Vector3(0.1f, 0.1f, 0.1f);
    public bool isEnabled = true;

    [Header("Audio")]
    public AudioSource fxSource;
    public AudioClip soundHover;
    public AudioClip soundClicked;
    public AudioClip soundExit;

    [Header("IsChecked")]
    public bool isChecked = false;

    [Header("Cursor")]
    public Texture2D defaultCursor;
    public Texture2D hoverCursor;
    public Texture2D disabledCursor;

    void Start()
    {
        this.GetComponent<Image>().sprite = parDefaut;
        fxSource = GameObject.Find("FXSource").GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isEnabled)
        {
            if (isChecked)
                this.GetComponent<Image>().sprite = whenChecked;
            else
                this.GetComponent<Image>().sprite = parDefaut;
        }
        else
            this.GetComponent<Image>().sprite = disabled;
    }

    public void MouseEnter()
    {
        if (isEnabled)
        {
            this.transform.localScale += agrandisement;
            if (soundHover != null) fxSource.PlayOneShot(soundHover);
            if (isChecked)
                this.GetComponent<Image>().sprite = whenCheckedHover;
            else
                this.GetComponent<Image>().sprite = onHover;
            Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
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
            this.transform.localScale -= agrandisement;
            if (soundExit != null) fxSource.PlayOneShot(soundExit);
            if (isChecked)
                this.GetComponent<Image>().sprite = whenChecked;
            else
                this.GetComponent<Image>().sprite = parDefaut;
        }
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    public void MouseClick()
    {
        isChecked = !isChecked;
        if (soundClicked != null) fxSource.PlayOneShot(soundClicked);
    }
}
