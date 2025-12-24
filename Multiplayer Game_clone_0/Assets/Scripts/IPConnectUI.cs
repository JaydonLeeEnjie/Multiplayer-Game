using TMPro;
using UnityEngine;
using PurrNet;
using PurrNet.Transports;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.Net.NetworkInformation;

public class IPConnectUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private TextMeshProUGUI statusText;

    private NetworkManager networkManager;
    private UDPTransport udpTransport;
    public GameObject IPCanvas;
    public GameObject IPCamera;
    public TextMeshProUGUI LocalIPText;

    private string localIP;

    private void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
        udpTransport = FindObjectOfType<UDPTransport>();

        FindLocalIP();
        UpdateUI();
    }

    private void FindLocalIP()
    {
        string activeIP = GetActiveLocalIP();

        if (!string.IsNullOrEmpty(activeIP))
        {
            localIP = activeIP;
        }
        else
        {
            // Fallback method
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
        }
    }

    private string GetActiveLocalIP()
    {
        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            // Skip non-active or non-Ethernet/Wireless interfaces
            if (ni.OperationalStatus != OperationalStatus.Up)
                continue;

            if (ni.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 &&
                ni.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
                continue;

            foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
            {
                if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    if (!IPAddress.IsLoopback(ip.Address) &&
                        !ip.Address.ToString().StartsWith("169.254"))
                    {
                        return ip.Address.ToString();
                    }
                }
            }
        }
        return null;
    }

    private void UpdateUI()
    {
        if (LocalIPText != null)
            LocalIPText.text = $"Host IP: {localIP}\nShare this IP with other players";

        // Pre-fill input field with local IP for convenience
        if (ipInput != null && string.IsNullOrEmpty(ipInput.text))
            ipInput.placeholder.GetComponent<TMP_Text>().text = $"Enter Host IP (e.g., {localIP})";
    }

    public void JoinGame()
    {
        string ip = ipInput.text.Trim();

        if (string.IsNullOrEmpty(ip))
        {
            if (statusText != null)
                statusText.text = "Please enter the host's IP address!";
            return;
        }

        // Validate IP format
        if (!IsValidIP(ip))
        {
            if (statusText != null)
                statusText.text = "Invalid IP format! Use format like 192.168.1.100";
            return;
        }

        if (statusText != null)
            statusText.text = $"Connecting to {ip}...";

        // Set the host IP for connection
        udpTransport.address = ip;

        try
        {
            networkManager.StartClient();
            TurnOffIPPage();
        }
        catch (System.Exception e)
        {
            if (statusText != null)
                statusText.text = $"Connection failed: {e.Message}";
        }
    }

    public void HostGame()
    {

        udpTransport.address = localIP; 

        if (statusText != null)
            statusText.text = $"Hosting on {localIP}\nWaiting for players...";

        try
        {
            networkManager.StartHost();
            TurnOffIPPage();
        }
        catch (System.Exception e)
        {
            if (statusText != null)
                statusText.text = $"Failed to host: {e.Message}";
        }
    }

    private bool IsValidIP(string ip)
    {
        System.Net.IPAddress address;
        return System.Net.IPAddress.TryParse(ip, out address);
    }

    public void QuickHostAndJoin()
    {
        // For testing on same network - auto-join self
        udpTransport.address = localIP;
        networkManager.StartHost();
        TurnOffIPPage();
    }

    public void TurnOffIPPage()
    {
        if (IPCanvas != null) IPCanvas.SetActive(false);
        if (IPCamera != null) IPCamera.SetActive(false);
    }
}