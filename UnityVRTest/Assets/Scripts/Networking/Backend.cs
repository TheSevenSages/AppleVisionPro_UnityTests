using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;
using System;
using NUnit.Framework;
using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class Device
{
    public string name { get; set; }
    public string id { get; set; }
}
public class Backend : MonoBehaviour
{
    [Header("Settings")]
    public string deviceName = "DEVICE_NAME";
    public bool isHost = false;

    [Header("API Location")]
    [SerializeField]
    string ip = "127.0.0.1";
    [SerializeField]
    string port = "8080";

    private HubConnection connection;
   
    async void Start()
    {
        // Build the connection
        connection = new HubConnectionBuilder()
            .WithUrl($"http://{ip}:{port}/connectionHub")
            .Build();

        await connection.StartAsync();

        try
        {
            await connection.InvokeAsync("InitDevice", deviceName, isHost);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to init device: " + e.Message);
        }
    }

    public async Task<List<Device>> GetGuestList()
    {
        try
        {
            string response = await connection.InvokeAsync<string>("RequestServerData", "GUESTS");
            return JsonConvert.DeserializeObject<List<Device>>(response);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to retrieve guest list: " + e.Message);
            return null;
        }
    }

    public void GetMessage(string message)
    {
        Debug.Log(message);
    }
}
