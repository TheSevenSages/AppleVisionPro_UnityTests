using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuestList : MonoBehaviour
{
    public List<Device> devices;
    public Backend backend;
    public ScrollRect list;
    public GameObject listItem;

    public async void RefreshGuestList()
    {
        // Update the list
        devices = await backend.GetGuestList();
        Debug.Log(devices[0].name);

        DisplayGuestList();
    }

    // Display the guest list in the scroll rect
    private void DisplayGuestList()
    {
        // Clear the current list
        for (int i = 0; i < list.content.childCount; i++)
        {
            GameObject.Destroy(list.content.GetChild(i).gameObject);
        }

        // Repopulate it with the updated items
        float currentYOffset = 0;
        foreach (Device device in devices)
        {
            GameObject newListItem = GameObject.Instantiate(listItem, list.content, false);
            RectTransform t = newListItem.GetComponent<RectTransform>();
            t.position = new Vector3(t.position.x, t.position.y + currentYOffset, t.position.z);
            currentYOffset -= t.rect.height;
            // Set the text display to be the device name
            newListItem.GetComponentInChildren<TMP_Text>().text = device.name;
        }
    }
}
