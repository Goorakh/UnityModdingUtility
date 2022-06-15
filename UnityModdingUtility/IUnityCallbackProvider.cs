using System;

namespace UnityModdingUtility
{
    public interface IUnityCallbackProvider
    {
        int RegisterUpdateCallback(Action update);
        void UnregisterUpdateCallback(int handle);
    }
}
