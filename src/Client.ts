import * as net from 'net';

export class Client {
    private socket: net.Socket;
    private timeout;

    public timeoutCreate() {
        let _this = this;
        return function () {
            _this.socket.end();
            _this.socket.destroy();
        };
    }

    public constructor(socket: net.Socket) {
        this.socket = socket;
        this.timeout = setTimeout(this.timeoutCreate(), 1000);
    }
}