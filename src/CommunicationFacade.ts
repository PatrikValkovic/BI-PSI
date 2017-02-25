import * as net from 'net';

export class CommunicationFacade {
    public static ServerUser(socket: net.Socket, callback: Function): boolean {
        console.log("Sending: 100 LOGIN");
        return socket.write('100 LOGIN\r\n', callback);
    }

    public static ServerPassword(socket: net.Socket, callback: Function): boolean {
        console.log("Sending: 101 PASSWORD");
        return socket.write('101 PASSWORD\r\n', callback);
    }

    public static ServerMove(socket: net.Socket, callback: Function): boolean {
        console.log("Sending: 102 MOVE");
        return socket.write('102 MOVE\r\n', callback);
    }

    public static ServerTurnLeft(socket: net.Socket, callback: Function): boolean {
        console.log("Sending: 103 TURN LEFT");
        return socket.write('103 TURN LEFT\r\n', callback);
    }

    public static ServerTurnRight(socket: net.Socket, callback: Function): boolean {
        console.log("Sending: 104 TURN RIGHT");
        return socket.write('104 TURN RIGHT\r\n', callback);
    }

    public static ServerPickUp(socket: net.Socket, callback: Function): boolean {
        console.log("Sending: 105 GET MESSAGE");
        return socket.write('105 GET MESSAGE\r\n', callback);
    }

    public static ServerOk(socket: net.Socket, callback: Function): boolean {
        console.log("Sending: 200 OK");
        return socket.write('200 OK\r\n', callback);
    }

    public static ServerLoginFailed(socket: net.Socket, callback: Function): boolean {
        console.log("Sending: 300 LOGIN FAILED");
        return socket.write('300 LOGIN FAILED\r\n', callback);
    }

    public static ServerSyntaxError(socket: net.Socket, callback: Function): boolean {
        console.log("Sending: 301 SYNTAX ERROR");
        return socket.write('301 SYNTAX ERROR\r\n', callback);
    }

    public static ServerLogicError(socket: net.Socket, callback: Function): boolean {
        console.log("Sending: 302 LOGIC ERROR");
        return socket.write('302 LOGIC ERROR\r\n', callback);
    }

    public static ServerMyError(socket: net.Socket, callback: Function): boolean {
        console.log("Sending: 400 MY ERROR");
        return socket.write('400 MY ERROR\r\n', callback);
    }
}