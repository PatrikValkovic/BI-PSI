import {Errors} from './Constants';
import {PausingTimer} from './PausingTimer';

export class Charging {

    private getTimeoutFn: Function;
    private charging: boolean = false;
    private timeout: PausingTimer;
    private forceMessagesFn : Function;
    private errorFn : Function;

    public constructor(getTimeoutFn: Function, forceMessages: Function) {
        this.getTimeoutFn = getTimeoutFn;
        this.forceMessagesFn = forceMessages;
        let _this = this;
        this.timeout = new PausingTimer(function () {
            console.log("Recharging timeout");
            _this.getTimeoutFn().exec(Errors.timeout);
        }, 5000);
        this.timeout.pause();

    }

    public setErrorFn(callback : Function){
        this.errorFn = callback;
    }

    private static startWith(text: string, required: string): boolean {
        let lenText = text.length;
        let lenReq = required.length;
        for (let i = 0; i < Math.min(lenText, lenReq); i++)
            if (text.charAt(i) !== required.charAt(i))
                return false;
        return true;
    }

    public couldHandle(text: string) {
        //if recharging get everything
        if (this.charging === true)
            return true;

        return Charging.startWith(text, 'RECHARGING') || Charging.startWith(text, 'FULL POWER');
    }

    public createArriveTimeout(): Function {
        let _this = this;
        return function () {
            if (_this.charging)
                _this.timeout.repeat();
        }
    }

    public handle(text): boolean {
        console.log("Arrive into charging: " + text);

        if (text === "RECHARGING") {
            console.log("Charging message arrive into middleware");
            this.charging = true;
            this.getTimeoutFn().pause();
            this.timeout.repeat();
            return false;
        }

        if (text === "FULL POWER" && this.charging === true) {
            console.log("Full power message arrive into middleware");
            this.timeout.pause();
            this.charging = false;
            this.getTimeoutFn().repeat();
            this.forceMessagesFn();
            return false;
        }

        if (this.charging === true) {
            console.log("Message during recharging - error");
            this.timeout.pause();
            this.errorFn(Errors.logic);
            return false;
        }

        console.log("Charging ignore this message");
        return true;
    }
}