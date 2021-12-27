using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuteMixer : MonoBehaviour
{
    public void MuteToggle(bool muted)
    {
        if (muted)
        {
            AudioListener.volume = -80;
        }    
    }
}
