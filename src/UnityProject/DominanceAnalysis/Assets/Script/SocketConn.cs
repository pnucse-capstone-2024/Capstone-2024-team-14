using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Debug = UnityEngine.Debug;
using System.Diagnostics;
using System.IO;

public class SocketConn : MonoBehaviour
{
    Thread mThread;
    public string connectionIP = "127.0.0.1";
    public int connectionPort = 25001;
    [HideInInspector] public TcpClient client;
    NetworkStream nwStream;

    public string pythonPath;
    public string filePath;

    public bool openWindow = false;

    private Process pythonProcess;
    private string dialog = "";
    bool running;

    private async void Start()
    {
        // 상대 경로를 절대 경로로 변환
        string absolutePythonPath = Path.Combine(Application.dataPath, pythonPath);
        string absoluteFilePath = Path.Combine(Application.dataPath, filePath);

        try
        {
            // Python 프로세스 시작
            Process psi = new Process();
            psi.StartInfo.FileName = absolutePythonPath;  // Python 실행 파일 경로
            psi.StartInfo.Arguments = absoluteFilePath;   // 실행할 Python 파일 경로
            psi.StartInfo.CreateNoWindow = !openWindow;
            psi.StartInfo.UseShellExecute = openWindow;

            psi.Start();
            psi.WaitForExit(3000);

            Debug.Log("[알림] .py 파일 실행 중: " + absoluteFilePath);

        }
        catch (Exception e)
        {
            Debug.LogError("[알림] 에러 발생: " + e.Message);
        }

        await SetupSocketAsync();
    }

    async Task SetupSocketAsync()
    {
        try
        {
            client = new TcpClient();
            await client.ConnectAsync(connectionIP, connectionPort);
            nwStream = client.GetStream();
            running = true;
            Debug.Log("Connected to server");
        }
        catch (SocketException e)
        {
            Debug.LogError($"SocketException: {e}");
        }
    }

    public async Task<string> SendAndReceiveDataAsync(string inputData)
    {
        byte[] myWriteBuffer = Encoding.UTF8.GetBytes(inputData);
        await nwStream.WriteAsync(myWriteBuffer, 0, myWriteBuffer.Length); // Sending the data to Python

        // Receiving Data from the Host
        byte[] buffer = new byte[client.ReceiveBufferSize];
        int bytesRead = await nwStream.ReadAsync(buffer, 0, client.ReceiveBufferSize); // Getting data in Bytes from Python
        string dataReceived = Encoding.UTF8.GetString(buffer, 0, bytesRead); // Converting byte data to string
        if (!string.IsNullOrEmpty(dataReceived))
        {
            dialog = dataReceived; // Assigning dialog value from Python
            // Debug.Log("Received dialog data: " + dialog);

            return dialog;
        }

        return null;
    }

    private void OnApplicationQuit()
    {
        if(pythonProcess != null && !pythonProcess.HasExited)
        {
            pythonProcess.Kill();
            pythonProcess.Dispose();
            Debug.Log("[알림] Python 프로세스를 종료했습니다.");
        }
    }

    private void Update()
    {
        //if (!string.IsNullOrEmpty(dialog))
        //{
        //    Debug.Log($"Received dialog: {dialog}");
        //    // Here you can update the object's dialogue UI or perform actions based on the dialog
        //}
    }
}
