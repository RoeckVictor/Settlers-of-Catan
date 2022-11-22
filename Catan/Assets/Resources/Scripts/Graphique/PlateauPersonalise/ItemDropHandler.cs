using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Noyau.View;

public class ItemDropHandler : MonoBehaviour, IDropHandler
{
    public int tagItem1;
    public int tagItem2;

    public Transform gameMaster;

    public GameObject item
    {
        get
        {
            if (transform.childCount > 0)
            {
                return transform.GetChild(0).gameObject;
            }
            return null;
        }
    }

    public GameObject item2
    {
        get
        {
            if(transform.childCount > 1)
            {
                return transform.GetChild(1).gameObject;
            }
            return null;
        }
    }

    public void Start()
    {
        gameMaster = GameObject.Find("UIController").transform;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(ItemDragHandler.item != null)
        {
            //si il n'y a rien dans la case
            if (!item)
                ItemDragHandler.item.transform.SetParent(transform);

            //si il y a 1 objet dans la case, et qu'on essaye de deposer un objet du meme type dedans
            else if (item && !item2 && item.GetComponent<ItemDragHandler>().theTag == ItemDragHandler.item.GetComponent<ItemDragHandler>().theTag)
            {
                Transform aux = ItemDragHandler.item.transform.parent;
                GameObject theItem = item;
                if ((!item2 && !aux.GetComponent<ItemDropHandler>().item2) || transform.GetComponent<ItemDropHandler>().item.GetComponent<ItemDragHandler>().typeTile != TerrainType.DESERT)
                {
                    item.transform.SetParent(aux);
                    theItem.transform.localPosition = Vector3.zero;
                    ItemDragHandler.item.transform.SetParent(transform);
                    ItemDragHandler.item.transform.localPosition = Vector3.zero;
                    //if(theItem.GetComponent<ItemDragHandler>().theTag == tagItem1 && aux.childCount>1)
                    //aux.GetChild(1).SetSiblingIndex(0);
                }
            }

            //si il y a 1 objet dans la case et qu'on essaye d'y deposer un nombre
            else if (item && !item2 && ItemDragHandler.item.GetComponent<ItemDragHandler>().theTag == tagItem2)
            {
                //si l'objet qu'on essaye de deposer est different que celui dans la case et que ce n'est pas un desert
                if (item.GetComponent<ItemDragHandler>().theTag != ItemDragHandler.item.GetComponent<ItemDragHandler>().theTag && item.GetComponent<ItemDragHandler>().typeTile != TerrainType.DESERT)
                    ItemDragHandler.item.transform.SetParent(transform);
            }

            //si il y a 1 objet dans la case, que cet objet est un nombre et qu'on essaye de deposer une tuile
            else if (item && !item2 && item.GetComponent<ItemDragHandler>().theTag == tagItem2 && ItemDragHandler.item.GetComponent<ItemDragHandler>().theTag == tagItem1)
            {
                //si la tuile qu'on essaye de deposer n'est pas un desert
                if (ItemDragHandler.item.GetComponent<ItemDragHandler>().typeTile != TerrainType.DESERT)
                {
                    ItemDragHandler.item.transform.SetParent(transform);
                    transform.GetChild(1).SetSiblingIndex(0);
                }
            }

            //si il y a 2 objets dans la case
            else if (item && item2)
            {
                if (item.GetComponent<ItemDragHandler>().theTag == ItemDragHandler.item.GetComponent<ItemDragHandler>().theTag && ItemDragHandler.item.GetComponent<ItemDragHandler>().typeTile != TerrainType.DESERT)
                {
                    Transform aux = ItemDragHandler.item.transform.parent;
                    GameObject theItem = item;
                    item.transform.SetParent(aux);
                    theItem.transform.localPosition = Vector3.zero;
                    ItemDragHandler.item.transform.SetParent(transform);
                    ItemDragHandler.item.transform.localPosition = Vector3.zero;
                    transform.GetChild(1).SetSiblingIndex(0);
                    if (aux.childCount > 1)
                        aux.GetChild(1).SetSiblingIndex(0);
                }
                else if (item2.GetComponent<ItemDragHandler>().theTag == ItemDragHandler.item.GetComponent<ItemDragHandler>().theTag)
                {
                    Transform aux = ItemDragHandler.item.transform.parent;
                    GameObject theItem = item2;
                    item2.transform.SetParent(aux);
                    theItem.transform.localPosition = Vector3.zero;
                    ItemDragHandler.item.transform.SetParent(transform);
                    ItemDragHandler.item.transform.localPosition = Vector3.zero;
                }
            }
            gameMaster.GetComponent<UIControllerScript>().PrepareLaunchButtonLocal();
        }
    }

    public bool IsFull()
    {
        if (item && item2)
            return true;
        else if (item && item.GetComponent<ItemDragHandler>().typeTile == TerrainType.DESERT)
            return true;
        return false;
    }
}
