using System;
using System.Collections;

namespace UnityModdingUtility
{
    public class InitializeAsync<T>
    {
        readonly IUnityCallbackProvider _callbackProvider;

        readonly Func<CoroutineOut<T>, IEnumerator> _coroutineFunc;
        CoroutineOut<T> _taskResult;
        IEnumerator _coroutine;
        int _updateCallbackIndex = -1;
        T _result;

        public InitializeAsync(IUnityCallbackProvider callbackProvider, Func<CoroutineOut<T>, IEnumerator> coroutineFunc)
        {
            _callbackProvider = callbackProvider;

            _coroutineFunc = coroutineFunc;
            State = AsyncOperationState.NotStarted;
        }

        public void StartAsyncOperation()
        {
            _taskResult = new CoroutineOut<T>();
            _coroutine = _coroutineFunc(_taskResult);
            _updateCallbackIndex = _callbackProvider.RegisterUpdateCallback(update);

            State = AsyncOperationState.InProgress;
        }

        public AsyncOperationState State { get; private set; }

        public T Get
        {
            get
            {
                if (State != AsyncOperationState.Complete)
                {
                    if (State == AsyncOperationState.NotStarted)
                        StartAsyncOperation();

                    step(true);
                }

                return _result;
            }
        }

        void update()
        {
            if (State == AsyncOperationState.InProgress)
            {
                step(false);
            }
        }

        void step(bool forceCompleteThisFrame)
        {
            if (forceCompleteThisFrame)
            {
                while (_coroutine.MoveNext())
                {
                }
            }

            if (forceCompleteThisFrame || !_coroutine.MoveNext())
            {
                State = AsyncOperationState.Complete;

                if (_updateCallbackIndex != -1)
                {
                    _callbackProvider.UnregisterUpdateCallback(_updateCallbackIndex);
                }

                _result = _taskResult.Result;
            }
        }
    }
}
