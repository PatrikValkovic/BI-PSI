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

    private startWith(text : string, required : string): boolean {
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

        return this.startWith(text,'RECHARGING') || this.startWith(text,'FULL POWER');
    }

    public handle(text): boolean {
        let _this = this;
        console.log("Arrive into charging: " + text);
        if (text === "RECHARGING") {
            this.handled = true;
            this.getTimeout().pause();
            this.timeout = setTimeout(function () {
                console.log("Recharging timeout");
                _this.getTimeout().exec(Errors.timeout);
            }, 5000);
            return false;
        }

        if (text === "FULL POWER" && this.handled === true) {
            console.log("Robot now have full power and server accepts messages");
            clearTimeout(this.timeout);
            this.timeout = null;
            this.handled = false;
            this.getTimeout().repeat();
            return false;
        }

        if (this.handled === true) {
            console.log("Message during recarching - error");
            this.getTimeout().exec(Errors.logic);
            return false;
        }

        console.log("Charging ignore this message");
        return true;
    }
}