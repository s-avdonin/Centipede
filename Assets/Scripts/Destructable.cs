using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    virtual internal void ReceiveShot()
    {
        Debug.Log(name + " received shot.");
    }
}
