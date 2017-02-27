export class PausingTimer {

    private original: number;
    private callback: Function;
    private timeout;
    private remain: number;
    private start: Date;

    private called: boolean;

    public constructor(callback: Function, delay: number) {
        this.callback = callback;
        this.remain = delay || 0;
        this.original = delay;
        this.called = false;
        this.resume();
    }

    public pause() {
        if (this.timeout === null)
            return false;

        clearTimeout(this.timeout);
        this.timeout = null;
        this.remain -= ((new Date()).getTime() - this.start.getTime());
    }

    public resume() {
        let _this = this;
        this.start = new Date();
        //if is still running
        if (this.timeout !== null)
            this.pause();
        //execute
        this.timeout = setTimeout(() => {_this.exec()}, this.remain);
    }

    public exec(err = null) {
        if (this.called === true)
        {
            console.log("Callback was already called");
            return;
        }
        console.log("PauseTimer.exec executed with param: " + err);
        this.called = true;
        this.callback(err);
    }

    public repeat() {
        this.pause();
        this.remain = this.original;
        this.resume();
    }
}