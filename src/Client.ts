import * as net from 'net';


export class Client {
    private socket: net.Socket;
    private timeout;

    private timeouted(): void {

    }

    public constructor(socket: net.Socket) {
        this.socket = socket;
        this.timeout = setTimeout(this.timeouted, 1000);
    }


}