using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace UISystem.Core
{
    public class ViewManager : MonoBehaviour
    {
        public static ViewManager Instance { get; private set; }

        public event UnityAction<View> ViewShow;
        public void OnViewShow(View view) => ViewShow?.Invoke(view);

        [SerializeField] private View _startingView;
        [SerializeField] private View[] _allViews;

        private readonly Stack<View> _viewHistory = new();
        private View _activeView;

        private void Awake() => Instance = this;

        private void Start()
        {
            foreach (View view in _allViews)
            {
                view.Initialize();
                view.Hide();
            }

            if (_startingView == null)
                return;
            ShowView(_startingView);
        }

        public void ShowView<T>(bool saveInHistory = true)
        {
            foreach (View view in _allViews)
            {
                if (view is not T)
                    continue;
                if (_activeView != null)
                {
                    if (saveInHistory)
                    {
                        Instance._viewHistory.Push(_activeView);
                    }

                    _activeView.Hide();
                }

                view.Show();
                _activeView = view;
                OnViewShow(_startingView);
            }
        }

        public void ShowView(View view, bool saveInHistory = true)
        {
            if (_activeView != null)
            {
                if (saveInHistory)
                {
                    _viewHistory.Push(Instance._activeView);
                }

                _activeView.Hide();
            }

            OnViewShow(_startingView);
            view.Show();
            _activeView = view;
        }

        public void ShowLastView()
        {
            if (Instance._viewHistory.Count <= 0)
                return;
            ShowView(Instance._viewHistory.Pop());
        }
    }
}