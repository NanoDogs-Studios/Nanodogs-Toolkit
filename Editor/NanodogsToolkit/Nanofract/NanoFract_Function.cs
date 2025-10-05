using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace Nanodogs.Nanofract
{
    public class NanoFract_Function : MonoBehaviour
    {
        // ... (OutputMesh and FractureMesh remain the same) ...

        public static void OutputMesh(MeshRenderer meshRenderer, bool fracture, int fractureCount = 1)
        {
            if (meshRenderer == null) return;

            MeshFilter mf = meshRenderer.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null)
            {
                Debug.LogWarning("MeshFilter or mesh missing!");
                return;
            }

            Mesh meshCopy = Instantiate(mf.sharedMesh);
            meshCopy.name = meshRenderer.name + "_FracturedMesh";

            string meshPath = Path.Combine("Assets/Nanodogs/NanoFract", meshCopy.name + ".asset");
            meshPath = AssetDatabase.GenerateUniqueAssetPath(meshPath);

            AssetDatabase.CreateAsset(meshCopy, meshPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (fracture)
            {
                Debug.Log($"Fracturing {meshCopy.name} into {fractureCount} pieces...");
                FractureMesh(meshCopy, meshRenderer.transform, fractureCount);
            }
        }

        static void FractureMesh(Mesh mesh, Transform sourceTransform, int fractureCount)
        {
            List<Mesh> pieces = new List<Mesh> { mesh };

            for (int f = 0; f < fractureCount; f++)
            {
                List<Mesh> newPieces = new List<Mesh>();
                foreach (Mesh piece in pieces)
                {
                    Plane plane = new Plane(Random.onUnitSphere, sourceTransform.position + Random.insideUnitSphere * 0.5f);
                    var split = SliceAndCap(piece, plane, sourceTransform);
                    if (split.Item1 != null) newPieces.Add(split.Item1);
                    if (split.Item2 != null) newPieces.Add(split.Item2);
                }
                pieces = newPieces;
            }

            string basePath = "Assets/Nanodogs/NanoFract/";
            for (int i = 0; i < pieces.Count; i++)
            {
                Mesh m = pieces[i];
                m.RecalculateNormals();
                m.RecalculateBounds();
                // Optionally Recalculate Tangents if needed: m.RecalculateTangents();
                string path = AssetDatabase.GenerateUniqueAssetPath(basePath + mesh.name + "_Part" + i + ".asset");
                AssetDatabase.CreateAsset(m, path);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"✅ Fracture complete: created {pieces.Count} pieces.");
        }

        // --- Slice and Cap Mesh ---
        static (Mesh, Mesh) SliceAndCap(Mesh mesh, Plane plane, Transform transform)
        {
            Vector3[] vertices = mesh.vertices;
            int[] triangles = mesh.triangles;
            Vector2[] uvs = mesh.uv; // Retrieve original UVs

            List<Vector3> aboveVerts = new List<Vector3>();
            List<int> aboveTris = new List<int>();
            List<Vector2> aboveUVs = new List<Vector2>(); // New: UV list

            List<Vector3> belowVerts = new List<Vector3>();
            List<int> belowTris = new List<int>();
            List<Vector2> belowUVs = new List<Vector2>(); // New: UV list

            // Store cap edges and their UVs
            List<Vector3> capEdges = new List<Vector3>();
            List<Vector2> capUVs = new List<Vector2>(); // New: Cap UV list

            for (int i = 0; i < triangles.Length; i += 3)
            {
                // Original vertex positions (in local space)
                Vector3 v0_l = vertices[triangles[i]];
                Vector3 v1_l = vertices[triangles[i + 1]];
                Vector3 v2_l = vertices[triangles[i + 2]];

                // UVs
                Vector2 uv0 = uvs.Length > 0 ? uvs[triangles[i]] : Vector2.zero;
                Vector2 uv1 = uvs.Length > 0 ? uvs[triangles[i + 1]] : Vector2.zero;
                Vector2 uv2 = uvs.Length > 0 ? uvs[triangles[i + 2]] : Vector2.zero;

                // World positions for slicing
                Vector3 v0_w = transform.TransformPoint(v0_l);
                Vector3 v1_w = transform.TransformPoint(v1_l);
                Vector3 v2_w = transform.TransformPoint(v2_l);

                bool s0 = plane.GetSide(v0_w);
                bool s1 = plane.GetSide(v1_w);
                bool s2 = plane.GetSide(v2_w);

                if (s0 && s1 && s2) AddTriangle(aboveVerts, aboveTris, aboveUVs, transform, v0_w, v1_w, v2_w, uv0, uv1, uv2);
                else if (!s0 && !s1 && !s2) AddTriangle(belowVerts, belowTris, belowUVs, transform, v0_w, v1_w, v2_w, uv0, uv1, uv2);
                else
                {
                    // Split triangle and store cap points and interpolated UVs
                    SplitTriangleWithCap(v0_w, v1_w, v2_w, uv0, uv1, uv2, s0, s1, s2, plane,
                                         aboveVerts, aboveTris, aboveUVs,
                                         belowVerts, belowTris, belowUVs,
                                         capEdges, capUVs, transform);
                }
            }

            // Triangulate cap to close the mesh
            if (capEdges.Count >= 3)
            {
                CreateCap(plane, capEdges, capUVs, aboveVerts, aboveTris, aboveUVs, belowVerts, belowTris, belowUVs, transform);
            }

            Mesh aboveMesh = null;
            Mesh belowMesh = null;

            if (aboveVerts.Count > 0)
            {
                aboveMesh = new Mesh { name = mesh.name + "_Above" };
                aboveMesh.vertices = aboveVerts.ToArray();
                aboveMesh.triangles = aboveTris.ToArray();
                aboveMesh.uv = aboveUVs.ToArray(); // Assign UVs
            }

            if (belowVerts.Count > 0)
            {
                belowMesh = new Mesh { name = mesh.name + "_Below" };
                belowMesh.vertices = belowVerts.ToArray();
                belowMesh.triangles = belowTris.ToArray();
                belowMesh.uv = belowUVs.ToArray(); // Assign UVs
            }

            return (aboveMesh, belowMesh);
        }

        static void SplitTriangleWithCap(Vector3 v0, Vector3 v1, Vector3 v2,
                                         Vector2 uv0, Vector2 uv1, Vector2 uv2,
                                         bool s0, bool s1, bool s2,
                                         Plane plane,
                                         List<Vector3> aboveVerts, List<int> aboveTris, List<Vector2> aboveUVs,
                                         List<Vector3> belowVerts, List<int> belowTris, List<Vector2> belowUVs,
                                         List<Vector3> capEdges, List<Vector2> capUVs, Transform transform)
        {
            Vector3[] verts = { v0, v1, v2 };
            Vector2[] uvs = { uv0, uv1, uv2 };
            bool[] sides = { s0, s1, s2 };

            List<Vector3> above = new List<Vector3>();
            List<Vector3> below = new List<Vector3>();
            List<Vector2> above_uv = new List<Vector2>();
            List<Vector2> below_uv = new List<Vector2>();

            for (int i = 0; i < 3; i++)
            {
                Vector3 a = verts[i];
                Vector3 b = verts[(i + 1) % 3];
                Vector2 uva = uvs[i];
                Vector2 uvb = uvs[(i + 1) % 3];

                bool sideA = sides[i];
                bool sideB = sides[(i + 1) % 3];

                if (sideA)
                {
                    above.Add(a);
                    above_uv.Add(uva);
                }
                else
                {
                    below.Add(a);
                    below_uv.Add(uva);
                }

                if (sideA != sideB)
                {
                    plane.Raycast(new Ray(a, b - a), out float dist);
                    Vector3 intersection = a + (b - a).normalized * dist;
                    float t = dist / (b - a).magnitude; // Interpolation factor
                    Vector2 intersection_uv = Vector2.Lerp(uva, uvb, t); // Interpolate UV

                    above.Add(intersection);
                    above_uv.Add(intersection_uv);
                    below.Add(intersection);
                    below_uv.Add(intersection_uv);
                    capEdges.Add(intersection);
                    capUVs.Add(intersection_uv); // Store cap UV
                }
            }

            // Re-triangulate and add to mesh lists
            // ABOVE
            if (above.Count >= 3)
                for (int i = 1; i < above.Count - 1; i++)
                    AddTriangle(aboveVerts, aboveTris, aboveUVs, transform, above[0], above[i], above[i + 1], above_uv[0], above_uv[i], above_uv[i + 1]);

            // BELOW
            if (below.Count >= 3)
                for (int i = 1; i < below.Count - 1; i++)
                    AddTriangle(belowVerts, belowTris, belowUVs, transform, below[0], below[i], below[i + 1], below_uv[0], below_uv[i], below_uv[i + 1]);
        }

        static void CreateCap(Plane plane, List<Vector3> capVerts, List<Vector2> capUVs,
            List<Vector3> aboveVerts, List<int> aboveTris, List<Vector2> aboveUVs,
            List<Vector3> belowVerts, List<int> belowTris, List<Vector2> belowUVs,
            Transform transform)
        {
            // Compute center (in world space)
            Vector3 center = Vector3.zero;
            foreach (var v in capVerts) center += v;
            center /= capVerts.Count;

            // Compute center UV for the cap. This UV will be a new coordinate specific to the cap.
            // We use a planar projection based on the plane normal.
            Vector3 planeNormal = plane.normal;
            Vector3 uAxis = Vector3.Cross(planeNormal, Vector3.up).normalized;
            if (uAxis.sqrMagnitude < 0.001f) uAxis = Vector3.Cross(planeNormal, Vector3.right).normalized;
            Vector3 vAxis = Vector3.Cross(planeNormal, uAxis).normalized;

            Vector2 centerUV = Vector2.zero; // The UV for the center vertex (a) in the triangulation

            // Sort points for consistent triangulation (copied from previous code)
            // Note: We use the already stored capUVs list in the loops below
            capVerts.Sort((a_v, b_v) =>
            {
                Vector3 dirA = a_v - center;
                Vector3 dirB = b_v - center;
                float angleA = Mathf.Atan2(Vector3.Dot(Vector3.Cross(planeNormal, Vector3.right), dirA), Vector3.Dot(Vector3.right, dirA));
                float angleB = Mathf.Atan2(Vector3.Dot(Vector3.Cross(planeNormal, Vector3.right), dirB), Vector3.Dot(Vector3.right, dirB));
                return angleA.CompareTo(angleB);
            });

            // Re-sort the capUVs list to match the sorted capVerts list's order
            // This requires a more complex sorting setup or, preferably, sorting an intermediate list
            // of (Vector3, Vector2) pairs, but for simplicity, we'll assume the original capVerts/capUVs
            // lists were parallelly populated based on edge traversal, and the sort only needs to happen 
            // if we are guaranteeing a closed loop. Since the original code relies on this Vector3 sort, 
            // the UVs may now be out of sync.

            // To ensure capVerts and capUVs are sorted in parallel, we'll use a temporary list of tuples.
            List<(Vector3 vertex, Vector2 uv)> capData = new List<(Vector3, Vector2)>();
            for (int i = 0; i < capVerts.Count; i++) capData.Add((capVerts[i], capUVs[i]));

            capData.Sort((a, b) =>
            {
                Vector3 dirA = a.vertex - center;
                Vector3 dirB = b.vertex - center;
                float angleA = Mathf.Atan2(Vector3.Dot(Vector3.Cross(planeNormal, Vector3.right), dirA), Vector3.Dot(Vector3.right, dirA));
                float angleB = Mathf.Atan2(Vector3.Dot(Vector3.Cross(planeNormal, Vector3.right), dirB), Vector3.Dot(Vector3.right, dirB));
                return angleA.CompareTo(angleB);
            });

            // Now capData is sorted by vertex position, so we iterate on it.
            for (int i = 0; i < capData.Count; i++)
            {
                Vector3 a = center;
                Vector3 b = capData[i].vertex;
                Vector3 c = capData[(i + 1) % capData.Count].vertex;

                // Calculate planar UVs for B and C
                Vector2 uv_b = new Vector2(Vector3.Dot(b - center, uAxis), Vector3.Dot(b - center, vAxis));
                Vector2 uv_c = new Vector2(Vector3.Dot(c - center, uAxis), Vector3.Dot(c - center, vAxis));

                // 1. Cap for the "Above" side. (Flipped winding for outward normal)
                AddTriangle(aboveVerts, aboveTris, aboveUVs, transform, a, c, b, centerUV, uv_c, uv_b);

                // 2. Cap for the "Below" side. (Standard winding for outward normal)
                AddTriangle(belowVerts, belowTris, belowUVs, transform, a, b, c, centerUV, uv_b, uv_c);
            }
        }

        static void AddTriangle(List<Vector3> verts, List<int> tris, List<Vector2> uvs, Transform transform, Vector3 a, Vector3 b, Vector3 c, Vector2 uva, Vector2 uvb, Vector2 uvc)
        {
            int index = verts.Count;
            verts.Add(transform.InverseTransformPoint(a));
            verts.Add(transform.InverseTransformPoint(b));
            verts.Add(transform.InverseTransformPoint(c));

            uvs.Add(uva);
            uvs.Add(uvb);
            uvs.Add(uvc);

            tris.Add(index);
            tris.Add(index + 1);
            tris.Add(index + 2);
        }
    }
}