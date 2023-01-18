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

    public GeyserState GeyserCurrentState;

    [SerializeField] private float SplashTime = 3;

    [SerializeField] private VisualEffect GeyserEffect;

    public enum GeyserState
    {
        GeyserActive,
        GeyserResting,
        GeyserAsleep
    }

    public bool CanUse()
    {
        return true;
    }

    public void Use()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        GeyserEffect.SendEvent("OnStop");
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
        if (Physics.CheckSphere(transform.position, CheckForPlayersRange, OnlyPlayersMask))
        {
            WakeUpFromSleep();
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
        GeyserCurrentState = GeyserState.GeyserActive;
        StartCoroutine("Splash");
    }

    private void Rest()
    {

    }

    private IEnumerator Splash()
    {
        GeyserEffect.SendEvent("OnPlay");
        yield return new WaitForSeconds(SplashTime);
        GeyserEffect.SendEvent("OnStop");
    }
}
