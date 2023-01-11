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
        Glub,
        Fepler,
        Vitaliv,
        Shbulk,
        Lipachu,
    }
    public Fruit FruitType;

    public int MaxAmount;
    public int CurrentAmount;
}
