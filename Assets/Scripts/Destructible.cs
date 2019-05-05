using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// a class common for objects able to take a shot
public class Destructible : MonoBehaviour
{
    internal virtual void ReceiveShot()
    {
        // log message
        Debug.Log(name + " received shot.");
    }
}
