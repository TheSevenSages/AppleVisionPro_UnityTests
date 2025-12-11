using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuestList : MonoBehaviour
{
    public List<DataStructures.Device> guests;
    public Backend backend;
    public ScrollRect list;
    public GameObject listItem;

    private void Start()
    {
        RefreshGuestList();    
    }

    public async void RefreshGuestList()
    {
        // Update the list
        guests = await backend.GetGuestList();

        if (guests != null)
        {
            DisplayGuestList();
        }
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
        foreach (var guest in guests)
        {
            // Create new list item for this guest
            GameObject newListItem = GameObject.Instantiate(listItem, list.content, false);
            RectTransform t = newListItem.GetComponent<RectTransform>();

            // Set list item's position
            t.position = new Vector3(t.position.x, t.position.y + currentYOffset, t.position.z);
            currentYOffset -= t.rect.height;

            // Set the text display to be the device name
            newListItem.GetComponentInChildren<TMP_Text>().text = guest.name;

            // Set onclick to send an invite for this guest
            newListItem.GetComponent<Button>().onClick.AddListener(async () =>
            {
                float before = Time.time;
                bool success = await backend.SendInviteToGuest(guest.id);
                if (success)
                {
                    Debug.Log($"Now linked to guest(ID={guest.id},NAME={guest.name}) [{(int)((Time.time - before) * 1000)}ms]");
                    RefreshGuestList();
                }
            });
        }
    }
}
