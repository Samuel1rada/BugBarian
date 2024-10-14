using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class push : MonoBehaviour
{
    public float pushForce = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 pushDirection = other.transform.position - transform.position;
            pushDirection.Normalize();

            rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        }
    }

}
