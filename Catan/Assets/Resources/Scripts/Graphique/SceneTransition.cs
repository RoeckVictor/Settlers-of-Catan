using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public Animator levelTransition;
    public AudioSource musicSource;
    public Transform uiController = null;

    int levelToLoad;

    public void BackToMenu()
    {
        levelToLoad = 0;
        levelTransition.SetTrigger("FadeOut");
        musicSource.Stop();
    }

    public void StartGameLocal()
    {
        levelToLoad = 1;
        levelTransition.SetTrigger("FadeOut");
        musicSource.Stop();
    }

    public void LevelTransitionFinished()
    {
        SceneManager.LoadScene(levelToLoad);
    }
}
