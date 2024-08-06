using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace MonoBehaviours.Network
{
    public class NetworkUIManager : NetworkBehaviour
    {
        public InputField joinCodeInputField;
        public Transform roomListParent;
        public GameObject roomListItemPrefab;

        private void Start()
        {
            
        }

        public void DisplayRooms()
        {
            // Here you would dynamically create UI elements for each room
            // For simplicity, we create one manually
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);
            roomItem.GetComponentInChildren<Text>().text = "Room 1"; // Replace with actual room name
            roomItem.GetComponent<Button>().onClick.AddListener(() =>
            {
                joinCodeInputField.text = "JOIN_CODE"; // Replace with actual join code
                //JoinHost();
            });
        }
    }
}