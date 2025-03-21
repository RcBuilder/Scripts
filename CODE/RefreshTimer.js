﻿function RefreshTimer(selector) {
    const initTime = { minutes: 0, seconds: 30 };

    // properties
    let timerId, refreshTimerId;
    let oTimer = document.querySelector(selector);
    let startTime = null;

    // private methods
    PrintTime = () => {
        let minutes = startTime.minutes.toString();
        let seconds = startTime.seconds.toString();
        if (minutes.length == 1) minutes = `0${minutes}`;
        if (seconds.length == 1) seconds = `0${seconds}`;

        oTimer.innerHTML = `${minutes}:${seconds}`;
    }
    UpdateTime = () => {
        if (startTime.seconds == 0) {
            startTime.seconds = 59;
            startTime.minutes--;
        }
        else startTime.seconds--;
    }
    NeedToRefresh = () => {
        return startTime.minutes == 0 && startTime.seconds == 0;
    }
    Tick = () => {
        ///console.log('Tick');
        UpdateTime();
        PrintTime();

        if (NeedToRefresh()) Refresh();
    }
    Refresh = () => {
        ///console.log('Refresh');
        this.Reset();
        document.location.reload();
    }

    // public methods
    this.Reset = () => {
        startTime = { ...initTime };
    }
    this.Start = () => {
        if (!oTimer) return;
        timerId = setInterval(Tick, 1000);
    }
    this.Stop = () => {
        if (!oTimer) return;
        clearInterval(timerId);
        clearTimeout(refreshTimerId);
    }

    // init
    (() => {
        if (!oTimer) return;
        console.log('Init RefreshTimer')
        this.Reset();
        PrintTime();
    })();
}