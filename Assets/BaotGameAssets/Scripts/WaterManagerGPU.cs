using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class WaterManagerGPU : MonoBehaviour
{
    [SerializeField] Material material;
    private MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }

    void Start()
    {
        //CreateMeshLowPoly(meshFilter);
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
        //material.SetFloat("_Amplitude", NetworkWaveMaster.instance.amplitude);
        //material.SetFloat("_Steepness", NetworkWaveMaster.instance.steepness);
        //material.SetFloat("_Wavelength", NetworkWaveMaster.instance.length);
        //material.SetFloat("_Speed", NetworkWaveMaster.instance.speed);

        Vector4[] float4Array;
        float4Array = new Vector4[NetworkWaveMaster.instance.waves.Length];


        for (int i = 0; i < NetworkWaveMaster.instance.waves.Length; i++) 
        {
            NetworkWaveMaster.Wave wave = NetworkWaveMaster.instance.waves[i];
            float4Array[i] = new Vector4(wave.direction.x, wave.direction.y, wave.steepness, wave.length);
        }

        material.SetVectorArray("_Waves", float4Array); 
        material.SetFloat("_ArraySize", float4Array.Length);
        material.SetFloat("_Offset", NetworkWaveMaster.instance.offset);
    }
}
