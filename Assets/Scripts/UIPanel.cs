using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel : MonoBehaviour
{
    // Used for button click to turn off and on the UI panels
    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
