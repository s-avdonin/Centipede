using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// a class common for objects able to take a shot
public abstract class Destructible : MonoBehaviour
{
    internal abstract void ReceiveShot();
}
