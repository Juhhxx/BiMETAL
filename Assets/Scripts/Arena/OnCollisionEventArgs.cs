using System;
using UnityEngine;
public class OnCollisionEventArgs : EventArgs
{
    public Collider other;
    public Collider self;
}
