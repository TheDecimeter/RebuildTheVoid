using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoActionHUD : MonoBehaviour
{
    public void Activate(bool activate)
    {
        if (activate)
        {
            gameObject.SetActive(true);
            PlayerMovement.stopInput = true;
        }
        else
        {
            gameObject.SetActive(false);
            PlayerMovement.stopInput = false;
        }
    }


    private void Update()
    {
        if (Input.anyKeyDown)
        {
            Activate(false);
        }
    }
}
