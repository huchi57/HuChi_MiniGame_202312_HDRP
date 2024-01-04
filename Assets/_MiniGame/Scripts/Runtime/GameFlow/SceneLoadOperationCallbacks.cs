using System;

namespace UrbanFox.MiniGame
{
    public struct SceneLoadOperationCallbacks
    {
        public Action OnFadeOutStarts;
        public Action OnFadeOutCompletedAndIdleStarts;
        public Action OnLoadingOperationStarts;
        public Action OnLoadingOperationCompletedAndIdleStarts;
        public Action OnFadeInStarts;
        public Action OnFadeInCompleted;
    }
}
