using UnityEngine;

namespace UPDB.CamerasAndCharacterControllers.Cameras.TpsCamera
{
    /// <summary>
    /// simple tps camera controller, can be used with fps controller or alone
    /// </summary>
    [AddComponentMenu("UPDB/CamerasAndCharacterControllers/Cameras/TpsCamera/Tps Camera Controller")]
    public class CameraController : MonoBehaviour
    {
        [SerializeField, Tooltip("Camera pivot linked to this Player(where you have to put camera script)")]
        private Transform _cameraPivot;

        [SerializeField, Tooltip("speed of mouse look in X and Y")]
        private Vector2 _lookSpeed = Vector2.one;

        [SerializeField]
        private Transform _target;

        [SerializeField]
        private float _translationSpeed = 20;

        private Vector2 _rotation = Vector2.zero;

        private bool _lastInput = false;

        private float _timer = 0f;

        private Vector3 _memoTagretSkatePos;

        private bool _isCameraManual = true;


        public Vector2 LookSpeed
        {
            get { return _lookSpeed; }
            set { _lookSpeed = value; }
        }

        public Transform CameraPivot
        {
            get { return _cameraPivot; }
            set { _cameraPivot = value; }
        }

        private void Awake()
        {
            InitVariables();
        }

        private void Update()
        {
            Look();
        }

        public void InitVariables()
        {
            if (_cameraPivot == null)
                _cameraPivot = transform;
        }

        private void Look()
        {
            if (_isCameraManual)
                MouseControl();
            else
                CameraAuto();

            if (Input.GetKeyDown(KeyCode.C))
                _isCameraManual = !_isCameraManual;
        }

        private void MouseControl()
        {
            Vector2 mouse = new Vector2(Input.GetAxis("Mouse X") * _lookSpeed.x, Input.GetAxis("Mouse Y") * _lookSpeed.y);
            _rotation += new Vector2(-mouse.y, mouse.x);

            _rotation.x = Mathf.Clamp(_rotation.x, -89, 89);

            _cameraPivot.eulerAngles = new Vector3(_rotation.x, _rotation.y, 0.0f);
        }

        private void CameraAuto()
        {
            Transform player = GameObject.FindWithTag("Player").transform;
            bool _lastInputMemo = _lastInput;

            if (player.gameObject.GetComponent<PlayerController>().IsGrounded)
            {
                if (Input.GetAxis("Vertical") > 0)
                {
                    _target.Translate((new Vector3(transform.position.x + player.forward.x, player.position.y - 0.3f, transform.position.z + player.forward.z) - _target.position) / _translationSpeed, Space.World);
                    _lastInput = true;
                }
                else if (Input.GetAxis("Vertical") < 0)
                {
                    _target.Translate((new Vector3(transform.position.x - player.forward.x, player.position.y - 0.3f, transform.position.z - player.forward.z) - _target.position) / _translationSpeed, Space.World);
                    _lastInput = false;
                }
                else if (Input.GetAxis("Vertical") == 0)
                {
                    if (_lastInput)
                        _target.Translate((new Vector3(transform.position.x + player.forward.x, player.position.y - 0.3f, transform.position.z + player.forward.z) - _target.position) / _translationSpeed, Space.World);
                    else
                        _target.Translate((new Vector3(transform.position.x - player.forward.x, player.position.y - 0.3f, transform.position.z - player.forward.z) - _target.position) / _translationSpeed, Space.World);
                }

                if (_timer < 0.1f * _translationSpeed)
                    _target.localPosition = new Vector3(-Mathf.Pow(_target.localPosition.z, 2) + 1, _target.localPosition.y, _target.localPosition.z);
                else
                {
                    Transform playerYRotOnly = new GameObject("playerYRotOnly").transform;
                    playerYRotOnly.position = player.position;
                    playerYRotOnly.eulerAngles = new Vector3(0, player.eulerAngles.y, 0);
                    playerYRotOnly.localScale = player.localScale;

                    Vector3 localTargetPos = playerYRotOnly.InverseTransformPoint(_target.position);
                    _target.position = playerYRotOnly.TransformPoint(new Vector3(0, localTargetPos.y, localTargetPos.z));

                    Destroy(playerYRotOnly.gameObject);
                }

                _target.position = new Vector3(_target.position.x, player.position.y - 0.3f, _target.position.z);

                _cameraPivot.LookAt(_target.position);

                _timer += Time.deltaTime;

                if (_lastInput != _lastInputMemo)
                    _timer = 0;

                _memoTagretSkatePos = _target.position - player.position;
            }
            else
            {
                _target.position = player.position + _memoTagretSkatePos;
                _cameraPivot.LookAt(_target.position);
            }
        }
    }
}
