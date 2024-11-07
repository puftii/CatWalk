using Unity.VisualScripting;
using UnityEngine;

public static class AudioManager
{
    public static void PlaySoundFromPosition(AudioClip sound, Vector3 position, float volume)
    {
        AudioSource.PlayClipAtPoint(sound, position, volume);
    }


    public static void PlaySoundFromClipArray(AudioClip[] array, Vector3 position, float volume) 
    {
        if (array.Length > 0)
        {
            int number = Random.Range(0, array.Length);
            AudioSource.PlayClipAtPoint(array[number], position, volume);
        }
    }
}
