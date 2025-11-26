using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;

[System.Serializable]
public class Guest
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
    private bool isConnected = false;
    private Task initTask = null;

    private async void Awake()
    {
        // Build the connection
        connection = new HubConnectionBuilder()
            .WithUrl($"http://{ip}:{port}/connectionHub")
            .Build();

        await connection.StartAsync();

        try
        {
            await connection.InvokeAsync<Task>("InitDevice", deviceName, isHost);
            Debug.Log("Initialized with server!");
            isConnected = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to init device: " + e.Message);
        }
    }

    // If not initialized yet wait until either we are or too much time passes.
    private async Task WaitForInit(float timeout = 0.5f)
    {
        float totalWaitingTime = 0.0f;
        while (!isConnected) 
        { 
            Debug.Log("Waiting for init...");
            totalWaitingTime += Time.deltaTime;
            if (totalWaitingTime > timeout) { break; }
            await Task.Yield();
        }
        if (!isConnected) { throw new Exception("This client is not initialized with the server yet."); }
    }

    public async Task<List<Guest>> GetGuestList()
    {
        // Make sure that all the initialization has been completed before continuing
        try
        {
            await WaitForInit();

            string response = await connection.InvokeAsync<string>("RequestServerData", "GUESTS");
            return JsonConvert.DeserializeObject<List<Guest>>(response);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to retrieve guest list: " + e.Message);
            return new List<Guest>();
        }
    }

    /// <summary>
    /// Send an invite to the guest with the provided id.
    /// </summary>
    /// <param name="guestId">The id associated with the guest we want to send an invite.</param>
    /// <returns>True if the link succeeds and false otherwise.</returns>
    public async Task<bool> SendInviteToGuest(string guestId)
    {
        try
        {
            await WaitForInit();

            await connection.InvokeAsync("RequestDeviceLink", guestId);
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to invite guest: " + e.Message);
            return false;
        }
    }

    public void GetMessage(string message)
    {
        Debug.Log(message);
    }
}
