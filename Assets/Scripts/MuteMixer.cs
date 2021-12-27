using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MuteAudio : MonoBehaviour
{
    public void MuteToggle(bool muted)
    {
        if (muted)
        {
            AudioListener.pause = true;
        }
        else if (!muted)
        {
            AudioListener.pause = false;
        }
    }

}
