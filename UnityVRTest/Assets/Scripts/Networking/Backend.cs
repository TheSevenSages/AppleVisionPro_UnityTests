using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
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

    // Events to be called based on recieved messages
    [HideInInspector]
    public static UnityEvent<string> TextMessageEvent = new UnityEvent<string>();

    // Represents a package sent from the server
    private class ServerPackage
    {
        public string type;
        public string message;
    }

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

            // Map methods to SignalR client endpoints
            connection.On<string>("GetMessage", GetMessage);
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

    /// <summary>
    /// Processes incoming messages from the server
    /// </summary>
    /// <param name="package">The message from the server. Json with the structure {type, message}</param>
    private void GetMessage(string package)
    {
        // Parse the package Json into the type and message
        ServerPackage _package = JsonConvert.DeserializeObject<ServerPackage>(package);
        ProcessPackage(_package);
    }

    /// <summary>
    /// Handle server packages properly according to its type
    /// </summary>
    /// <param name="package">The package sent by the server</param>
    private void ProcessPackage(ServerPackage package)
    {
        switch (package.type)
        {
            case "TEXT":
                TextMessageEvent.Invoke(package.message);
                break;

            default:
                Debug.LogWarning($"Recieved package of unknown type: {package.type}");
                break;
        }
    }

    private void OnDestroy()
    {
        if (connection != null)
        {
            connection.StopAsync();
        }
    }
}
