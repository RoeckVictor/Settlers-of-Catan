using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifyOfferedRessources : MonoBehaviour
{
    public bool isAdd = true;
    public int theRessource = 0;
    public int thePlayerSlot = 0;

    private UIControllerScript controller;

    public void Start()
    {
        controller = GameObject.Find("UIController").transform.GetComponent<UIControllerScript>();
    }

    public void SendOfferModify()
    {
        if (isAdd)
            controller.AddOfferedRessource(thePlayerSlot, theRessource);
        else
            controller.SubstractOfferedRessource(thePlayerSlot, theRessource);
    }
}
