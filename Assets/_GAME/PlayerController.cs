using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 1;

    [SerializeField, Tooltip("value for scaling speed of rotation")]
    private float _turnSpeed = 1;

    [SerializeField, Tooltip("the higher the value is, the quickest the turn speed will slow down when velocity is increasing"), Range(0, 1)]
    private float _turnSpeedSlowDownScale = 1;

    [SerializeField]
    private float _velocityClamp = 10;

    [SerializeField, Range(0f, 10f)]
    private float _frixionStrength = 0.5f;

    [SerializeField, Tooltip("transform of board mesh")]
    private Transform _boardMesh;

    [SerializeField, Tooltip("duration of flip process to return skate")]
    private float _flipDuration = 1;

    [SerializeField]
    private float _vrilleStrength = 10;

    [SerializeField]
    private float A;

    [SerializeField]
    private float B;

    private float _returnPhase = -1;

    private float _speedLower = 1;

    private float _timer = 0;
    private float _timer2 = 0;

    bool _firstTimeGetUp = false;
    bool _firstTimeBackGetUp = false;

    private bool _getUpFirstPhase = false;
    private bool _backGetUpFirstPhase = false;

    private bool _canVrille = false;

    private bool _isGrounded = true;

    /// <summary>
    /// value used to calculate speed of rotation
    /// </summary>
    private float _turnFactor = 1;

    private Rigidbody _rb;

    #region Public API

    public bool IsGrounded => _isGrounded;

    #endregion

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        _rb = gameObject.GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Update is called once per Physics update
    /// </summary>
    private void FixedUpdate()
    {
        _isGrounded = Physics.CheckSphere(Vector3.Lerp(-transform.up + ((transform.forward * 3.15f) + transform.position), transform.position, 0.84f), 0.08f) || Physics.CheckSphere(Vector3.Lerp(-transform.up + ((-transform.forward * 2.7f) + transform.position), transform.position, 0.84f), 0.08f);
        bool isTouchingAnything = Physics.CheckBox(new Vector3(transform.position.x, transform.position.y - 0.075f, transform.position.z), new Vector3(0.45f, 0.35f, 1.8f) / 2, transform.rotation, ~LayerMask.NameToLayer("Ground"));

        if (_isGrounded)
            GroundControls();
        else
            AirControls();

        //if (isTouchingAnything && Input.GetKeyDown(KeyCode.R))
        //    transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        if (_returnPhase >= 0)
        {
            transform.Rotate(0, 0, (Time.deltaTime / _flipDuration) * 180);

            _returnPhase += Time.deltaTime;

            if (_returnPhase >= _flipDuration)
                _returnPhase = -1;
        }
    }

    /// <summary>
    /// Update is called every frame
    /// </summary>
    private void Update()
    {
        bool isTouchingAnything = Physics.CheckBox(new Vector3(transform.position.x, transform.position.y - 0.075f, transform.position.z), new Vector3(0.45f, 0.35f, 1.8f) / 2, transform.rotation, ~LayerMask.NameToLayer("Ground"));

        if (isTouchingAnything && Input.GetKeyDown(KeyCode.R) && _returnPhase == -1)
        {
            _rb.AddForce(0, 100, 0);
            _returnPhase = 0;
        }
        Debug.DrawRay(transform.position, _rb.angularVelocity, Color.white);
        Debug.DrawRay(transform.position, _rb.angularVelocity - (transform.right * transform.InverseTransformDirection(_rb.angularVelocity).x), Color.red);
    }

    private void SkateFrixion()
    {
        Vector3 localSideVelocity = transform.right * transform.InverseTransformDirection(gameObject.GetComponent<Rigidbody>().velocity).x;

        Debug.DrawRay(transform.position, localSideVelocity);

        _rb.AddForce(-localSideVelocity * _frixionStrength);
    }

    private void AirControls()
    {
        if (_canVrille)
            Vrille();
    }

    private void GroundControls()
    {
        _canVrille = true;

        Vector3 moveDir = (transform.forward * Input.GetAxis("Vertical")).normalized;
        _turnFactor = _turnSpeed / ((_rb.velocity.magnitude * _turnSpeedSlowDownScale) + 1);

        GetUp();
        FrontGetUp();

        if (_rb.velocity.magnitude < _velocityClamp)
            _rb.AddForce(moveDir * _moveSpeed / _speedLower);

        transform.Rotate(0, Input.GetAxis("Horizontal") * _turnFactor, 0);


        SkateFrixion();
    }

    private void GetUp()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (_timer >= 0.08f)
            {
                _rb.AddRelativeTorque(-10, 0, 0);
            }
            else
            {
                _getUpFirstPhase = true;
            }

            if (!_getUpFirstPhase)
            {
                _turnFactor *= 3;
                _timer += Time.deltaTime;
            }
        }
        else if (!_getUpFirstPhase)
        {
            _speedLower = 1;
            _timer = 0;
            _firstTimeGetUp = true;
        }

        if (_getUpFirstPhase)
        {
            if (_timer >= 0.08f)
            {
                if (!_firstTimeGetUp)
                {
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        _rb.angularVelocity = _rb.angularVelocity - (transform.right * transform.InverseTransformDirection(_rb.angularVelocity).x);

                        if (!Input.GetKey(KeyCode.Space))
                        {
                            _rb.AddRelativeTorque(12, 0, 0);
                            _rb.AddForce(0, 30, 0);
                        }
                    }

                    _firstTimeGetUp = true;
                    _getUpFirstPhase = false;
                }
            }
            else
            {
                if (_firstTimeGetUp)
                {
                    _firstTimeGetUp = false;
                    _rb.AddRelativeTorque(-150, 0, 0);
                }
            }

            _turnFactor *= 3;
            _timer += Time.deltaTime;
        }
    }

    private void FrontGetUp()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            if (_timer2 >= 0.08f)
            {
                _rb.AddRelativeTorque(10, 0, 0);
            }
            else
            {
                _backGetUpFirstPhase = true;
            }

            if (!_backGetUpFirstPhase)
            {
                _turnFactor *= 3;
                _timer2 += Time.deltaTime;
            }
        }
        else if (!_backGetUpFirstPhase)
        {
            _speedLower = 1;
            _timer2 = 0;
            _firstTimeBackGetUp = true;
        }

        if (_backGetUpFirstPhase)
        {
            if (_timer2 >= 0.08f)
            {
                if (!_firstTimeBackGetUp)
                {
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        _rb.angularVelocity = _rb.angularVelocity - (transform.right * transform.InverseTransformDirection(_rb.angularVelocity).x);

                        if (!Input.GetKey(KeyCode.Space))
                            _rb.AddRelativeTorque(6, 0, 0);
                    }

                    _firstTimeBackGetUp = true;
                    _backGetUpFirstPhase = false;
                }
            }
            else
            {
                if (_firstTimeBackGetUp)
                {
                    _firstTimeBackGetUp = false;
                    _rb.AddRelativeTorque(150, 0, 0);
                }
            }

            _turnFactor *= 3;
            _timer2 += Time.deltaTime;
        }
    }

    private void Vrille()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            _rb.AddRelativeTorque(0, 0, -_vrilleStrength);
            _canVrille = false;
        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            _rb.AddRelativeTorque(0, 0, _vrilleStrength);
            _canVrille = false;
        }
    }
}
