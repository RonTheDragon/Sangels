using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    private GameManager _gm =>GameManager.Instance;
    private PlayerHealth _playerHealth => _player.GetComponent<PlayerHealth>();
    private PlayerEatFruits _playerEatFruits => _player.GetComponentInChildren<PlayerEatFruits>();




    //components
    [SerializeField] private Image _playerHealthBar;
    [SerializeField] private Image _playerGutBar;


    private void Start()
    {
        _playerHealth.OnHealthChangeUI += UpdateHealth;
        _playerEatFruits.OnGutChangeUI += UpdateGut;
    }

    //private void Update()
    //{
    //    
    //}


    private void UpdateHealth(float fillAmount,Color color) 
    {
        _playerHealthBar.fillAmount = fillAmount;
        _playerHealthBar.color = color;
    }


    private void UpdateGut(float fillAmount, Color color)
    {
        _playerGutBar.fillAmount = fillAmount;
        _playerGutBar.color = color;
    }



}
