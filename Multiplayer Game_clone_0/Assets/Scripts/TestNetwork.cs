using System;
using PurrNet;
using TMPro;
using UnityEngine;

public class TestNetwork : NetworkIdentity
{
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private SyncVar<int> _health = new(100,ownerAuth:true);


    private void Awake()
    {
        _health.onChanged += OnChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _health.onChanged -= OnChanged;
    }

    protected override void OnSpawned()
    {
        base.OnSpawned();

        enabled = isOwner;
    }

    private void OnChanged(int newValue)
    {
        _healthText.text = newValue.ToString();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            TakeDamage(10);
        }
    }

            
    private void TakeDamage(int damage)
    {
        _health.value -= damage;

        if(_health.value <= 0)
        {
            _health.value = 0;
            Destroy(gameObject);
        }
    }


}
