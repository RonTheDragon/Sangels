using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Visible //

    [Tooltip("List of all The Players")]
    [ReadOnly][SerializeField] GameObject[] Players = new GameObject[4];

    [Tooltip("True - will Remove a player that disconnected his Controller\n\nFalse - will Keep the player inside, allowing him to rejoin to where he left off")]
    public bool LeaveOnDisconnect;

    [Header("Layer Masks")]
    public LayerMask CanJumpOn;
    public LayerMask PlayersCanAttack;
    public LayerMask EnemiesCanSee;
    public LayerMask EnemiesCanAttack;
    public LayerMask TrajectoryHits;
    public LayerMask Everything;
    public LayerMask Nothing;
    public LayerMask ProjectileBounceCanSee;

    [Header("Camera Layers For Each Player")]
    [Tooltip("These LayerMasks are needed for local Co-op so Cinemachine can work properly\n\n Just Make Sure Each Layer Mask cant see other numbered player but itself")]
    public LayerMask[] PlayerMasks = new LayerMask[4];

    // Invisible //

    [HideInInspector] public static GameManager instance;
    bool _turnedOffStartingCam;

    GameObject _playerSpawner => transform.GetChild(0).gameObject;
    GameObject _noPlayersCamera => _playerSpawner.transform.GetChild(0).gameObject;
    Transform _playerSpawnPoint => _playerSpawner.transform.GetChild(1);

    // Starts Before The Game Starts
    private void Awake()
    {
        instance = this;
    }

    /// <summary> When Any Player Joins </summary>
    public void PlayerJoined()
    {
        if (!_turnedOffStartingCam)
        {
            _noPlayersCamera.SetActive(false);
            _turnedOffStartingCam = true;
        }
    }

    /// <summary> When "LeaveOnDisconnect" is true and someone left </summary>
    public void PlayerLeft(int n)
    {
            Players[n - 1] = null;
            // Turn On No Players Camera if there are no players left        
            if (0 == CountPlayers())
            {
                _noPlayersCamera.SetActive(true);
                _turnedOffStartingCam = false;
            }
    }

    /// <summary> Players Call this Function When they join to update the Game Manager </summary>
    public int AddPlayer(GameObject Player)
    {
        CharacterController CC = Player.GetComponentInChildren<CharacterController>();
        CC.enabled = false;
        if (0 == CountPlayers())
        { 
            Player.transform.position = _playerSpawnPoint.position;
        }
        else
        {
            Player.transform.position = GetLeadPlayer().transform.position + Vector3.up*2;
        }
        CC.enabled = true;

        for (int i = 0; i < 4; i++)
        {
            if (Players[i] == null)
            {
                Players[i] = Player;
                return (i+1);
            }
        }


        return 0;
    }

    GameObject GetLeadPlayer()
    {
        foreach(GameObject p in Players)
        {
            if(p != null)
            {
                return p;
            }
        }
        return null;
    }

    /// <summary> Returns The Amount of Players Currently Playing </summary>
    public int CountPlayers()
    {
        int count = 0;
        for (int i = 0; i < 4; i++)
        {
            if (Players[i] != null)
            {
                count++;
            }
        }
        return count;
    }

    public float AngleDifference(float angle1, float angle2)
    {
        float diff = (angle2 - angle1 + 180) % 360 - 180;
        return diff < -180 ? diff + 360 : diff;
    }

    public Collider ClosestColliderInList(List<Collider> coliders)
    {
        if (coliders == null)
            return null;
        if (coliders.Count == 1)
            return coliders.FirstOrDefault();
        float MinDist = Mathf.Infinity;
        int ClosestColliderIndex = 0;
        for (int i = 0; i < coliders.Count - 1; i++)
        {
            float dist = Vector3.Distance(coliders[i].transform.position, transform.position);
            if (MinDist > dist)
            {
                MinDist = dist;
                ClosestColliderIndex = i;
            }
        }
        return coliders[ClosestColliderIndex];
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(_playerSpawnPoint.position, Vector3.one);
    }
}

#region Magic Trick That enables ReadOnly

public class ReadOnlyAttribute : PropertyAttribute
{

}

#endregion
