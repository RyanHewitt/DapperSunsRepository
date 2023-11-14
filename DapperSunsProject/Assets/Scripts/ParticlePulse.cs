using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePulse : MonoBehaviour
{
    public ParticleSystem particleQuick;
    public ParticleSystem particleLong;
    public GameObject objectToPlayOn;

    [SerializeField] float life = 1;
    [SerializeField] int StepsOrig = 4;

    private int steps;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnBeatEvent += DoBeat;
        steps = StepsOrig;
        
    }
    IEnumerator RemoveTimer(ParticleSystem copyDelete)
    {
        yield return new WaitForSeconds(life);
        Destroy(copyDelete);
    }

    void DoBeat()
    {
        ParticleSystem copy;
        steps--;
        if(steps <= 0)
        {
            steps = StepsOrig;
            copy = Instantiate(particleLong, objectToPlayOn.transform);
        }
        else
        {
            copy = Instantiate(particleQuick, objectToPlayOn.transform);
        }
        StartCoroutine(RemoveTimer(copy));
    }
}
