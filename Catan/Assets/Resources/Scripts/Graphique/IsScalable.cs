using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsScalable : MonoBehaviour
{
    public Vector2 originalSize;
    private float scaleValue;

    // Update is called once per frame
    void Update()
    {
        scaleValue = Mathf.Min(Screen.width / originalSize.x, Screen.height / originalSize.y);
        this.transform.localScale = new Vector3(scaleValue, scaleValue, 1);
    }
}
