using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuTransition : MonoBehaviour
{
    public GameObject[] menus;

    private int currentIndex = 0;
    private int newIndex = 0;

    public void FadeMenuTo(int index)
    {
        newIndex = index;
        this.GetComponent<Animator>().SetTrigger("Fade");
    }

    public void ChangeMenu()
    {
        menus[currentIndex].SetActive(false);
        menus[newIndex].SetActive(true);
        currentIndex = newIndex;
        this.GetComponent<Animator>().ResetTrigger("Fade");
    }
}
