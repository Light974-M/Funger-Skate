using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkateboardManager : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField, Range(0f, 10f)]
    private float _frixionStrength = 0.5f;

    // Start is called before the first frame update
    private void Awake()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 localSideVelocity = transform.right* transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().velocity).x;

        Debug.DrawRay(transform.position, localSideVelocity);

        _rb.AddForce(-localSideVelocity * _frixionStrength);
    }
}
