using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Ce script permet au objets auxquels il est attache de persister entre les scenes */
public class DoNotDestroyOnLoad : MonoBehaviour
{
    static DoNotDestroyOnLoad instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            //Destroy(gameObject);
            Destroy(instance);
    }
}
