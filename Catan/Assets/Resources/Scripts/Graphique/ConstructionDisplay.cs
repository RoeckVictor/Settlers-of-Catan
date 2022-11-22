using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Noyau.View;

public class ConstructionDisplay : MonoBehaviour
{
    public Sprite invisible = null;
    public Sprite[] colony = new Sprite[4];
    public Sprite[] city = new Sprite[4];
    public Sprite[] road = new Sprite[4];
    public Sprite bandit;

    public ConstructionType constructionType;
    public Coordinate displayCoordinate
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
                                        return new Coordinate(j / 2 - i / 2, 3 - j / 2, i / 2 - 3, Direction.SOUTH_EAST);
                                }
                                //Direction == North_East
                                else
                                {
                                    if (this.transform == theParent.GetChild(i).GetChild(j))
                                        return new Coordinate(j / 2 - i / 2, 2 - j / 2, i / 2 - 2, Direction.NORTH_EAST);
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
                                        return new Coordinate(j / 2 - 2, 5 - i / 2 - j / 2, i / 2 - 3, Direction.SOUTH_EAST);
                                }
                                //Direction == North_East
                                else
                                {
                                    if (this.transform == theParent.GetChild(i).GetChild(j))
                                        return new Coordinate(j - 3 - j / 2, 5 - i / 2 - j / 2, i / 2 - 2, Direction.NORTH_EAST);
                                }
                            }
                        }
                    }
                }
            }
            else if(constructionType == ConstructionType.NONE)
            {
                for (i = 0; i < theParent.childCount; i++)
                {
                    if (i <= theParent.childCount / 2)
                    {
                        for (j = 0; j < theParent.GetChild(i).childCount; j++)
                        {
                            if (this.transform == theParent.GetChild(i).GetChild(j))
                                return new Coordinate(j - i, theParent.childCount / 2 - j, i - theParent.childCount / 2, Direction.NONE);
                        }
                    }
                    else
                    {
                        for (j = 0; j < theParent.GetChild(i).childCount; j++)
                        {
                            if (this.transform == theParent.GetChild(i).GetChild(j))
                                return new Coordinate(j - theParent.childCount / 2, (theParent.childCount - 1) - i - j, i - theParent.childCount / 2, Direction.NONE);
                        }
                    }
                } 
            }
            return null;
        }
    }

    private Transform theParent;

    public void ChangeSprite(ConstructionType buildingType, int player)
    {
        if (player == -1)
            this.transform.GetComponent<Image>().sprite = invisible;
        else if (buildingType == ConstructionType.SETTLEMENT)
            this.transform.GetComponent<Image>().sprite = colony[player];
        else if (buildingType == ConstructionType.CITY)
            this.transform.GetComponent<Image>().sprite = city[player];
        else if (buildingType == ConstructionType.ROAD)
            this.transform.GetComponent<Image>().sprite = road[player];
    }

    public void ToggleBandit(bool toggle)
    {
        if(toggle)
            this.transform.GetComponent<Image>().sprite = bandit;
        else
            this.transform.GetComponent<Image>().sprite = invisible;
    }
}
