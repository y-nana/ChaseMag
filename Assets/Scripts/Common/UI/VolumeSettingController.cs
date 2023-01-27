using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class VolumeSettingController : MonoBehaviour
{
    [SerializeField]
    private AudioMixer mixer;

    [SerializeField]
    private SliderController bgmSlider;

    [SerializeField]
    private SliderController seSlider;

    private readonly string bgmGroup = "BGM_Volume";
    private readonly string seGroup = "SE_Volume";

    // Start is called before the first frame update
    void Start()
    {
        mixer.GetFloat(bgmGroup, out float bgmValue);
        
        bgmSlider.value = bgmValue;
        bgmSlider.changeAct = ChangeBGM;
        mixer.GetFloat(seGroup, out float seValue);

        seSlider.value = seValue;
        seSlider.changeAct = ChangeSE;

    }

    public void ChangeBGM(float value)
    {
        mixer.SetFloat(bgmGroup, value);
    }

    public void ChangeSE(float value)
    {
        mixer.SetFloat(seGroup, value);
    }

}
