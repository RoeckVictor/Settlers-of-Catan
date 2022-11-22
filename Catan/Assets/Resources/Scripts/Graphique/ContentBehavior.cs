using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentBehavior : MonoBehaviour
{
    public Vector2 defaultSize;
    public Vector2 enlargedSize;

    private bool isEnlarged = false;

    void Start()
    {
        this.GetComponent<RectTransform>().sizeDelta = defaultSize;
    }

    public void Enlarge()
    {
        if (!isEnlarged)
        {
            this.GetComponent<RectTransform>().sizeDelta = enlargedSize;
            int i;
            for (i = 0; i < this.transform.childCount; i++)
            {
                this.transform.GetChild(i).localPosition = new Vector2(this.transform.GetChild(i).localPosition.x + (enlargedSize.x - defaultSize.x) / 2, this.transform.GetChild(i).localPosition.y + (enlargedSize.y - defaultSize.y) / 2);
            }
            isEnlarged = true;
        }
    }

    public void Shrink()
    {
        if (isEnlarged)
        {
            this.GetComponent<RectTransform>().sizeDelta = defaultSize;
            int i;
            for (i = 0; i < this.transform.childCount; i++)
            {
                this.transform.GetChild(i).localPosition = new Vector2(this.transform.GetChild(i).localPosition.x - (enlargedSize.x - defaultSize.x) / 2, this.transform.GetChild(i).localPosition.y - (enlargedSize.y - defaultSize.y) / 2);
            }
            isEnlarged = false;
        }
    }
}
