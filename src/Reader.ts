export class Reader {
    private callback: Function;

    public buffer: string;

    public constructor() {
        this.buffer = '';
        this.setCallback(function () {
        });
    }

    public setCallback(callback: Function) {
        this.callback = callback;
        if(this.buffer.length !== 0)
            this.callback();
    }

    public appendText(text : string)
    {
        this.buffer = this.buffer + text;
        this.callback();
    }
}