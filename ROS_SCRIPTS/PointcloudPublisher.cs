using System;
using System.Collections;
using System.Collections.Generic;
using RosSharp.RosBridgeClient.Messages.Sensor;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;


namespace RosSharp.RosBridgeClient
{
    [RequireComponent(typeof(RosConnector))]
    public class PointcloudPublisher : Publisher<Messages.Sensor.PointCloud2>
    {
        public Lidar lidar;
        
        private Messages.Sensor.PointCloud2 pcd_message;

        private byte[] byteArray;
        private bool isMessageReceived = false;
        bool readyToProcessMessage = true;
        private int size;

        private Vector3[] pcl;
        private Color[] pcl_color;

        int width;
        int height;
        int row_step;
        int point_step;

        protected override void Start()
        {
            base.Start();
            //InitializeMessage();

        }

        public void Update()
        {
            var points = lidar.Scan();
            
            var pcd = new Messages.Sensor.PointCloud2();
                        // create a byte array and copy the floats into it...
            var byteArray = new byte[points.Length * 4];
            Buffer.BlockCopy(points, 0, byteArray, 0, byteArray.Length);
            
            var point_x = new PointField { datatype = PointField.FLOAT32, name = "x", offset = 0, count = 1 };
            var point_y = new PointField { datatype = PointField.FLOAT32, name = "y", offset = 4, count = 1 };
            var point_z = new PointField { datatype = PointField.FLOAT32, name = "z", offset = 8, count = 1 };

            
            pcd.header.frame_id = "M8_Fix_Reference";
            pcd.data = byteArray;
            pcd.fields = new PointField[] { point_x,point_y,point_z} ;
            pcd.is_dense = true;
            pcd.is_bigendian = false;
            pcd.width = (uint)(int)byteArray.Length / 12;
            pcd.height = 1;
            pcd.point_step = 12;
            pcd.row_step = pcd.point_step * pcd.width;
           
            Publish(pcd);
           
        }





        /*
        protected override void ReceiveMessage(PointCloud2 message)
        {


            size = message.data.GetLength(0);

            byteArray = new byte[size];
            byteArray = message.data;


            width = (int)message.width;
            height = (int)message.height;
            row_step = (int)message.row_step;
            point_step = (int)message.point_step;

            size = size / point_step;
            isMessageReceived = true;
        }

        //点群の座標を変換
        void PointCloudRendering()
        {
            pcl = new Vector3[size];
            pcl_color = new Color[size];

            int x_posi;
            int y_posi;
            int z_posi;

            float x;
            float y;
            float z;

            int rgb_posi;
            int rgb_max = 255;

            float r;
            float g;
            float b;

            //この部分でbyte型をfloatに変換         
            for (int n = 0; n < size; n++)
            {
                x_posi = n * point_step + 0;
                y_posi = n * point_step + 4;
                z_posi = n * point_step + 8;

                x = BitConverter.ToSingle(byteArray, x_posi);
                y = BitConverter.ToSingle(byteArray, y_posi);
                z = BitConverter.ToSingle(byteArray, z_posi);


                rgb_posi = n * point_step + 16;

                b = byteArray[rgb_posi + 0];
                g = byteArray[rgb_posi + 1];
                r = byteArray[rgb_posi + 2];

                r = r / rgb_max;
                g = g / rgb_max;
                b = b / rgb_max;

                pcl[n] = new Vector3(x, z, y);
                pcl_color[n] = new Color(r, g, b);


            }
        }

        public Vector3[] GetPCL()
        {
            return pcl;
        }

        public Color[] GetPCLColor()
        {
            return pcl_color;
        }
        */
    }
}