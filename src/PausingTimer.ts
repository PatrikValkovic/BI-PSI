export class PausingTimer {

    private callback: Function;
    private timeout;
    private remain: number;
    private start: Date;

    public constructor(callback: Function, delay: number) {
        this.callback = callback;
        this.remain = delay || 0;
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
        this.timeout = setTimeout(this.callback,this.remain);
    }
}