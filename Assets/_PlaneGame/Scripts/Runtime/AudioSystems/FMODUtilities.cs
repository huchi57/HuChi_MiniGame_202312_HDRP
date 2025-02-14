using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FMOD;
using FMOD.Studio;
using FMODUnity;

namespace UrbanFox.MiniGame
{
    public static class FMODUtilities
    {
        private static readonly Dictionary<GUID, string> s_missingEvents = new Dictionary<GUID, string>();

        public static void PrintMissingEventGUIDsOrPaths()
        {
            if (!s_missingEvents.IsNullOrEmpty())
            {
                string missingEventList;
#if UNITY_EDITOR
                missingEventList = string.Join("\n - ", s_missingEvents.Values.ToList());
#else
                missingEventList = string.Join("\n - ", s_missingEvents.Keys.ToList());
#endif
                FoxyLogger.LogError($"{s_missingEvents.Count} missing audio event paths found:\n - {missingEventList}");
            }
        }

        public static bool IsEventExist(this EventReference eventRefAttribute)
        {
            try
            {
                if (eventRefAttribute.Guid.IsNull)
                {
                    FoxyLogger.LogWarning($"An empty FMOD event has been found. Check callstack to check if any blank audio is in the project.");
                    return false;
                }
                RuntimeManager.GetEventDescription(eventRefAttribute);
                return true;
            }
            catch
            {
                if (!s_missingEvents.ContainsKey(eventRefAttribute.Guid))
                {
#if UNITY_EDITOR
                    s_missingEvents.TryAdd(eventRefAttribute.Guid, eventRefAttribute.Path);
                    FoxyLogger.LogError($"Cannot find an event with path {eventRefAttribute.Path}");
#else
                    s_missingEvents.TryAdd(eventRefAttribute.Guid, string.Empty);
                    FoxyLogger.LogError($"Cannot find an event with GUID {eventRefAttribute.Guid}");
#endif
                }
                return false;
            }
        }

        public static void PlayOneShot(this EventReference eventReference)
        {
            eventReference.PlayOneShot(default);
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

        public static EventInstance Play(this EventReference eventReference)
        {
            return eventReference.Play(default);
        }

        public static EventInstance Play(this EventReference eventReference, Vector3 worldPosition)
        {
            try
            {
                var instance = RuntimeManager.CreateInstance(eventReference);
                instance.set3DAttributes(worldPosition.To3DAttributes());
                instance.start();
                return instance;
            }
            catch
            {
                IsEventExist(eventReference);
                throw;
            }
        }

        public static EventInstance PlayWithMinMaxDistanceOverride(this EventReference eventReference, Vector3 worldPosition, float minDistance, float maxDistance)
        {
            var instance = eventReference.Play(worldPosition);
            instance.SetMinMaxDistanceOverride(minDistance, maxDistance);
            return instance;
        }

        public static void SetMinMaxDistanceOverride(this EventInstance eventInstance, float minDistance, float maxDistance)
        {
            eventInstance.setProperty(EVENT_PROPERTY.MINIMUM_DISTANCE, minDistance);
            eventInstance.setProperty(EVENT_PROPERTY.MAXIMUM_DISTANCE, maxDistance);
        }

        public static void StopByFadeOut(this EventInstance eventInstance)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            eventInstance.release();
        }

        public static void StopImmediately(this EventInstance eventInstance)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
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
    }
}
