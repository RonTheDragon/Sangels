using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private GameManager _gm =>GameManager.Instance;
    private PlayerHealth _playerHealth => _player.GetComponent<PlayerHealth>();
    private PlayerEatFruits _playerEatFruits => _player.GetComponentInChildren<PlayerEatFruits>();
    private DeadPlayer _deadPlayer => _player.GetComponent<DeadPlayer>();

    private Action _loop;


    // UI Elements
    [SerializeField] private Image _playerHealthBar;
    [SerializeField] private Image _playerHealthHitEffect;
    [SerializeField] private Image _playerGutBar;

    [SerializeField] private TMP_Text _interactInfo;

    private float _hpEffectCooldown;

    private void Start()
    {
        _playerHealth.OnHealthChangeUI += UpdateHealth;
        _playerHealth.OnHurt = () => _hpEffectCooldown = 1;
        _playerEatFruits.OnGutChangeUI += UpdateGut;
        //_deadPlayer.OnRevivingRange += InfoNearDeadPlayerInteractInfo;
        _loop += HealthEffect;
    }

    private void Update()
    {
        _loop?.Invoke();
    }


    //private void InfoNearDeadPlayerInteractInfo(string TellToUser, Color color)
    //{
    //    _interactInfo.text = TellToUser;
    //    _interactInfo.color = color;
    //}

    private void UpdateHealth(float fillAmount,Color color) 
    {
        _playerHealthBar.fillAmount = fillAmount;
        _playerHealthBar.color = color;
    }

    private void HealthEffect()
    {
        if (_playerHealthBar.fillAmount > _playerHealthHitEffect.fillAmount)
        {
            _playerHealthHitEffect.fillAmount = _playerHealthBar.fillAmount;
        }
        else if (_playerHealthBar.fillAmount < _playerHealthHitEffect.fillAmount && _hpEffectCooldown<=0)
        {
            _playerHealthHitEffect.fillAmount =
                Mathf.Lerp(_playerHealthHitEffect.fillAmount, _playerHealthBar.fillAmount, 0.1f);
        }

        if (_hpEffectCooldown > 0)
        {
            _hpEffectCooldown -= Time.deltaTime;
        }
    }

    private void UpdateGut(float fillAmount, Color color)
    {
        _playerGutBar.fillAmount = fillAmount;
        _playerGutBar.color = color;
    }



}
