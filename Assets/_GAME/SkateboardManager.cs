using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkateboardManager : MonoBehaviour
{
    private Rigidbody _rb;

    [SerializeField, Range(0f, 10f)]
    private float _frixionStrength = 0.5f;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        _rb = gameObject.GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Update is called once per Physics update
    /// </summary>
    private void FixedUpdate()
    {
        Vector3 localSideVelocity = transform.right* transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().velocity).x;

        Debug.DrawRay(transform.position, localSideVelocity);

        _rb.AddForce(-localSideVelocity * _frixionStrength);
    }
}
