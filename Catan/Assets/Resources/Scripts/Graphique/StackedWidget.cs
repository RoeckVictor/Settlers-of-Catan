using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackedWidget : MonoBehaviour
{
    public float target = 1;

    void Update()
    {
        this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(target * Screen.width / 2, this.transform.position.y, this.transform.position.z), Time.deltaTime * 4);
        if (Mathf.Abs(this.transform.position.x - target * Screen.width / 2) < 10)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, new Vector3(target * Screen.width / 2, this.transform.position.y, this.transform.position.z), Time.deltaTime * 6);
        }
    }

    public void Changetarget(int x)
    {
        target = x;
    }
}
