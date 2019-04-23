using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    internal virtual void ReceiveShot()
    {
        Debug.Log(name + " received shot.");
    }
}
