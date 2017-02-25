export class Exception {
}

export class TimeoutException extends Exception {
    private type = 'timeout';
}

export class LoginException extends Exception {
    private type = 'login';
}

export class SyntaxException extends Exception {
    private type = 'syntax';
}

export class LogicException extends Exception {
    private type = 'logic';
}

