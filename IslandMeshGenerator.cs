using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class IslandLevelData
{
    public float spacing = 5f;
    public float height = 5f;
    public float maxVarianceInSpacing = 1f;
    public float maxVarianceInHeight = 0f;
    public bool duplicateAboveVertices = false;
}

[RequireComponent(typeof(MeshFilter)), RequireComponent(typeof(MeshRenderer))]
public class IslandMeshGenerator : MonoBehaviour
{
    public bool generateDebugGizmos = true;

    [Header("Levels Settings")]
    public IslandLevelData[] levels = new IslandLevelData[1] { new IslandLevelData() };

    [Header("Mesh Settings")]
    public int resolution = 4;

    [SerializeField] Vector3[] vertices;
    [SerializeField] int[] triangles;
    [SerializeField] Vector2[] uvs;
    [SerializeField] Vector2[] topFaceVertices;

    MeshCollider meshCollider;

    void Awake()
    {
        GetTopFaceVertices(true);
        TryGetComponent<MeshCollider>(out meshCollider);
    }

    public void GenerateIsland()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        GenerateVertices();
        GenerateTriangles();
        GenerateUVs();
        GetTopFaceVertices(true);

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();

        TryGetComponent<MeshCollider>(out meshCollider);
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = true;
    }

    void GenerateVertices()
    {
        // * +2 because we need to add the middle/average of top and bottom vertices to create their faces.
        vertices = new Vector3[(resolution * levels.Length) + 2];

        float angle;
        float radius;
        int vertexIndex = 0;

        float cumulativeXTop = 0f, cumulativeZTop = 0f;
        float cumulativeXBottom = 0f, cumulativeZBottom = 0f;

        for (int i = 0; i < levels.Length; i++)
        {
            radius = levels[i].spacing;
            for (int j = 0; j < resolution; j++)
            {
                float x = 0, z = 0;
                if (levels[i].duplicateAboveVertices && i > 0)
                {
                    x = vertices[(resolution * (i - 1)) + j].x;
                    z = vertices[(resolution * (i - 1)) + j].z;
                }
                else
                {
                    angle = j * (360f / resolution);

                    // Convert polar coordinates (radius, angle) to Cartesian coordinates (x, z)
                    x = radius * Mathf.Cos(angle * Mathf.Deg2Rad) + Random.Range(-levels[i].maxVarianceInSpacing, levels[i].maxVarianceInSpacing);
                    z = radius * Mathf.Sin(angle * Mathf.Deg2Rad) + Random.Range(-levels[i].maxVarianceInSpacing, levels[i].maxVarianceInSpacing);
                }
                if (i == 0)
                {
                    cumulativeXTop += x;
                    cumulativeZTop += z;
                }
                else if (i == levels.Length - 1)
                {
                    cumulativeXBottom += x;
                    cumulativeZBottom += z;
                }

                vertices[vertexIndex] = new Vector3(x, levels[i].height, z);
                vertexIndex++;
            }
        }
        vertices[^2] = new Vector3(cumulativeXTop / resolution, levels[0].height, cumulativeZTop / resolution);
        vertices[^1] = new Vector3(cumulativeXBottom / resolution, levels[^1].height, cumulativeZBottom / resolution);
    }

    void GenerateTriangles()
    {
        triangles = new int[(resolution * levels.Length * 3 * 2)];

        int vertexIndex = 0;

        // Top Face.
        for (int i = 0; i < resolution; i++)
        {
            triangles[vertexIndex] = vertices.Length - 2;
            triangles[vertexIndex + 1] = (i + 1) % resolution;
            triangles[vertexIndex + 2] = i;
            vertexIndex += 3;
        }

        // Side faces.
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < levels.Length - 1; j++)
            {
                triangles[vertexIndex] = i + j * resolution;
                triangles[vertexIndex + 1] = (i + 1) % resolution + j * resolution;
                triangles[vertexIndex + 2] = i + (j + 1) * resolution;
                triangles[vertexIndex + 3] = (i + 1) % resolution + j * resolution;
                triangles[vertexIndex + 4] = (i + 1) % resolution + (j + 1) * resolution;
                triangles[vertexIndex + 5] = i + (j + 1) * resolution;
                vertexIndex += 6;
            }
        }

        // Bottom Face.
        for (int i = 0; i < resolution; i++)
        {
            triangles[vertexIndex] = i + (levels.Length - 1) * resolution;
            triangles[vertexIndex + 1] = (i + 1) % resolution + (levels.Length - 1) * resolution;
            triangles[vertexIndex + 2] = vertices.Length - 1;
            vertexIndex += 3;
        }
    }

    void GenerateUVs()
    {
        uvs = new Vector2[vertices.Length];

        for (int i = 0; i < vertices.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        // Top face.
        for (int i = 0; i < resolution; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        // Side faces.
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < levels.Length - 1; j++)
            {
                uvs[i + j * resolution] = new Vector2(vertices[i + j * resolution].x, vertices[i + j * resolution].z);
            }
        }

        // Bottom face.
        for (int i = 0; i < resolution; i++)
        {
            uvs[i + (levels.Length - 1) * resolution] = new Vector2(vertices[i + (levels.Length - 1) * resolution].x, vertices[i + (levels.Length - 1) * resolution].z);
        }
    }

    void OnDrawGizmos()
    {
        if (!generateDebugGizmos)
        {
            return;
        }

        if (vertices == null)
        {
            return;
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawWireSphere(transform.position + vertices[i], 0.1f);
        }
        Gizmos.color = Color.white;
    }

    public Vector2[] GetTopFaceVertices(bool forceRecalculate = false)
    {
        if (forceRecalculate || topFaceVertices == null || topFaceVertices.Length == 0)
        {
            topFaceVertices = new Vector2[resolution];
            for (int i = 0; i < resolution; i++)
            {
                float adjustedXPosition = vertices[i].x >= 0 ? vertices[i].x - 1 : vertices[i].x + 1;
                float adjustedZPosition = vertices[i].z >= 0 ? vertices[i].z - 1 : vertices[i].z + 1;
                topFaceVertices[i] = new Vector2(adjustedXPosition, adjustedZPosition);
            }
        }
        return topFaceVertices;
    }
}
