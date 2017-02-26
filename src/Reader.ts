import {Errors} from './Constants';

export class Reader {
    private callback: Function;

    public buffer: string;
    public maxLength: number;

    public constructor() {
        this.buffer = '';
        this.setCallback(()=>{});
    }

    public pullMessage() : boolean {
        let parsed = this.obtainMessage();
        if(parsed === null)
            return false;

        this.callback(parsed);
        return true;
    }

    public setCallback(callback: Function) {
        this.callback = callback;

        let parsed = this.obtainMessage();
        if(parsed === null)
            return;

        this.callback(parsed);
    }

    private getTextInBuffer(): string {
        return this.buffer.replace(/\r/g, '\\r')
            .replace(/\n/g, '\\n')
            .replace(/\0/g, '\\0')
    }

    private obtainMessage() {
        let posOfDelimiter = this.buffer.indexOf('\r\n');
        if (posOfDelimiter < 0) //not accepted whole name yet
        {
            if (this.buffer.length > this.maxLength) //already arrive more symbols that require
                return Errors.overLength;
            return null;
        }
        let parsed : string = this.buffer.substring(0, posOfDelimiter);
        if (parsed.length > this.maxLength)
            return Errors.overLength;

        this.buffer = this.buffer.substring(parsed.length + 2);
        console.log("Parsed message, message: " + parsed + " ? buffer: " + this.getTextInBuffer());
        return parsed;
    }

    public appendText(text: string) {
        this.buffer = this.buffer + text;
        console.log("Text obtained, length: " + this.buffer.length + " ? bufer: " + this.getTextInBuffer());

        let parsed = this.obtainMessage();
        if(parsed === null)
            return;

        this.callback(parsed);
    }
}