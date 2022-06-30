using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DancerManager : MonoBehaviour
{   
    [Header("Dancer Prefabs")]
    [SerializeField] Dancer zombiePref;
    [SerializeField] Dancer humanPref;

    [Header("Dancer Positions")]
    [SerializeField] Transform[] zombieDancerPositions;
    [SerializeField] Transform[] humanDancerPositions;

    List<Dancer> dancers = new List<Dancer>();

    public static Action OnZombiesHighScore;
    public static Action OnHumanHighScore;
    public static Action OnDancersIdeal;

    private void Awake()
    {
        InstantiateDancers();

        OnZombiesHighScore += ZombieDance;
        OnHumanHighScore += HumanDance;
        OnDancersIdeal += DancersIdeal;
    }

    void InstantiateDancers()
    {
        for(int i=0; i< zombieDancerPositions.Length; i++)
        {
            Dancer dancer = Instantiate(zombiePref, zombieDancerPositions[i].position, Quaternion.identity);
            dancers.Add(dancer);
            dancer.OnStart();
            dancer.GoIdeal();

            dancer.SetSortingLayers(zombieDancerPositions[i].GetComponent<DancerPos>().id);
        }

        for(int i=0; i< humanDancerPositions.Length; i++)
        {
            Dancer dancer = Instantiate(humanPref, humanDancerPositions[i].position, Quaternion.identity);
            dancers.Add(dancer);
            dancer.OnStart();
            dancer.GoIdeal();

            dancer.SetSortingLayers(humanDancerPositions[i].GetComponent<DancerPos>().id);
        }
    }

    void ZombieDance()
    {        
        for(int i=0; i< dancers.Count; i++)
        {
            if (dancers[i].Type == DancerType.Human)
                dancers[i].GoIdeal();
            else
                dancers[i].StartDance();
        }
    }

    void HumanDance()
    {
        for (int i = 0; i < dancers.Count; i++)
        {
            if (dancers[i].Type == DancerType.Zombie)
                dancers[i].GoIdeal();
            else
                dancers[i].StartDance();
        }
    }

    void DancersIdeal()
    {
        for (int i = 0; i < dancers.Count; i++)
        {
            dancers[i].GoIdeal();
        }
    }

    private void OnDisable()
    {
        OnZombiesHighScore -= ZombieDance;
        OnHumanHighScore -= HumanDance;
        OnDancersIdeal -= DancersIdeal;
    }

}
