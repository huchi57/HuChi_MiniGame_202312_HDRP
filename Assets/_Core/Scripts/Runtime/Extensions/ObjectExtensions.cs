using UnityEngine;

namespace UrbanFox
{
    public static class ObjectExtensions
    {
        public static bool IsNull(this object obj, string objName, Object context, bool logErrorOnNull = false)
        {
            if (obj != null)
            {
                return false;
            }
            if (logErrorOnNull)
            {
                if (!objName.IsNullOrEmpty() && context != null)
                {
                    FoxyLogger.LogError($"There is a missing reference of {objName} on {context}! Click to ping the missing context.", context);
                }
                else if (!objName.IsNullOrEmpty() && context == null)
                {
                    FoxyLogger.LogError($"There is a missing reference of {objName}!");
                }
                else if (objName.IsNullOrEmpty() && context != null)
                {
                    FoxyLogger.LogError($"There is a missing reference on {context}! Click to ping the missing context.", context);
                }
                else
                {
                    FoxyLogger.LogError($"There is a missing reference!");
                }
            }
            return true;
        }

        public static bool IsNull(this object obj, string objName, bool logErrorOnNull = false)
        {
            return obj.IsNull(objName, null, logErrorOnNull);
        }

        public static bool IsNull(this object obj, Object context, bool logErrorOnNull = false)
        {
            return obj.IsNull(null, context, logErrorOnNull); ;
        }

        public static bool IsNull(this object obj, bool logErrorOnNull = false)
        {
            return obj.IsNull(null, null, logErrorOnNull);
        }
    }
}
