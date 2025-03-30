using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static WaveManager;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    [Serializable]
    public struct Wave
    {
        public Vector2 direction;
        public float steepness;
        public float length;
        //public float speed;
    };

    public Wave[] waves = {
        new Wave {direction = new Vector2(1,0), steepness = 0.5f, length = 10},
        new Wave {direction = new Vector2(0,1), steepness = 0.2f, length = 20},
        new Wave {direction = new Vector2(1,1), steepness = 0.3f, length = 15},
    };

    public float offset = 0f;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Update()
    {
        offset = Time.time;
    }

    Vector3 GerstnerWave(Wave wave, Vector3 p)
    {
        float steepness = wave.steepness;
        float wavelength = wave.length;
        float k = 2 * Mathf.PI / wavelength;
        float c = Mathf.Sqrt(9.8f / k);
        Vector2 d = wave.direction.normalized;
        float f = k * (Vector2.Dot(d, new Vector2(p.x, p.z)) - c * offset);
        float a = steepness / k;

        // return float3(
        // 	d.x * (a * cos(f)),
        // 	a * sin(f),
        // 	d.y * (a * cos(f))
        // );

        return new Vector3(
             0,
             a * Mathf.Sin(f),
             0
         );
    }

    public float GetWaveHeight(Vector3 pos)
    {
        Vector3 p = Vector3.zero;

        foreach (Wave wave in waves)
        {
            p += GerstnerWave(wave, pos);
        }

        // p.x += d.x * a * cos(f);
        // p.y = a * sin(f);
        // p.z += d.y * (a * cos(f));

        // p.y = _Amplitude * sin(p.x / _Wavelength + _Offset) * cos(p.z / _Wavelength + _Offset);

        return p.y;

        //return amplitude * Mathf.Sin(_x / length + offset) * Mathf.Cos(_z / length + offset);
    }
}
