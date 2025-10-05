// © 2025 Nanodogs Studios. All rights reserved.

using System.Collections.Generic;
using System.Net;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;

namespace Nanodogs.Toolkit
{
    public class NanodogsToolkitImporter
    {
        public static string baseURL = "Assets/Plugins/Nanodogs-Toolkit/Editor/NanodogsToolkit/Import/";

        [MenuItem("Nanodogs/Importer/Import Modular FPS Package")]
        private static void importModularFPS()
        {
            string package = baseURL + "Modular First Person Controller.unitypackage"; // I wanted to leave out specifics, so this is just a sample link.
            AssetDatabase.ImportPackage(package, false);
        }
        [MenuItem("Nanodogs/Importer/Import Mirror")]
        private static void importMirror()
        {
            string package = baseURL + "Mirror.unitypackage"; // I wanted to leave out specifics, so this is just a sample link.
            AssetDatabase.ImportPackage(package, false);
        }
        [MenuItem("Nanodogs/Importer/Import LowPoly Water")]
        private static void importLowPolyWater()
        {
            string package = baseURL + "Assets/Editor/Import/LowPoly Water.unitypackage"; // I wanted to leave out specifics, so this is just a sample link.
            AssetDatabase.ImportPackage(package, false);
        }
        [MenuItem("Nanodogs/Importer/Import Mesh Combiner")]
        private static void importMeshCombiner()
        {
            string package = baseURL + "Assets/Editor/Import/Mesh Combiner.unitypackage"; // I wanted to leave out specifics, so this is just a sample link.
            AssetDatabase.ImportPackage(package, false);
        }

        [MenuItem("Nanodogs/Importer/Import Volumetric Light Beam")]
        private static void importNewInput()
        {
            string package = baseURL + "Assets/Editor/Import/VolumetricLightBeam.unitypackage"; // I wanted to leave out specifics, so this is just a sample link.
            AssetDatabase.ImportPackage(package, false);
        }
    }
}
