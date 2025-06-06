using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Configs/WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    [Min(1)] public int burstCount = 3;
    public float interval = 0.1f;
    public float cooldown = 0.5f;

    public GameObject projectilePrefab;
}
