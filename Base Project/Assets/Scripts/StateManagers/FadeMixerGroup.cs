﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
public class FadeMixerGroup
{
    public static List<string> exposedBGMParams = new List<string>() { "TutorialBGMVol", "Stage1BGMVol", "Stage2BGMVol", "Stage3BGMVol", "BossBGMVol" };
    public static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
    {
        float currentTime = 0;
        float currentVol;
        audioMixer.GetFloat(exposedParam, out currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }
        yield break;
    }

    public static void TurnOffSound(AudioMixer audioMixer)
    {
        foreach (string param in exposedBGMParams)
        {
            audioMixer.SetFloat(param, -80f);
        }
    }

    public static void TransitToSnapshot(AudioMixerSnapshot snapshot)
    {
        snapshot.TransitionTo(1f);
    }
}