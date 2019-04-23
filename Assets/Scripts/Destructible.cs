using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    virtual internal void ReceiveShot()
    {
        Debug.Log(name + " received shot.");
    }
}
