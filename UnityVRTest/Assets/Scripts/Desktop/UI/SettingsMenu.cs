using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("ScriptableObjects")]
    [SerializeField]
    private BackendSettingsSO BackendSettings;

    [Header("UI Elements")]
    public TMP_InputField serverAddress;
    public TMP_Text deviceName;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        serverAddress.text = BackendSettings.ip;
        if (Backend._instance != null)
        {
            deviceName.text = Backend._instance.deviceName;
        }
    }

    public void ChangeServerAddress(string address)
    {
        BackendSettings.ip = address;
        Backend.ResetConnection();
    }
}
