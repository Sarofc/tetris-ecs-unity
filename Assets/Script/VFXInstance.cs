using UnityEngine;
using System.Collections;
using Saro;

public class VFXInstance : MonoBehaviour, Saro.IPoolable<VFXInstance>
{
    public bool InPooled { get; set; }
    public Pool<VFXInstance> Pool { get; set; }

    private new ParticleSystem particleSystem;
    private Saro.Timer timer;

    private void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
    }

    public void Play()
    {
        particleSystem.Play();
        if (timer == null) timer = Saro.Timer.Register(particleSystem.main.duration, Free, null, false);
        timer.Restart();
    }

    private void Free()
    {
        particleSystem.Stop();
        Pool.Free(this);
    }
}
