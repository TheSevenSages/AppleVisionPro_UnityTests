using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.AppUI.Core;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARSubsystems;
using static Messenger;
using static UnityEngine.Rendering.ReloadAttribute;

public class Backend : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private BackendSettingsSO BackendSettings;
    public string deviceName = "DEVICE_NAME";
    public bool isHost = false;

    private static HubConnection connection;
    private static bool isConnected = false;

    [HideInInspector]
    public static Backend _instance = null;

    // Represents a package sent from the server
    private class PackageWrapper
    {
        public string type;
    }

    // Connect to the server specified in BackendSettings
    private async void StartConnection()
    {
        // If a connection already exists, stop it before we start this new one
        StopConnection();

        // Build the connection
        connection = new HubConnectionBuilder()
            .WithUrl($"http://{BackendSettings.ip}:{BackendSettings.port}/connectionHub")
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

    public async void StopConnection()
    {
        if (connection != null)
        {
            await connection.StopAsync();
        }
        isConnected = false;
    }

    private void Awake()
    {
        // Singleton pattern
        if (_instance == null)
        {
            DontDestroyOnLoad(this);
            _instance = this;
        }
        else { GameObject.Destroy(this); }

        StartConnection();
    }

    // If not initialized yet wait until either we are or too much time passes.
    private static async Task WaitForInit(float timeout = 0.5f)
    {
        if (connection == null) { throw new Exception("Connection to sever has not been established."); }

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

    public async Task<List<DataStructures.Device>> GetGuestList()
    {
        // Make sure that all the initialization has been completed before continuing
        try
        {
            await WaitForInit();

            string response = await connection.InvokeAsync<string>("RequestServerData", "GUESTS");
            return JsonConvert.DeserializeObject<List<DataStructures.Device>>(response);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to retrieve guest list: " + e.Message);
            return new List<DataStructures.Device>();
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
        try
        {
            string type = JsonConvert.DeserializeObject<PackageWrapper>(package).type;
            if (!Enum.IsDefined(typeof(Messenger.MessageTypes), type))
            {
                throw new Exception($"Invalid message type of {type}");
            }

            // Parse the package Json into the type and message
            var _package = JsonConvert.DeserializeObject<Messenger.Message>(package);
            Messenger.ProcessIncomingMessage(_package);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to deserialize incoming message: " + e.Message);
        }
    }

    public static async void SendMessage(Messenger.Message message)
    {
        try
        {
            if (_instance == null) { throw new Exception("No instance of \"Backend\" exists"); }
            await WaitForInit();

            await connection.SendAsync("SendMessage", message.type.ToString(), message.message);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to send message: " + e.Message);
        }
    }

    // Use to restart the connection to the server after one of the backend settings has been changed.
    public static void ResetConnection()
    {
        if (_instance != null)
        {
            _instance.StartConnection();
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
