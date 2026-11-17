using UnityEngine;

// Put on one object in the GAME scene. Lead layer(s) always play; each instrument
// stem fades in only while you have at least one of that instrument.
public class BandMusic : MonoBehaviour
{
    [Header("Always-on lead layer (drag all 3 lead files here)")]
    public AudioClip[] leadClips;   // Tom Gtr Rhythm 1, Harmony solo, Jim Lead bus

    [Header("Instrument layers")]
    public AudioClip percussionClip;   // Drum Mega Bus
    public AudioClip keyboardClip;     // New Keys Bus
    public AudioClip bassClip;         // Bass
    public AudioClip rhythmClip;       // Acoustic Rhythm

    [Header("Mix")]
    public float layerVolume = 0.8f;
    public float fadeSpeed = 2f;

    private AudioSource percSrc, keysSrc, bassSrc, rhythmSrc;

    void Start()
    {
        // lead: every clip plays, full volume, always
        if (leadClips != null)
            foreach (var clip in leadClips)
                MakeSource(clip, true);

        percSrc   = MakeSource(percussionClip, false);
        keysSrc   = MakeSource(keyboardClip,    false);
        bassSrc   = MakeSource(bassClip,        false);
        rhythmSrc = MakeSource(rhythmClip,      false);
    }

    AudioSource MakeSource(AudioClip clip, bool startOn)
    {
        if (clip == null) return null;
        var src = gameObject.AddComponent<AudioSource>();
        src.clip = clip;
        src.loop = true;
        src.playOnAwake = false;
        src.volume = startOn ? layerVolume : 0f;   // all PLAY, we only change volume (stays in sync)
        src.Play();
        return src;
    }

    void Update()
    {
        var bm = BandManager.Instance;
        if (bm == null) return;

        Fade(percSrc,   bm.GetCount(BandManager.InstrumentType.Percussion)   > 0);
        Fade(keysSrc,   bm.GetCount(BandManager.InstrumentType.Keyboard)     > 0);
        Fade(bassSrc,   bm.GetCount(BandManager.InstrumentType.Bass)         > 0);
        Fade(rhythmSrc, bm.GetCount(BandManager.InstrumentType.RhythmGuitar) > 0);
    }

    void Fade(AudioSource src, bool on)
    {
        if (src == null) return;
        float target = on ? layerVolume : 0f;
        src.volume = Mathf.MoveTowards(src.volume, target, fadeSpeed * Time.deltaTime);
    }
}