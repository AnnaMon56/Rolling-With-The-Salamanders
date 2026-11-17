using UnityEngine;

// Put on one object in the MENU scene. Plays all stems together, full volume.
public class MenuMusic : MonoBehaviour
{
    public AudioClip[] stems;   // drag in every track you want playing on the menu
    public float volume = 0.8f;

    void Start()
    {
        foreach (var clip in stems)
        {
            if (clip == null) continue;
            var src = gameObject.AddComponent<AudioSource>();
            src.clip = clip;
            src.loop = true;
            src.volume = volume;
            src.playOnAwake = false;
            src.Play();   // all start together, stay in sync
        }
    }
}