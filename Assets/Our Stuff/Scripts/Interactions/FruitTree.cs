using System.Collections.Generic;
using UnityEngine;

public class FruitTree : MonoBehaviour
{
    [Header("Fruit Spawn Locations")]
    [Tooltip("Pick an object with Empty childs, the childs are used for the fruit spawning spots")][SerializeField] Transform GrowingSpots;
    [SerializeField] Color _gizmoColor = Color.green;
    [SerializeField] float _gizmoSize = 0.5f;

    [Header("Growing")]
    [Tooltip("x = min Time\ny = max Time\n")]
    public Vector2 RegrowTime = new Vector2(5, 20);
    [HideInInspector]public float CurrentRegrowTime;
    [SerializeField] private string _fruit;
    [Tooltip("x = min\ny = max\nif both bigger than the Growing Spots amount then always all the spots will grow fruits")]
    [SerializeField] private Vector2 _randomAmountOfFruits = new Vector2(2, 5);

    private List<GameObject> _fruits = new List<GameObject>();
    

    public void DropFruits()
    {
        foreach (GameObject f in _fruits)
        {
            f.GetComponent<Rotate>().enabled = true;
            f.GetComponent<FruitCollectable>().Falling = true;
        }
        _fruits.Clear();
    }

    public void GrowFruits()
    {
        if (GrowingSpots != null)
        {
            if (GrowingSpots.childCount < 1)
            {
                Debug.LogWarning($"{gameObject.name} is a tree without places to grow fruits");
            }
            else
            {
                int Amount = (int)Random.Range(_randomAmountOfFruits.x, _randomAmountOfFruits.y);
                if (Amount < 1) Amount = 1;
                else if (Amount > GrowingSpots.childCount) Amount = GrowingSpots.childCount;


                if (Amount == GrowingSpots.childCount) // Spawn fruits in all spots
                {
                    foreach (Transform t in GrowingSpots)
                    {
                        GrowFruit(t);
                    }
                }
                else // Spawn fruits Random Spots
                {

                    List<Transform> locations = new List<Transform>();
                    List<Transform> AllLocations = new List<Transform>();

                    foreach (Transform t in GrowingSpots) // ChildArray to list
                    {
                        AllLocations.Add(t);
                    }

                    for (int i = 0; i < Amount; i++) //pick random locations
                    {
                        int R = Random.Range(0, AllLocations.Count - 1);
                        locations.Add(AllLocations[R]);
                        AllLocations.RemoveAt(R);
                    }

                    foreach (Transform t in locations)
                    {
                        GrowFruit(t);
                    }
                }
            }
        }
    }

    private void GrowFruit(Transform t)
    {
        GameObject FruitObject = ObjectPooler.Instance.SpawnFromPool(_fruit, t.position, t.rotation);
        FruitObject.GetComponent<Collider>().isTrigger = false;
        FruitObject.GetComponent<Rotate>().enabled = false;
        FruitObject.GetComponent<Rigidbody>().isKinematic = true;
        _fruits.Add(FruitObject);
    }

    public void RandomizeGrowTime()
    {
        CurrentRegrowTime = Random.Range(RegrowTime.x,RegrowTime.y);
    }

    private void OnDrawGizmos()
    {
        if (GrowingSpots != null)
        {
            if (GrowingSpots.childCount > 0)
            {
                Gizmos.color = _gizmoColor;
                foreach (Transform t in GrowingSpots)
                {
                    Gizmos.DrawSphere(t.position, _gizmoSize);
                }
            }
        }
    }
}
