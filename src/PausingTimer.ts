export class PausingTimer {

    private original: number;
    private callback: Function;
    private timeout;
    private remain: number;
    private start: Date;

    public constructor(callback: Function, delay: number) {
        this.callback = callback;
        this.remain = delay || 0;
        this.original = delay;
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
        this.start = new Date();
        if(this.timeout !== null)
            this.pause();
        this.timeout = setTimeout(this.callback,this.remain);
    }

    public repeat() {
        this.pause();
        this.remain = this.original;
        this.resume();
    }
}