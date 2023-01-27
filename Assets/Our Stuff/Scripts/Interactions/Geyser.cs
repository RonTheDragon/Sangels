using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Geyser : MonoBehaviour , Iinteractable
{
    [SerializeField] protected string _info = "Your Gayser";
    string Iinteractable.Information { get { return _info; } set { _info = value; } }

    [SerializeField] protected Color _textColor = Color.white;
    Color Iinteractable.TextColor { get { return _textColor; } set { _textColor = value; } }

    [SerializeField] private float CheckForPlayersRange = 7;
    [SerializeField] private float CheckForPlayersCooldown = 1;
    private float CheckForPlayersCD;
    private GameManager _gm => GameManager.Instance;
    private LayerMask OnlyPlayersMask => _gm.PlayersOnly;

    public float UseTime { get => 0; set => throw new System.NotImplementedException(); }

    public GeyserState GeyserCurrentState;

    [SerializeField] private float SplashTime = 3;

    [SerializeField] private VisualEffect _geyserEffect;

    public enum GeyserState
    {
        GeyserActive,
        GeyserResting,
        GeyserAsleep
    }

    public bool CanUse()
    {
        if (GeyserCurrentState == GeyserState.GeyserResting)
        {
            return true;
        }
            return false;
    }

    public void Use()
    {
        WakeUpFromRest();
    }

    // Start is called before the first frame update
    void Start()
    {
        _gm.AddGeyser(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (GeyserCurrentState == GeyserState.GeyserAsleep)
        {
            SleepingGeyser();
        }
    }

    private void SleepingGeyser()
    {
        if (CheckForPlayersCD > 0) { CheckForPlayersCD -= Time.deltaTime; }
        else
        {
            CheckForPlayersCD = CheckForPlayersCooldown;

            if (Physics.CheckSphere(transform.position, CheckForPlayersRange, OnlyPlayersMask))
            {
                WakeUpFromSleep();
            }
        }
    }

    private void WakeUpFromSleep()
    {
        WakeUp();
    }

    private void WakeUpFromRest()
    {
        WakeUp();
    }

    private void WakeUp()
    {
        _info = "Your Gayser";
        _gm.SwitchGeyser(this);
        GeyserCurrentState = GeyserState.GeyserActive;
        StartCoroutine("Splash");
        _geyserEffect.SendEvent("OnPlaySmoke");
    }

    public void Rest()
    {
        GeyserCurrentState = GeyserState.GeyserResting;
        _info = "Activate To Set Spawnpoint";
        _geyserEffect.SendEvent("OnStopSmoke");
    }

    private IEnumerator Splash()
    {
        _geyserEffect.SendEvent("OnPlay");
        yield return new WaitForSeconds(SplashTime);
        _geyserEffect.SendEvent("OnStop");
    }
}
