using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoActionHUD : MonoBehaviour
{
    public InfoActionHUD next;
    public float MinDisplayTime = .5f;
    private float timer;
    public bool CloseOnPress = true;
    private void Start()
    {
        timer = MinDisplayTime;
        if(CloseOnPress)
            PlayerMovement.stopInput = true;
    }

    public void Activate(bool activate)
    {
        if (activate)
        {
            timer = MinDisplayTime;
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
                if (next != null)
                    next.Activate(true);
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
