import * as net from 'net';

class CommunicationFacade {
    public static ServerUser(socket: net.Socket): boolean {
        return socket.write('100 LOGIN\r\n');
    }

    public static ServerPassword(socket: net.Socket): boolean {
        return socket.write('101 PASSWORD\r\n');
    }

    public static ServerMove(socket: net.Socket): boolean {
        return socket.write('102 MOVE\r\n');
    }

    public static ServerTurnLeft(socket: net.Socket): boolean {
        return socket.write('103 TURN LEFT\r\n');
    }

    public static ServerTurnRight(socket: net.Socket) : boolean {
        return socket.write('104 TURN RIGHT\r\n');
    }

    public static ServerPickUp(socket: net.Socket): boolean {
        return socket.write('105 GET MESSAGE\r\n');
    }

    public static ServerOk(socket: net.Socket) : boolean {
        return socket.write('200 OK\r\n');
    }

    public static ServerLoginFailed(socket: net.Socket): boolean {
        return socket.write('300 LOGIN FAILED\r\n');
    }

    public static ServerSyntaxError(socket: net.Socket): boolean {
        return socket.write('301 SYNTAX ERROR\r\n');
    }

    public static ServerLogicError(socket: net.Socket): boolean {
        return socket.write('302 LOGIC ERROR\r\n');
    }


}