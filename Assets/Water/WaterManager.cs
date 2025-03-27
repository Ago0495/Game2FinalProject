using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class WaterManager : MonoBehaviour
{
    private MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    void Start()
    {
        CreateMeshLowPoly(meshFilter);
    }
    MeshFilter CreateMeshLowPoly(MeshFilter mf)
    {
        meshFilter.mesh = mf.sharedMesh;

        //Get the original vertices of the gameobject's mesh
        Vector3[] originalVertices = meshFilter.mesh.vertices;

        //Get the list of triangle indices of the gameobject's mesh
        int[] triangles = meshFilter.mesh.triangles;

        //Create a vector array for new vertices 
        Vector3[] vertices = new Vector3[triangles.Length];
        
        //Assign vertices to create triangles out of the mesh
        for (int i = 0; i < triangles.Length; i++)
        {
            vertices[i] = originalVertices[triangles[i]];
            triangles[i] = i;
        }

        //Update the gameobject's mesh with new vertices
        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.SetTriangles(triangles, 0);
        meshFilter.mesh.RecalculateBounds();
        meshFilter.mesh.RecalculateNormals();
        // this.vertices = meshFilter.mesh.vertices;

        return mf;
    }

    private void Update()
    {
        Vector3[] vertices = meshFilter.mesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = WaveManager.instance.GetWaveHeight(new Vector3(transform.position.x + vertices[i].x, 0, transform.position.z + vertices[i].z));
        }

        meshFilter.mesh.vertices = vertices;
        meshFilter.mesh.RecalculateNormals();
    }
}
