import * as cons from './Constants';

export class Exception {
}

export class TimeoutException extends Exception {
    public getType(): number {
        return cons.Errors.timeout;
    }
}

export class LoginException extends Exception {
    public getType(): number {
        return cons.Errors.login;
    }
}

export class SyntaxException extends Exception {
    public getType(): number {
        return cons.Errors.syntax;
    }
}

export class LogicException extends Exception {
    public getType(): number {
        return cons.Errors.logic;
    }
}

