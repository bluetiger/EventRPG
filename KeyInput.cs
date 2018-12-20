using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    // Start is called before the first frame update
    public MessageManager manager;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            manager.MessageListner(new Message("key", "escape"));
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            manager.MessageListner(new Message("key", "c"));
        }
    }
}
