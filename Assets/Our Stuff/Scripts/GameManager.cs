using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Visible //

    [Tooltip("List of all The Players")]
    [ReadOnly][SerializeField] GameObject[] Players = new GameObject[4];

    [Tooltip("True - will Remove a player that disconnected his Controller\n\nFalse - will Keep the player inside, allowing him to rejoin to where he left off")]
    public bool LeaveOnDisconnect;

    [Header("Camera Layers For Each Player")]
    [Tooltip("These LayerMasks are needed for local Co-op so Cinemachine can work properly\n\n Just Make Sure Each Layer Mask cant see other numbered player but itself")]
    public LayerMask[] PlayerMasks = new LayerMask[4];

    // Invisible //

    [HideInInspector] public static GameManager instance;
    bool _turnedOffStartingCam;
    GameObject _noPlayersCamera => transform.GetChild(0).gameObject;

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
}

#region Magic Trick That enables ReadOnly
public class ReadOnlyAttribute : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}
#endregion
