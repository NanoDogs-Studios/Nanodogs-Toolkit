using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace Nanodogs.Nanofract
{
    public class NanoFract_Function : MonoBehaviour
    {
        // Example: NanoFract_Function.OutputMesh(myRenderer, true, 3);
        public static void OutputMesh(MeshRenderer meshRenderer, bool fracture, int fractureCount = 1)
        {
            Mesh mesh = meshRenderer.GetComponent<MeshFilter>().sharedMesh;
            Mesh meshCopy = Object.Instantiate(mesh);
            meshCopy.name = meshRenderer.name + "_FracturedMesh";

            string meshPath = Path.Combine("Assets/Nanodogs/NanoFract", meshCopy.name + ".asset");
            meshPath = AssetDatabase.GenerateUniqueAssetPath(meshPath);

            AssetDatabase.CreateAsset(meshCopy, meshPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (fracture)
            {
                Debug.Log($"Fracturing {mesh.name} into {fractureCount} pieces...");
                FractureMesh(meshCopy, meshRenderer.transform, fractureCount);
            }
        }

        static void FractureMesh(Mesh mesh, Transform sourceTransform, int fractureCount)
        {
            List<Mesh> currentPieces = new List<Mesh> { mesh };

            for (int i = 0; i < fractureCount; i++)
            {
                List<Mesh> newPieces = new List<Mesh>();

                foreach (var piece in currentPieces)
                {
                    Plane plane = new Plane(
                        Random.onUnitSphere,
                        sourceTransform.position + Random.insideUnitSphere * 0.3f
                    );

                    var (above, below) = SliceMesh(piece, plane, sourceTransform);

                    if (above != null) newPieces.Add(above);
                    if (below != null) newPieces.Add(below);
                }

                currentPieces = newPieces;
            }

            string basePath = "Assets/Nanodogs/NanoFract/";
            for (int i = 0; i < currentPieces.Count; i++)
            {
                Mesh m = currentPieces[i];
                m.RecalculateNormals();
                m.RecalculateBounds();
                string path = AssetDatabase.GenerateUniqueAssetPath(basePath + mesh.name + "_Part" + i + ".asset");
                AssetDatabase.CreateAsset(m, path);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"✅ Fracture complete: created {currentPieces.Count} pieces.");
        }

        static (Mesh, Mesh) SliceMesh(Mesh original, Plane plane, Transform transform)
        {
            Vector3[] verts = original.vertices;
            int[] tris = original.triangles;

            List<Vector3> aboveVerts = new List<Vector3>();
            List<int> aboveTris = new List<int>();

            List<Vector3> belowVerts = new List<Vector3>();
            List<int> belowTris = new List<int>();

            for (int i = 0; i < tris.Length; i += 3)
            {
                Vector3 v0 = transform.TransformPoint(verts[tris[i]]);
                Vector3 v1 = transform.TransformPoint(verts[tris[i + 1]]);
                Vector3 v2 = transform.TransformPoint(verts[tris[i + 2]]);

                bool s0 = plane.GetSide(v0);
                bool s1 = plane.GetSide(v1);
                bool s2 = plane.GetSide(v2);

                // Case 1: All vertices on one side
                if (s0 && s1 && s2)
                {
                    AddTriangle(aboveVerts, aboveTris, transform, v0, v1, v2);
                }
                else if (!s0 && !s1 && !s2)
                {
                    AddTriangle(belowVerts, belowTris, transform, v0, v1, v2);
                }
                else
                {
                    // Case 2: The triangle crosses the plane
                    HandleIntersectingTriangle(plane, transform,
                        v0, v1, v2, s0, s1, s2,
                        aboveVerts, aboveTris,
                        belowVerts, belowTris);
                }
            }

            Mesh above = null;
            Mesh below = null;

            if (aboveVerts.Count > 0)
            {
                above = new Mesh();
                above.name = original.name + "_Above";
                above.vertices = aboveVerts.ToArray();
                above.triangles = aboveTris.ToArray();
            }

            if (belowVerts.Count > 0)
            {
                below = new Mesh();
                below.name = original.name + "_Below";
                below.vertices = belowVerts.ToArray();
                below.triangles = belowTris.ToArray();
            }

            return (above, below);
        }

        static void HandleIntersectingTriangle(
            Plane plane, Transform transform,
            Vector3 v0, Vector3 v1, Vector3 v2,
            bool s0, bool s1, bool s2,
            List<Vector3> aboveVerts, List<int> aboveTris,
            List<Vector3> belowVerts, List<int> belowTris)
        {
            Vector3[] verts = { v0, v1, v2 };
            bool[] sides = { s0, s1, s2 };

            List<Vector3> above = new List<Vector3>();
            List<Vector3> below = new List<Vector3>();

            for (int i = 0; i < 3; i++)
            {
                Vector3 a = verts[i];
                Vector3 b = verts[(i + 1) % 3];
                bool sideA = sides[i];
                bool sideB = sides[(i + 1) % 3];

                if (sideA) above.Add(a);
                else below.Add(a);

                if (sideA != sideB)
                {
                    plane.Raycast(new Ray(a, b - a), out float dist);
                    Vector3 intersection = a + (b - a).normalized * dist;
                    above.Add(intersection);
                    below.Add(intersection);
                }
            }

            // Build above triangles
            if (above.Count >= 3)
            {
                for (int i = 1; i < above.Count - 1; i++)
                    AddTriangle(aboveVerts, aboveTris, transform, above[0], above[i], above[i + 1]);
            }

            // Build below triangles
            if (below.Count >= 3)
            {
                for (int i = 1; i < below.Count - 1; i++)
                    AddTriangle(belowVerts, belowTris, transform, below[0], below[i], below[i + 1]);
            }
        }

        static void AddTriangle(List<Vector3> verts, List<int> tris, Transform transform, Vector3 a, Vector3 b, Vector3 c)
        {
            int index = verts.Count;
            verts.Add(transform.InverseTransformPoint(a));
            verts.Add(transform.InverseTransformPoint(b));
            verts.Add(transform.InverseTransformPoint(c));
            tris.AddRange(new int[] { index, index + 1, index + 2 });
        }
    }
}
