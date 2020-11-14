using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoActionHUD : MonoBehaviour
{
    private float timer = .5f;
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
            timer = 0;
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
        timer -= Time.deltaTime;
        if (timer < 0 && Input.anyKeyDown)
        {
            Activate(false);
        }
    }
}
