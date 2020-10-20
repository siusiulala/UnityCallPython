using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PythonStarter : MonoBehaviour
{
    public SocketServer socketServer;


    byte[] imgData;

    public RawImage rawImage;
    // Start is called before the first frame update
    void Start()
    {
        socketServer.ReceiveData += this.OnReceiveData;

        DoCV();


    }

    public void OnReceiveData(object sender, SocketServer.SocketEventArgs args)
    {
     
        imgData = args.data;

    }

    // Update is called once per frame
    void Update()
    {
        if (imgData!=null)
        {
            Texture2D texture = new Texture2D(10, 10);
            texture.LoadImage(imgData);

            rawImage.texture = texture;
        }
   
    }

    public void DoAdd(int x, int y)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo("python");
        
        startInfo.Arguments = "./add.py " + x + " " + y;
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        Process pyProcess = new Process();
        pyProcess.StartInfo = startInfo;
        pyProcess.Start();
        UnityEngine.Debug.LogFormat("Calling Python script with arguments {0} and {1}", x, y);
        var result = pyProcess.StandardOutput.ReadLine();
        print("Result: " + result);
        pyProcess.WaitForExit();
        pyProcess.Close();
    }

    //https://stackoverflow.com/questions/14455510/how-to-start-a-process-in-a-thread
    public void DoCV()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo("python3");

        startInfo.Arguments = "./cam.py";
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        Process pyProcess = new Process();
        pyProcess.StartInfo = startInfo;

        ThreadStart ths = new ThreadStart(() => pyProcess.Start());
        Thread th = new Thread(ths);
        th.Start();

        //pyProcess.WaitForExit();
        //pyProcess.Close();
    }
}
