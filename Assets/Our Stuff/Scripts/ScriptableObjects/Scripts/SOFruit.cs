using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

[SerializeField]
[CreateAssetMenu(fileName = "New Fruit", menuName = "Fruit Data", order = 51)]
public class SOFruit : ScriptableObject
{
    public Sprite Icon;

    public enum Fruit 
    {
        Luber,
        Scrumbulk,
        Glub,
        Fepler,
        Velo,
        Vitaliv
    }
    public Fruit FruitType;

    public int MaxAmount;
    public int CurrentAmount;
}
