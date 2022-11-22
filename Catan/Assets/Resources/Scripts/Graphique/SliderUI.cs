using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderUI : MonoBehaviour
{
    [Header("Visual")]
    public Sprite parDefaut;
    public Sprite onHover;
    public Vector3 agrandisement = new Vector3(0.1f, 0.1f, 0.1f);

    [Header("Cursor")]
    public Texture2D defaultCursor;
    public Texture2D hoverCursor;
    public Texture2D holdCursor;
    public Texture2D disabledCursor;

    void Start()
    {
        this.GetComponent<Image>().sprite = parDefaut;
    }

    void Update()
    {

    }

    public void MouseEnter()
    {
        this.GetComponent<Image>().sprite = onHover;
        this.transform.localScale += agrandisement;
        Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
    }

    public void MouseExit()
    {
        this.GetComponent<Image>().sprite = parDefaut;
        this.transform.localScale -= agrandisement;
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    public void MouseDown()
    {
        Cursor.SetCursor(holdCursor, Vector2.zero, CursorMode.Auto);
    }

    public void MouseUp()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }
}
