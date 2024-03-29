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

    private PlayerAmmoSwitch _playerAmmoSwitch => _player.GetComponentInChildren<PlayerAmmoSwitch>();
    private PlayerCombatManager _playerCM => _player.GetComponentInChildren<PlayerCombatManager>();

    private Interaction _interaction => _player.GetComponent<Interaction>();

    private Action _loop;


    // UI Elements
    [Header("Health")]
    [SerializeField] private Image _playerHealthBar;
    [SerializeField] private Image _playerHealthHitEffect;
    [Header("Gut")]
    [SerializeField] private Image _playerGutBar;

    [Header("Interaction")]
    [SerializeField] private GameObject _interactWindow;
    [SerializeField] private TMP_Text _interactInfo;
    [SerializeField] private Image _progressBar;

    [Header("UI Wheel")]
    [SerializeField] private Color selectedColor;
    [SerializeField] private TMP_Text _fruitAmount;
    [SerializeField] private TMP_Text _fruitName;
    [SerializeField] private Color isNotSelectedColor;
    //[SerializeField] private Button _luberImage;
    //[SerializeField] private Button _glubImage;
    //[SerializeField] private Button Fepler;
    //[SerializeField] private Button Vitaliv;
    //[SerializeField] private Button Shbulk;
    //[SerializeField] private Button Lipachu;





    private float _hpEffectCooldown;

    private void Start()
    {
        _playerHealth.OnHealthChangeUI += UpdateHealth;
        _playerHealth.OnHurt = () => _hpEffectCooldown = 1;
        _playerEatFruits.OnGutChangeUI += UpdateGut;

        _interaction.OnInteracting += UpdateInteractionInfo;
        _interaction.OnStopInteracting += StopInteraction;
        _interaction.OnInteractingProgress += UpdateInteractionProgress;

        _playerAmmoSwitch.OnFruitSwitchUI += OnWheelItemSelected;
        _playerCM.OnConsumeFruitUI += OnUpdateFruitAmount;

        _loop += HealthEffect;
    }

    private void Update()
    {
        _loop?.Invoke();
    }

    public void OnWheelItemSelected(string fruitName,int fruitAmount) 
    {
        _fruitName.text = fruitName;
        _fruitAmount.text = $"Amount: {fruitAmount}";
    }
    public void OnUpdateFruitAmount(int fruitAmount)
    {

        _fruitAmount.text = $"Amount: {fruitAmount}";

    }

    #region Health
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
    #endregion
    #region Gut
    private void UpdateGut(float fillAmount, Color color)
    {
        _playerGutBar.fillAmount = fillAmount;
        _playerGutBar.color = color;
    }
    #endregion
    #region Interaction
    private void UpdateInteractionInfo(string text, Color color, bool canUse)
    {
        _interactWindow.SetActive(true);
        _interactInfo.color = color;
        _interactInfo.alpha = canUse? 1 : 0.5f;
        _interactInfo.text = text;
        _progressBar.fillAmount = 0;
    }

    private void StopInteraction()
    {
        _interactWindow.SetActive(false);
    }

    private void UpdateInteractionProgress(float current, float Max)
    {
        _progressBar.fillAmount = current / Max;
    }
    #endregion
}
