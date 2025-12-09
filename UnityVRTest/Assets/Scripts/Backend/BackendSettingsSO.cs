using UnityEngine;

[CreateAssetMenu(fileName = "BackendSettingsSO", menuName = "Scriptable Objects/BackendSettingsSO")]
public class BackendSettingsSO : ScriptableObject
{
    [Header("API Location")]
    public string ip = "127.0.0.1";
    public string port = "8080";
}
