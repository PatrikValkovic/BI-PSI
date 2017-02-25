export class Exception {
}

export const TIMEOUT = 'timeout';
export const LOGIN = 'login';
export const SYNTAX = 'syntax';
export const LOGIC = 'logic';


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

