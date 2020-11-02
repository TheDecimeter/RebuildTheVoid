using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoActionHUD : MonoBehaviour
{
    public bool CloseOnPress = true;
    private void Start()
    {
        if(CloseOnPress)
            PlayerMovement.stopInput = true;
    }

    public void Activate(bool activate)
    {
        if (activate)
        {
            gameObject.SetActive(true);
            if (CloseOnPress)
                PlayerMovement.stopInput = true;
        }
        else
        {
            if (CloseOnPress)
            {
                gameObject.SetActive(false);
                PlayerMovement.stopInput = false;
            }
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
