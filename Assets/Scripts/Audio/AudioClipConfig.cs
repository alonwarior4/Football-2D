using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Audio Clip Config")]
public class AudioClipConfig : ScriptableObject
{
    [Header("Environment")]
    [Range(0 , 1f)] public float fireAmbient_Vol;
    [Space(20)]
    [Range(0, 1f)] public float suctionDeviceActivate_Vol;
    [Space(20)]
    [Range(0, 1f)] public float boxBreak_Vol;
    [Range(0, 1f)] public float boxExplosion_Vol;
    [Space(20)]
    [Range(0, 1f)] public float glassBreak_Vol;
    [Range(0, 1f)] public float gasExplosion_Vol;
    [Range(0, 1f)] public float ropeTear_Vol;
    [Space(20)]
    [Range(0, 1f)] public float normalDoorOpen_Vol;
    [Range(0, 1f)] public float doorTaghSound_Vol;
    [Space(20)]
    [Range(0, 1f)] public float buttonHit_Vol;
    [Range(0, 1f)] public float concreteWallExplode_Vol;
    [Space(20)]
    [Range(0, 1f)] public float wreckballCollision_Vol;


    [Header("Effects")]
    public AudioConfig bulletWallHit_Cfg;
    public AudioConfig bulletFullRicochet_Cfg;
    [Space(20)]
    [Range(0, 1f)] public float bloodSplash_Vol;
    [Range(0, 1f)] public float zombieCanDrop_Vol;


    [Header("Player")]
    [Range(0, 1f)] public float playerStartJump_Vol;
    [Range(0, 1f)] public float playerEndJumpGround_Vol;


    [Header("Enemies")]
    [Range(0, 1f)] public float enemyPistolShoot_Vol;
    [Range(0, 1f)] public float enemyAssualtSingleShoot_Vol;
    [Space(20)]
    [Range(0, 1f)] public float zombieDeath_Vol;
    [Range(0, 1f)] public float humanDeath_Vol;
    [Space(20)]
    [Range(0, 1f)] public float enemyBodyDamage_Vol;
    [Space(20)]
    [Range(0, 1f)] public float zombieMeleeAttack_Vol;
}


[System.Serializable]
public class AudioConfig
{    
    [Range(0 , 1f)] public float volume;
    [Range(1 , 100f)] public float chance;
}
