using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnimation : MonoBehaviour
{
    public Animator titleTransition;
    public Transform uiController = null;

    int levelToLoad;

    public void PopTitle()
    {
        titleTransition.SetTrigger("animating");
    }

    public void TitleTransitionFinished()
    {
        titleTransition.ResetTrigger("animating");
        uiController.GetComponent<UIControllerScript>().PopTitleFinished();
    }
}
