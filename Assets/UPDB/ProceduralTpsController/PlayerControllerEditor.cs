using UnityEditor;
using UnityEngine;

namespace UPDB.CamerasAndCharacterControllers.CharacterControllers.ProceduralTpsController
{
    [CustomEditor(typeof(PlayerController)), CanEditMultipleObjects]
    public class PlayerControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            PlayerController myTarget = (PlayerController)target;

            //DrawCustomInspector(myTarget);
            DrawDefaultInspector();

            if (!Application.isPlaying)
                myTarget.InitVariables();

            myTarget.Rb.drag = Mathf.Clamp(myTarget.Rb.drag, 0, 50);
        }

        private void DrawCustomInspector(PlayerController myTarget)
        {
            myTarget.Rb = (Rigidbody)EditorGUILayout.ObjectField("RigidBody", myTarget.Rb, typeof(Rigidbody), true);
            myTarget.CameraPivot = (Transform)EditorGUILayout.ObjectField("Camera Pivot", myTarget.CameraPivot, typeof(Transform), true);
            myTarget.Camera = (Camera)EditorGUILayout.ObjectField("Camera", myTarget.Camera, typeof(Camera), true);
            myTarget.Speed = EditorGUILayout.FloatField(new GUIContent("Speed"), myTarget.Speed);

            if (myTarget.Rb != null)
                myTarget.Rb.drag = EditorGUILayout.FloatField(new GUIContent("Drag"), myTarget.Rb.drag);
        }
    }
}
