import * as net from 'net';


export class MyClient {
    private socket : net.Socket;

    public constructor(socket : net.Socket) {
        this.socket = socket;
    }

    public hello() {

    }
}