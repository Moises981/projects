using RosSharp.RosBridgeClient;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Lidar : MonoBehaviour
{

    public float angle_max = 10;
    public float angle_min = -10;
    public int numberOfLayers = 16;
    public int update_rate = 1800;
    public int samples = 360;
    public float range_max = 12f;
    public float range_min = 0.4f;
    public float Lidar_Angle = 180f;
    public float[] intensities;
    public float Frequency = 5;
    public float time_increment = 0;
    float vertIncrement;
    public float angle_increment;
    public Vector3 Direction_Lidar;
    private LaserScanVisualizer[] laserScanVisualizers;


    //[HideInInspector]
    public float[] ranges;
    public Vector3[] directions;
    public List<Vector3> points;
    [HideInInspector]
    public float[] azimuts;


    // Use this for initialization
    void Start()
    {
        ranges = new float[numberOfLayers *samples];
        azimuts = new float[samples];
        directions = new Vector3[numberOfLayers * samples];
        intensities = new float[numberOfLayers * samples];
        vertIncrement = (float)(angle_max - angle_min) / (float)(numberOfLayers - 1);
        angle_increment = (float)(Lidar_Angle /samples);
        time_increment = (float)(((1/Frequency)/(samples))*1000); //In ms
    }

    public float[] Scan()
    {
        MeasureDistance();

        laserScanVisualizers = GetComponents<LaserScanVisualizer>();
        if (laserScanVisualizers != null)
            foreach (LaserScanVisualizer laserScanVisualizer in laserScanVisualizers)
                laserScanVisualizer.SetSensorData(gameObject.transform, directions, ranges, range_min, range_max);
        
        return Get_array();
    }

    float[] Get_array()
    {
        float[] Data = new float[points.Count*3];
        int i = 0;
        foreach(var point in points)
        {
            Data[i] = point.z;
            Data[i+1] = - point.x;
            Data[i+2] = point.y;
            i += 3;
        }
        return Data;
    }

    // Update is called once per frame
    private void MeasureDistance()
    {
        ranges = new float[samples*numberOfLayers];
        directions = new Vector3[samples*numberOfLayers];
        points.Clear();
        Vector3 fwd = Direction_Lidar;
        Vector3 dir;
        RaycastHit hit;
        int indx = 0;
        float angle;

        //azimut angles
        for (int incr = 0; incr <samples; incr++)
        {
            for (int layer = 0; layer < numberOfLayers; layer++)
            {
                //print("incr "+ incr +" layer "+layer+"\n");
                indx = layer + incr * numberOfLayers;
                angle = angle_min + (float)layer * vertIncrement;
                azimuts[incr] = incr * angle_increment;
                dir = transform.rotation * Quaternion.Euler(-angle, azimuts[incr], 0) * fwd;
                directions[indx] = dir;
                // print("idx "+ indx +" angle " + angle + "  azimut " + azimut + " quats " + Quaternion.Euler(-angle, azimut, 0) + " dir " + dir+ " fwd " + fwd+"\n");
                Debug.DrawRay(transform.position, dir * range_max , Color.green);

                Debug.Log("Medir");
                if (Physics.Raycast(transform.position, dir, out hit, range_max))
                {
                    if (hit.distance >= range_min && hit.distance <= range_max)
                    {
                            ranges[indx] = (float)hit.distance;
                            points.Add(dir*hit.distance);
                    }
                }
            }
        }

    }
}