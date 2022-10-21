using System;
using UnityEditor;
using UnityEngine;

namespace UPDB.CamerasAndCharacterControllers.Cameras.SimpleFpsCamera
{
    [CustomEditor(typeof(CameraController)), CanEditMultipleObjects]
    public class CameraControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            CameraController myTarget = (CameraController)target;

            DrawCustomInspector(myTarget);

            if (!Application.isPlaying)
                myTarget.InitVariables();
        }

        private void DrawCustomInspector(CameraController myTarget)
        {
            myTarget.Camera = (Camera)EditorGUILayout.ObjectField("Camera", myTarget.Camera, typeof(Camera), true);
            myTarget.Player = (Transform)EditorGUILayout.ObjectField("Player", myTarget.Player, typeof(Transform), true);
            myTarget.LookSpeed = EditorGUILayout.Vector2Field(new GUIContent("Look Speed"), myTarget.LookSpeed);
        }
    } 
}
