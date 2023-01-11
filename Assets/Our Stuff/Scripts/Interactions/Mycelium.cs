using System.Collections.Generic;
using UnityEngine;

// Connects Between Mushrooms and MushroomBarriers
public class Mycelium : MonoBehaviour
{
    [SerializeField] private List<Mushroom> _mushrooms;
    [SerializeField] private List<MushroomBarrier> _barriers;
    private bool _open;

    private void Start()
    {
        foreach (Mushroom m in _mushrooms)
        {
            m.SetupMushroom(this);
        }
    }
    public void TryOpen()
    {
        if (!_open)
        {
            bool allAwake = true;
            foreach (Mushroom m in _mushrooms)
            {
                if (!m.Awoken) 
                {
                    allAwake = false;
                    break; 
                }
            }
            if (allAwake)
            {
                foreach(MushroomBarrier b in _barriers)
                {
                    b.Open();
                }
            }
        }
    }
}
