﻿#if !BESTHTTP_DISABLE_SERVERSENT_EVENTS

using System;
using BestHTTP.Examples.Helpers;
using BestHTTP.ServerSentEvents;
using UnityEngine;
using UnityEngine.UI;

namespace BestHTTP.Examples.ServerSentEvents
{
    public class SimpleSample : BestHTTP.Examples.Helpers.SampleBase
    {
#pragma warning disable 0649

        [Tooltip("The url of the resource to use.")]
        [SerializeField]
        private string _path = "/sse";

        [SerializeField]
        private ScrollRect _scrollRect;

        [SerializeField]
        private RectTransform _contentRoot;

        [SerializeField]
        private TextListItem _listItemPrefab;

        [SerializeField]
        private int _maxListItemEntries = 100;

        [SerializeField]
        private Button _startButton;

        [SerializeField]
        private Button _closeButton;

#pragma warning restore

        private EventSource eventSource;

        protected override void Start()
        {
            base.Start();

            SetButtons(true, false);
        }

        private void OnDestroy()
        {
            if (this.eventSource != null)
            {
                this.eventSource.Close();
                this.eventSource = null;
            }
        }

        public void OnStartButton()
        {
            GUIHelper.RemoveChildren(this._contentRoot, 0);

            // Create the EventSource instance
            this.eventSource = new EventSource(new Uri(base.sampleSelector.BaseURL + this._path));

            // Subscribe to generic events
            this.eventSource.OnOpen += OnOpen;
            this.eventSource.OnClosed += OnClosed;
            this.eventSource.OnError += OnError;
            this.eventSource.OnStateChanged += this.OnStateChanged;
            this.eventSource.OnMessage += OnMessage;

            // Subscribe to an application specific event
            this.eventSource.On("datetime", OnDateTime);

            // Start to connect to the server
            this.eventSource.Open();

            AddText("Opening Server-Sent Events...");

            SetButtons(false, true);
        }

        public void OnCloseButton()
        {
            SetButtons(false, false);
            this.eventSource.Close();
        }

        private void OnOpen(EventSource eventSource)
        {
            AddText("Open");
        }

        private void OnClosed(EventSource eventSource)
        {
            AddText("Closed");

            this.eventSource = null;

            SetButtons(true, false);
        }

        private void OnError(EventSource eventSource, string error)
        {
            AddText($"Error: <color=red>{error}</color>");
        }

        private void OnStateChanged(EventSource eventSource, States oldState, States newState)
        {
            AddText($"State Changed {oldState} => {newState}");
        }

        private void OnMessage(EventSource eventSource, Message message)
        {
            AddText($"Message: <color=yellow>{message}</color>");
        }

        private void OnDateTime(EventSource eventSource, Message message)
        {
            DateTimeData dtData = LitJson.JsonMapper.ToObject<DateTimeData>(message.Data);

            AddText($"OnDateTime: <color=yellow>{dtData.ToString()}</color>");
        }

        private void SetButtons(bool start, bool close)
        {
            if (this._startButton != null)
                this._startButton.interactable = start;

            if (this._closeButton != null)
                this._closeButton.interactable = close;
        }

        private void AddText(string text)
        {
            GUIHelper.AddText(this._listItemPrefab, this._contentRoot, text, this._maxListItemEntries, this._scrollRect);
        }
    }

    internal sealed class DateTimeData
    {
#pragma warning disable 0649
        public int eventid;
        public string datetime;
#pragma warning restore

        public override string ToString()
        {
            return $"[DateTimeData EventId: {this.eventid}, DateTime: {this.datetime}]";
        }
    }
}
#endif