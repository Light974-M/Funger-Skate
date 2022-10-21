using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UPDB.CamerasAndCharacterControllers.Cameras.SimpleFpsCamera
{
    /// <summary>
    /// simple fps camera controller, can be used with fps controller or alone
    /// </summary>
    [HelpURL(URL.baseURL + "/tree/main/CamerasAndCharacterControllers/Cameras/SimpleFpsCamera/README.md"), AddComponentMenu("UPDB/CamerasAndCharacterControllers/Cameras/SimpleFpsCamera/Fps Camera Controller")]
    public class CameraController : MonoBehaviour
    {
        [SerializeField, Tooltip("camera used to render fps game view")]
        private Camera _camera;

        [SerializeField, Tooltip("player linked to this camera")]
        private Transform _player;

        [SerializeField, Tooltip("speed of mouse look in X and Y")]
        private Vector2 _lookSpeed = Vector2.one;


        private Vector2 _rotation = Vector2.zero;


        public Vector2 LookSpeed
        {
            get { return _lookSpeed; }
            set { _lookSpeed = value; }
        }

        public Camera Camera
        {
            get { return _camera; }
            set { _camera = value; }
        }

        public Transform Player
        {
            get { return _player; }
            set { _player = value; }
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
            if (_camera == null)
                if (!TryGetComponent(out _camera))
                    _camera = gameObject.AddComponent<Camera>();

            if (_player == null)
                if (transform.parent == null)
                    _player = transform;
                else
                    _player = transform.parent;
        }

        private void Look()
        {
            Vector2 mouse = new Vector2(Input.GetAxis("Mouse X") * _lookSpeed.x, Input.GetAxis("Mouse Y") * _lookSpeed.y);
            _rotation += new Vector2(-mouse.y, mouse.x);

            _rotation.x = Mathf.Clamp(_rotation.x, -90, 90);

            _player.eulerAngles = new Vector3(0.0f, _rotation.y, 0.0f);
            _camera.transform.eulerAngles = new Vector3(_rotation.x, _camera.transform.eulerAngles.y, 0.0f);
        }
    } 
}
