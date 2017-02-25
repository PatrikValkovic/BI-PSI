export class Exception {
}

const TIMEOUT = 'timeout';
const LOGIN = 'login';
const SYNTAX = 'syntax';
const LOGIC = 'logic';


export class TimeoutException extends Exception {
    public getType(): string {
        return TIMEOUT;
    }
}

export class LoginException extends Exception {
    public getType(): string {
        return LOGIN;
    }
}

export class SyntaxException extends Exception {
    public getType(): string {
        return SYNTAX;
    }
}

export class LogicException extends Exception {
    public getType(): string {
        return LOGIC;
    }
}

