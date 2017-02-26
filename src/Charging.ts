import {Client} from "./Client";
import {Errors} from './Constants';

export class Charging {

    private getTimeout: Function;
    private handled: boolean;
    private timeout;

    public constructor(getTimeout: Function) {
        this.handled = false;
        this.getTimeout = getTimeout;
        this.timeout = null;
    }

    private static startWith(text : string, required : string): boolean {
        let lenText = text.length;
        let lenReq = required.length;
        for(let i=0;i<Math.min(lenText,lenReq);i++)
            if(text.charAt(i) !== required.charAt(i))
                return false;
        return true;
    }

    public couldHandle(text: string) {
        //if recharging get everything
        if (this.handled === true)
            return true;

        return Charging.startWith(text,'RECHARGING') || Charging.startWith(text,'FULL POWER');
    }

    public handle(text): boolean {
        let _this = this;
        console.log("Arrive into charging: " + text);

        if (text === "RECHARGING") {
            console.log("Charging message arrive into middleware");
            this.handled = true;
            this.getTimeout().pause();
            this.timeout = setTimeout(function () {
                console.log("Recharging timeout");
                _this.getTimeout().exec(Errors.timeout);
            }, 5000);
            return false;
        }

        if (text === "FULL POWER" && this.handled === true) {
            console.log("Full power message arrive into middleware");
            clearTimeout(this.timeout);
            this.timeout = null;
            this.handled = false;
            this.getTimeout().repeat();
            return false;
        }

        if (this.handled === true) {
            console.log("Message during recharging - error");
            this.getTimeout().exec(Errors.logic);
            return false;
        }

        console.log("Charging ignore this message");
        return true;
    }
}