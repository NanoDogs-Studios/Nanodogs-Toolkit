using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Nanodogs.Nanofract
{
    public class NanofractWindow : EditorWindow
    {
        public MeshRenderer renderer;

        int fractureCount = 1;

        [MenuItem("Nanodogs/Nanofract/Fracture Mesh", false, 30)]
        public static void ShowWindow()
        {
            GetWindow<NanofractWindow>("Fracture Mesh");
        }

        private void OnGUI()
        {
            GUILayout.Label("FractureObject", EditorStyles.boldLabel);

            renderer = (MeshRenderer)EditorGUILayout.ObjectField(renderer, typeof(MeshRenderer), true);

            fractureCount = (int)EditorGUILayout.Slider(fractureCount, 1, 100);
            if (GUILayout.Button("Fracture Mesh"))
            {
                OutputMesh(renderer, true);
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Output THIS Mesh"))
            {
                OutputMesh(renderer, false);
            }


            GUILayout.Space(50);
            EditorGUILayout.LabelField("Make sure Read/Write is ON..");

            
        }

        private void OutputMesh(MeshRenderer mesh, bool Fracture)
        {
            NanoFract_Function.OutputMesh(mesh, Fracture, fractureCount);
        }
    }
}
