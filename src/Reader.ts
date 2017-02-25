import {Errors} from './Constants';

export class Reader {
    private callback: Function;

    public buffer: string;
    public maxLength: number;

    public constructor() {
        this.buffer = '';
        this.setCallback(function () {
        });
    }

    public setCallback(callback: Function) {
        this.callback = callback;
        if (this.buffer.length !== 0)
            this.callback();
    }

    public appendText(text: string) {
        this.buffer = this.buffer + text;
        console.log("Text obtained, current buffer: " + this.buffer.replace(/\r/g, '\\r')
                        .replace(/\n/g, '\\n')
                        .replace(/\0/g, '\\0'));

        let posOfDelimiter = this.buffer.indexOf('\r\n');
        if (posOfDelimiter < 0) //not accepted whole name yet
        {
            if (this.buffer.length > this.maxLength) //already arrive more symbols that require
                this.callback(Errors.overLength);
            return;
        }

        let parsed = this.buffer.substring(0, posOfDelimiter);
        if (parsed.length > this.maxLength)
            this.callback(Errors.overLength);

        this.buffer = this.buffer.substring(parsed.length + 2);
        this.callback(parsed);
    }
}