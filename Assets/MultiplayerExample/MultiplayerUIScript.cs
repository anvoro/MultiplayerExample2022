using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class MultiplayerUIScript : MonoBehaviour
{
    [SerializeField] private Button serverButton;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;
    [SerializeField] private TMP_InputField ipInputField;
    [SerializeField] private GameObject ballPrefab;

    private void Awake()
    {
        serverButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartServer();

        });
        hostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            GameObject ball = Instantiate(ballPrefab, Vector3.zero, Quaternion.identity);
            ball.GetComponent<NetworkObject>().Spawn();

        });
        clientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipInputField.text, 7777);
            NetworkManager.Singleton.StartClient();

        });
        ipInputField.text = NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address;
    }
}
