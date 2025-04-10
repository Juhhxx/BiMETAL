using System;
using UnityEngine;

public class CollisionDetector : MonoBehaviour
{
    public event EventHandler<OnCollisionEventArgs> OnCollisionEnter;
    public event EventHandler<OnCollisionEventArgs> OnCollisionExit;
    public event EventHandler<OnCollisionEventArgs> OnCollisionStay;

    private void OnTriggerEnter(Collider other)
    {
        // Debug.Log("COLLIDED ENTER");
        OnCollisionEnter?.Invoke(this,new OnCollisionEventArgs {other = other, self = gameObject.GetComponent<Collider>()});
    }
    private void OnTriggerExit(Collider other)
    {
        // Debug.Log("COLLIDED EXIT");
        OnCollisionExit?.Invoke(this,new OnCollisionEventArgs {other = other, self = gameObject.GetComponent<Collider>()});
    }
    private void OnTriggerStay(Collider other)
    {
        // Debug.Log("COLLIDED");
        OnCollisionStay?.Invoke(this,new OnCollisionEventArgs {other = other, self = gameObject.GetComponent<Collider>()});
    }
}
