using PurrNet;
using PurrNet.Modules;
using PurrNet.Transports;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntryZone : MonoBehaviour
{
    private NetworkManager networkManager;
    private bool hasLockedIn = false;
    [SerializeField] private int playerCount;
    [SerializeField] private float timer;
    [SerializeField] private float timerMax;
    [SerializeField] private Image timerImage;
    private HashSet<NetworkIdentity> playersInside = new();

    [SerializeField] private List<Transform> PlayerSpots;

    public CombatManager combatManager;

    private void Start()
    {
        networkManager = FindObjectOfType<NetworkManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<NetworkIdentity>();
        if (player == null || playersInside.Contains(player)) return;

        playersInside.Add(player);
        playerCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponentInParent<NetworkIdentity>();
        if (player == null || !playersInside.Contains(player)) return;

        playersInside.Remove(player);
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


        if (!hasLockedIn && timer == timerMax)
        {
            hasLockedIn = true;
            AssignPlayersToSpots();
        }
    }

    private void UpdatePlayerSpots(int count)
    {
        float spacing = 0.025f;
        float startZ = -(count - 1) * spacing * 0.5f;

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = PlayerSpots[i].localPosition;
            pos.z = startZ + i * spacing;
            PlayerSpots[i].localPosition = pos;
        }
    }

    private void AssignPlayersToSpots()
    {
        int count = Mathf.Min(playersInside.Count, PlayerSpots.Count);

        // Ensure spots are positioned correctly first
        UpdatePlayerSpots(count);

        int index = 0;
        foreach (var playerIdentity in playersInside)
        {
            var controller = playerIdentity.GetComponent<PlayerController>();
            if (controller == null) continue;
            controller.PrepareEnterCombat(PlayerSpots[index]);
            combatManager.Entities.Add(controller.CombatPlayer);
            index++;
        }
        combatManager.CalculateTurnOrder();
    }





}
