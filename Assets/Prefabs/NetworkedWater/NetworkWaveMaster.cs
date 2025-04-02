using UnityEngine;
using NETWORK_ENGINE;
using System.Collections;
using System;
using System.Text.RegularExpressions;

public class NetworkWaveMaster : NetworkComponent
{
    public static NetworkWaveMaster instance;

    [Serializable] 
    public struct Wave
    {
        public Vector2 direction;
        public float steepness;
        public float length;
        //public float speed;
    };

    //sync vals
    public Wave[] waves = {
        new Wave {direction = new Vector2(1,0), steepness = 0.5f, length = 10},
        new Wave {direction = new Vector2(0,1), steepness = 0.2f, length = 20},
        new Wave {direction = new Vector2(1,1), steepness = 0.3f, length = 15},
    };

    public float offset = 0f;

    //non sync vals

    public Vector2 Vector2FromString(string s)
    {
        //"(X,Y)"
        string[] args = s.Trim().Trim('(').Trim(')').Split(',');

        return new Vector2(
            float.Parse(args[0]),
            float.Parse(args[1])
            );
    }

    public override void HandleMessage(string flag, string value)
    {
        if (flag == "SETWAVE")
        {
            if (IsClient)
            {
                MatchCollection valueCollection = Regex.Matches(value, "\\[(.*?)\\]");

                Array.Clear(waves, 0, waves.Length);

                int i = 0;
                foreach (Match m in valueCollection)
                {
                    string[] tempValues = m.Groups[1].Value.Split(",");

                    Vector2 tempDirection = Vector2FromString(tempValues[0] + "," + tempValues[1]);
                    float tempSteepness = float.Parse(tempValues[2]);
                    float tempLength = float.Parse(tempValues[3]);

                    waves[i] = new Wave { 
                        direction = tempDirection, 
                        steepness = tempSteepness, 
                        length = tempLength
                    };
                    i++;
                }
            }
        }

        if (flag == "OFFSET")
        {
            if (IsClient)
            {
                offset = float.Parse(value);
            }
        }
    }

    public override void NetworkedStart()
    {

    }

    public override IEnumerator SlowUpdate()
    {
        while (IsServer)
        {
            if (IsDirty)
            {
                string waveValue = "{";

                //waveValue += waves.Length.ToString() + ",{";


                for (int i = 0; i < waves.Length; i++)
                {
                    waveValue += "[";
                    waveValue += waves[i].direction.ToString() + ",";
                    waveValue += waves[i].steepness.ToString() + ",";
                    waveValue += waves[i].length.ToString();
                    waveValue += "]";
                    if (i < waves.Length - 1)
                    {
                        waveValue += ",";
                    }
                }

                waveValue += "}";

                SendUpdate("SETWAVE", waveValue);
                SendUpdate("OFFSET", offset.ToString());
                IsDirty = false;
            }
            yield return new WaitForSeconds(MyCore.MasterTimer);
        }
    }

    void Start()
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


    void Update()
    {
        offset += Time.deltaTime;
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
