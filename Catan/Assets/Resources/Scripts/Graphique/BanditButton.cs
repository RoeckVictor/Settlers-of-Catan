using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Noyau.View;

public class BanditButton : MonoBehaviour
{
    public Coordinate buttonCoordinate
    {
        get
        {
            theParent = this.transform.parent.parent;
            int i, j;
            for (i = 0; i < theParent.childCount; i++)
            {
                if (i <= theParent.childCount / 2)
                {
                    for (j = 0; j < theParent.GetChild(i).childCount; j++)
                        if (this.transform == theParent.GetChild(i).GetChild(j))
                            return new Coordinate(j - i, theParent.childCount / 2 - j, i - theParent.childCount / 2, Direction.NONE);
                }
                else
                {
                    for (j = 0; j < theParent.GetChild(i).childCount; j++)
                        if (this.transform == theParent.GetChild(i).GetChild(j))
                            return new Coordinate(j - theParent.childCount / 2, (theParent.childCount - 1) - i - j, i - theParent.childCount / 2, Direction.NONE);
                }
            }
            return null;
        }
    }

    private UIControllerScript controller;
    private Transform theParent;

    public void Start()
    {
        controller = GameObject.Find("UIController").transform.GetComponent<UIControllerScript>();
    }

    public void BanditMoveOrder()
    {
        Debug.Log(buttonCoordinate);
        controller.MoveBandit(buttonCoordinate);
    }

    public void ToggleButton(bool toggle)
    {
        this.GetComponent<Image>().enabled = toggle;
        this.GetComponent<Button>().enabled = toggle;
        if (toggle)
            this.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        else
            this.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
    }
}
