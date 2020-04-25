using System.Runtime.Serialization;
using System.Xml.Serialization;
using UnityEngine;

[System.Serializable]
public class PowerUpState
{
    public int Level; // -1: locked, 0: unlocked, but needed animation
    public int AmountPerLevel;
}