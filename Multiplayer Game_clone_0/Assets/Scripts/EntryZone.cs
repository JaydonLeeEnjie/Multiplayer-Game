using PurrNet;
using PurrNet.Modules;
using PurrNet.Transports;
using UnityEngine;
using UnityEngine.UI;

public class EntryZone : MonoBehaviour
{
    private NetworkManager networkManager;
    [SerializeField] private int playerCount;
    [SerializeField] private float timer;
    [SerializeField] private float timerMax;
    [SerializeField] private Image timerImage;
    public LayerMask playerLayer;


    private void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!(((1 << other.gameObject.layer) & playerLayer) != 0))
            return;
        if(other.transform.root != other.transform) return;

        playerCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!(((1 << other.gameObject.layer) & playerLayer) != 0))
            return;
        if (other.transform.root != other.transform) return;

        playerCount--;
    }

    private void Update()
    {

        if(playerCount == networkManager.playerCount && playerCount != 0)
        {
            timer = Mathf.Clamp(timer + Time.deltaTime, 0f, timerMax);
            timerImage.fillAmount = timer/ timerMax;
        }
        else
        {
            timer = Mathf.Clamp(timer - Time.deltaTime, 0f, timerMax);
            timerImage.fillAmount = timer/ timerMax;
        }


        if (timer == timerMax)
        {
            Debug.Log("ENTER");
        }
    }



}
