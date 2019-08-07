using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class VFX : MonoBehaviour
{
    [SerializeField] private AudioSource clipAudioSource;
    [SerializeField] private AudioSource bgAudioSource;

    [Header("sfx"), Space(1)]
    [SerializeField] private SoundBank[] sounds;

    [Header("bg"), Space(1)]
    [SerializeField] private SoundBank[] bg;

    [Header("particle"), Space(1)]
    [SerializeField] private ParticleSystem vfx_hardDrop;
    [SerializeField] private VFXInstance vfx_clearLine;

    [Header("text vfx")]
    [SerializeField] private GameObject textStart;

    [SerializeField] private RenText textRen;

    [SerializeField] private GameObject textTetris;
    [SerializeField] private GameObject textMini;
    [SerializeField] private GameObject textTspin;
    [SerializeField] private GameObject textB2B;

    [SerializeField] private GameObject textSingle;
    [SerializeField] private GameObject textDouble;
    [SerializeField] private GameObject textTriple;

    [System.Serializable]
    public class SoundBank
    {
        public string key;
        public AudioClip clip;
    }


   private Saro.Pool<VFXInstance> m_vfx_LineClearPool;

    private Dictionary<string, AudioClip> m_soundsLookup = new Dictionary<string, AudioClip>();
    
    private void Start()
    {
        m_soundsLookup.Clear();

        for (int i = 0; i < sounds.Length; i++)
        {
            m_soundsLookup.Add(sounds[i].key, sounds[i].clip);
        }

        for (int i = 0; i < bg.Length; i++)
        {
            m_soundsLookup.Add(bg[i].key, bg[i].clip);
        }

        m_vfx_LineClearPool = new Saro.Pool<VFXInstance>(vfx_clearLine, null, 4, true);
    }


    #region Particle
    public void VFX_LineClear(int line)
    {
        var go = m_vfx_LineClearPool.New();
        if (go == null) return;
        go.transform.position = new Vector2(vfx_clearLine.transform.position.x, line);
        go.Play();
    }

    public void VFX_HardDrop(Vector2 position)
    {
        vfx_hardDrop.transform.position = position;
        if (vfx_hardDrop.isPlaying) vfx_hardDrop.Stop();
        vfx_hardDrop.Play();
    }
    #endregion

    #region Text VFX
    public void TextVFX_Start()
    {
        if (textStart.activeSelf) textStart.SetActive(false);
        textStart.SetActive(true);
    }

    public void HideTextRen()
    {
        textRen.gameObject.SetActive(false);
    }

    public void UpdateTextRen(int count)
    {
        if (!textRen.gameObject.activeSelf)
        {
            textRen.gameObject.SetActive(true);
        }
        textRen.RestartTween(count);
    }

    public void TextVFX_Tetris()
    {
        if(textTetris.activeSelf) textTetris.SetActive(false);
        textTetris.SetActive(true);
    }

    public void TextVFX_B2B()
    {
        if (textB2B.activeSelf) textB2B.SetActive(false);
        textB2B.SetActive(true);
    }

    public void TextVFX_TSpinSingle()
    {
        TextVFX_TSpin();
        if (textSingle.activeSelf) textSingle.SetActive(false);
        textSingle.SetActive(true);
    }

    public void TextVFX_TSpinDouble()
    {
        TextVFX_TSpin();
        if (textDouble.activeSelf) textDouble.SetActive(false);
        textDouble.SetActive(true);
    }

    public void TextVFX_TSpinTriple()
    {
        TextVFX_TSpin();
        if (textTriple.activeSelf) textTriple.SetActive(false);
        textTriple.SetActive(true);
    }

    internal void TextVFX_TSpinMiniSingle()
    {
        TextVFX_TSpinSingle();
        if (textMini.activeSelf) textMini.SetActive(false);
        textMini.SetActive(true);
    }

    internal void TextVFX_TSpinMiniDouble()
    {
        TextVFX_TSpinDouble();
        if (textMini.activeSelf) textMini.SetActive(false);
        textMini.SetActive(true);
    }

    private void TextVFX_TSpin()
    {
        if (textTspin.activeSelf) textTspin.SetActive(false);
        textTspin.SetActive(true);
    }
    #endregion

    #region SFX
    public void PlayClip(string key)
    {
        if (string.IsNullOrEmpty(key)) return;
        if(m_soundsLookup.TryGetValue(key, out AudioClip clip))
            clipAudioSource.PlayOneShot(clip);
    }

    public void PlayBG(string key)
    {
        if (string.IsNullOrEmpty(key)) return;
        if (m_soundsLookup.TryGetValue(key, out AudioClip clip))
        {
            bgAudioSource.clip = clip;
            bgAudioSource.loop = true;
            bgAudioSource.Play();
        }
    }

    #endregion

}
