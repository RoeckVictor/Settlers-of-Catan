using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Noyau.View;

public class ConstructionButton : MonoBehaviour
{
    public ConstructionType constructionType;
    public Coordinate buttonCoordinate
    {
        get
        {
            theParent = this.transform.parent.parent;
            int i, j;

            if (constructionType == ConstructionType.SETTLEMENT || constructionType == ConstructionType.CITY)
            {
                for (i = 0; i < theParent.childCount; i++)
                {
                    for (j = 0; j < theParent.GetChild(i).childCount; j++)
                    {
                        if (i < 7)
                        {
                            if (i % 2 == 0)
                            {
                                if (this.transform == theParent.GetChild(i).GetChild(j))
                                    return new Coordinate(-i / 2 + j, 2 - j, i / 2 - 2, Direction.UP);
                            }
                            else
                            {
                                if (this.transform == theParent.GetChild(i).GetChild(j))
                                    return new Coordinate(-i / 2 + j, 3 - j, i / 2 - 3, Direction.DOWN);
                            }
                        }
                        else
                        {
                            if (i % 2 == 0)
                            {
                                if (this.transform == theParent.GetChild(i).GetChild(j))
                                    return new Coordinate(-3 + j, 3 - j - (i / 2 - 2), i / 2 - 2, Direction.UP);
                            }
                            else
                            {
                                if (this.transform == theParent.GetChild(i).GetChild(j))
                                    return new Coordinate(-2 + j, 3 - j - (i / 2 - 2), i / 2 - 3, Direction.DOWN);
                            }
                        }
                    }
                }
            }
            else if (constructionType == ConstructionType.ROAD)
            {
                for (i = 0; i < theParent.childCount; i++)
                {
                    for (j = 0; j < theParent.GetChild(i).childCount; j++)
                    {
                        if (i < 6)
                        {
                            //Direction = East
                            if (i % 2 == 1)
                            {
                                if (this.transform == theParent.GetChild(i).GetChild(j))
                                    return new Coordinate(j - i + (i / 2), 3 - j, i / 2 - 2, Direction.EAST);
                            }
                            else
                            {
                                //Direction = South_East
                                if (j % 2 == 0)
                                {
                                    if (this.transform == theParent.GetChild(i).GetChild(j))
                                        return new Coordinate(j/2 - i/2, 3-j/2, i / 2 - 3, Direction.SOUTH_EAST);
                                }
                                //Direction == North_East
                                else
                                {
                                    if (this.transform == theParent.GetChild(i).GetChild(j))
                                        return new Coordinate(j/2 - i/2, 2-j/2, i/2-2, Direction.NORTH_EAST);
                                }
                            }
                        }
                        else
                        {
                            //Direction = East
                            if (i % 2 == 1)
                            {
                                if (this.transform == theParent.GetChild(i).GetChild(j))
                                    return new Coordinate(j - 3, 5 - i / 2 - j, i / 2 - 2, Direction.EAST);
                            }
                            else
                            {
                                //Direction = South_East
                                if (j % 2 == 1)
                                {
                                    if (this.transform == theParent.GetChild(i).GetChild(j))
                                        return new Coordinate(j/2-2, 5-i/2-j/2, i / 2 - 3, Direction.SOUTH_EAST);
                                }
                                //Direction == North_East
                                else
                                {
                                    if (this.transform == theParent.GetChild(i).GetChild(j))
                                        return new Coordinate(j-3-j/2, 5-i/2-j/2, i/2-2, Direction.NORTH_EAST);
                                }
                            }
                        }
                    }
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

    public void SendBuildOrder()
    {
        controller.Construction(buttonCoordinate, constructionType);
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
