import * as net from 'net';

export class CommunicationFacade {
    public static ServerUser(socket: net.Socket, callback: Function): boolean {
        return socket.write('100 LOGIN\r\n', callback);
    }

    public static ServerPassword(socket: net.Socket, callback: Function): boolean {
        return socket.write('101 PASSWORD\r\n', callback);
    }

    public static ServerMove(socket: net.Socket, callback: Function): boolean {
        return socket.write('102 MOVE\r\n', callback);
    }

    public static ServerTurnLeft(socket: net.Socket, callback: Function): boolean {
        return socket.write('103 TURN LEFT\r\n', callback);
    }

    public static ServerTurnRight(socket: net.Socket, callback: Function): boolean {
        return socket.write('104 TURN RIGHT\r\n', callback);
    }

    public static ServerPickUp(socket: net.Socket, callback: Function): boolean {
        return socket.write('105 GET MESSAGE\r\n', callback);
    }

    public static ServerOk(socket: net.Socket, callback: Function): boolean {
        return socket.write('200 OK\r\n', callback);
    }

    public static ServerLoginFailed(socket: net.Socket, callback: Function): boolean {
        return socket.write('300 LOGIN FAILED\r\n', callback);
    }

    public static ServerSyntaxError(socket: net.Socket, callback: Function): boolean {
        return socket.write('301 SYNTAX ERROR\r\n', callback);
    }

    public static ServerLogicError(socket: net.Socket, callback: Function): boolean {
        return socket.write('302 LOGIC ERROR\r\n', callback);
    }
}