import * as net from 'net';
import {CommunicationFacade} from './CommunicationFacade';
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

export function SendError(socket: net.Socket, error: number) {
    let delSocket = function () {
        socket.end();
        socket.destroy();
    };
    switch (error) {
        case cons.Errors.logic:
            CommunicationFacade.ServerLogicError(socket, delSocket);
            console.log("Socket deleted because of logic error");
            delSocket();
            break;
        case cons.Errors.syntax:
            CommunicationFacade.ServerSyntaxError(socket, delSocket);
            console.log("Socket deleted because of syntax error");
            delSocket();
            break;
        case cons.Errors.timeout:
            console.log("Socket deleted because of timeout");
            delSocket();
            break;
        case cons.Errors.login:
            CommunicationFacade.ServerLoginFailed(socket, delSocket);
            console.log("Socket deleted because of login error");
            delSocket();
            break;
    }
}

