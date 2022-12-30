using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;




[SerializeField]
[CreateAssetMenu(fileName = "New Fruit", menuName = "Fruit Data", order = 51)]
public class SOFruit : ScriptableObject
{
    public Sprite Fruits;

    public enum Fruit 
    {
        Glub,
        Fepler,
        Luber,
        Velo,
        Vitaliv,
        Scrumbulk
    }
    public Fruit fruit;


}
