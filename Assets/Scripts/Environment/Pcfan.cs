using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pcfan : MonoBehaviour
{

    [SerializeField] private GameObject pcfan;
    [SerializeField] private float RotationSpeed;
    public float pushForce = 10f;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        pcfan.transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
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