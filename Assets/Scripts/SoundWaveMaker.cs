using UnityEngine;
using UnityEngine.Events;

public class SoundWaveMaker : MonoBehaviour
{
    public UnityEvent onGenerateSoundWave;

    public void MakeSoundWave()
    {
        onGenerateSoundWave?.Invoke();
    }
}
