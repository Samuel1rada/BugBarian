using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Pcfan : MonoBehaviour
{

    [SerializeField] private GameObject pcfan;
    [SerializeField] private float RotationSpeed;
    private float pushForce = 10f;
    

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        pcfan.transform.Rotate(Vector3.up * RotationSpeed * Time.deltaTime);
    }
    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.useGravity = false;
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.disableGravity = true;
                player.intrigger = true;
                Vector3 pushDirection = other.transform.position - transform.position;
                pushDirection.Normalize();

                rb.AddForce(pushDirection * pushForce, ForceMode.Force);
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.disableGravity = false;
                player.intrigger = false;
            }
        }

    }
}