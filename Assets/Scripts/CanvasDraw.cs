using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasDraw : MonoBehaviour
{
    public void ShowDrawCanvas()
    {
        gameObject.SetActive(true);
    }
    public void HideDrawCanvas()
    {
        gameObject.SetActive(false);
    }
}
