import {Errors} from './Constants';
import {Charging} from './Charging';

export class Reader {
    private callback: Function;
    private arrive: Function[];
    private charging: Charging[];

    public buffer: string;
    public maxLength: number;


    public constructor(onMessageArrive: Function) {
        this.buffer = '';
        this.setCallback(() => {});
        this.arrive = [onMessageArrive];
        this.charging = [];
    }

    public attachArriveMessage(fn: Function) {
        this.arrive.push(fn);
    }

    public setCallback(callback: Function) {
        let _this = this;
        this.callback = callback;

        this.obtainMessage(function (parsed) {
            if (parsed === null)
                return;

            _this.callback(parsed);
        });
    }

    private getTextInBuffer(): string {
        return this.buffer.replace(/\r/g, '\\r')
            .replace(/\n/g, '\\n')
            .replace(/\0/g, '\\0')
    }

    //TRUE if accept
    private couldMiddlewareHandle(text): boolean {
        for (let i = 0, l = this.charging.length; i < l; i++)
            if (this.charging[i].couldHandle(text) === true)
                return true;
        return false;
    }

    private handleMiddleware(text) {
        for (let i = 0, l = this.charging.length; i < l; i++)
            if (this.charging[i].handle(text) === false)
                return false;
        return true;
    }

    private obtainMessage(callback: Function) {
        let posOfDelimiter = this.buffer.indexOf('\r\n');
        if (posOfDelimiter < 0) //not accepted whole name yet
        {
            if (this.buffer.length > this.maxLength && this.couldMiddlewareHandle(this.buffer) === false)
                return callback(Errors.overLength); //already arrive more symbols that require
            return null;
        }
        let parsed: string = this.buffer.substring(0, posOfDelimiter);
        if (this.handleMiddleware(parsed) === false) {
            this.buffer = this.buffer.substring(parsed.length + 2);
            return;
        }

        if (parsed.length > this.maxLength)
            return callback(Errors.overLength);

        this.buffer = this.buffer.substring(parsed.length + 2);
        console.log("Parsed message, message: " + parsed + " ? buffer: " + this.getTextInBuffer());
        callback(parsed);
    }

    public appendText(text: string) {
        this.buffer = this.buffer + text;
        console.log("Text obtained, length: " + this.buffer.length + " ? bufer: " + this.getTextInBuffer());

        this.arrive.forEach((fn) => {fn();});

        let _this = this;
        this.obtainMessage(function (text) {
            if (text === null)
                return;

            _this.callback(text);
        });
    }

    public registerMiddleware(charger: Charging) {
        this.charging.push(charger);
    }
}