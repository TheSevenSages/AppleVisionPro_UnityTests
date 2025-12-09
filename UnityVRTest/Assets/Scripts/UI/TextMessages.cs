using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextMessages : MonoBehaviour
{
    private void Start()
    {
        Messanger.TextMessageEvent.AddListener(ProcessTextMessage);
    }

    void ProcessTextMessage(string text)
    {
        Debug.Log("TextRecieved!");
        Debug.Log(text);
    }
}
