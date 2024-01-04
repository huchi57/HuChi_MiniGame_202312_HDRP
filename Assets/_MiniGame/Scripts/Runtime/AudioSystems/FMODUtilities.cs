using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMOD.Studio;
using FMODUnity;

namespace UrbanFox.MiniGame
{
    public static class FMODUtilities
    {
        private static readonly List<string> s_missingEventPaths = new List<string>();

        public static void PrintMissingEventPaths()
        {
            if (!s_missingEventPaths.IsNullOrEmpty())
            {
                FoxyLogger.LogError($"{s_missingEventPaths.Count} missing audio event paths found:\n - {string.Join("\n - ", s_missingEventPaths)}");
            }
        }

        public static bool IsEventExist(this EventReference eventRefAttribute)
        {
            try
            {
                RuntimeManager.GetEventDescription(eventRefAttribute);
                return true;
            }
            catch
            {
                if (!s_missingEventPaths.Contains(eventRefAttribute.Path))
                {
                    s_missingEventPaths.Add(eventRefAttribute.Path);
                    FoxyLogger.LogError($"Cannot find an event with path {eventRefAttribute.Path}");
                }
                return false;
            }
        }

        public static void PlayOneShot(this EventReference eventReference, Vector3 worldPosition)
        {
            if (eventReference.IsEventExist())
            {
                RuntimeManager.PlayOneShot(eventReference, worldPosition);
            }
        }

        public static void PlayOneShotAttached(this EventReference eventReference, GameObject gameObject)
        {
            if (eventReference.IsEventExist())
            {
                RuntimeManager.PlayOneShotAttached(eventReference, gameObject);
            }
        }

        public static void PlayOneShotAttached(this EventReference eventReference, Component component)
        {
            eventReference.PlayOneShotAttached(component.gameObject);
        }

        public static bool TryGetEventDurationInMilliseconds(this EventReference eventReference, out int milliseconds)
        {
            try
            {
                if (!eventReference.IsEventExist())
                {
                    milliseconds = 0;
                    return false;
                }
                var eventDescription = RuntimeManager.GetEventDescription(eventReference);
                if (eventDescription.getLength(out milliseconds) == RESULT.OK)
                {
                    return true;
                }
                return false;
            }
            catch
            {
                milliseconds = 0;
                return false;
            }
        }

        public static bool TryGetEventDurationInSeconds(this EventReference eventReference, out float seconds)
        {
            if (eventReference.TryGetEventDurationInMilliseconds(out var milliseconds))
            {
                seconds = (float)milliseconds / 1000;
                return true;
            }
            seconds = 0;
            return false;
        }

        public static void SetParameter(this EventInstance eventInstance, string name, float value, bool ignoreSeekSpeed = false)
        {
            eventInstance.setParameterByName(name, value, ignoreSeekSpeed);
        }

        public static void SetVolume(this EventInstance eventInstance, float volume)
        {
            eventInstance.setVolume(volume);
        }

        public static void SetPosition(this EventInstance eventInstance, Vector3 position)
        {
            eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        }

        public static bool TryGetVolume(this EventInstance eventInstance, out float volume)
        {
            if (eventInstance.getVolume(out volume) == RESULT.OK)
            {
                return true;
            }
            volume = 0;
            return true;
        }

        public static void StopByFadeOut(this EventInstance eventInstance)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        public static void StopImmediately(this EventInstance eventInstance)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
}
