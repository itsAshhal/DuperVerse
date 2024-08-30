using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Switch : MonoBehaviour
{
    public Image on;
    public Image off;
    public Image img;
    public int index;
    private void Update()
    {
        //if(index==1)
        //{
        //    img.gameObject.SetActive(false);
        //}
        //if (index == 0)
        //{
        //    img.gameObject.SetActive(true);
        //}
    }

    public void ON()
    {
        index = 1;
        off.gameObject.SetActive(true);
        on.gameObject.SetActive(false);
    }
    public void OFF()
    {
        index = 1;
        on.gameObject.SetActive(true);
        off.gameObject.SetActive(false);
    }
}
