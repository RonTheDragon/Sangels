using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager instance;
    public int PlayersAmount = 0;
    bool TurnedOffStartingCam;

    private void Awake()
    {
        GameManager.instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddPlayer()
    {
        PlayersAmount++;
        if (!TurnedOffStartingCam)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            TurnedOffStartingCam = true;
        }
    }
}
