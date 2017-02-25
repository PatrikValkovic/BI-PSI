import * as net from 'net';
import {CommunicationFacade} from './CommunicationFacade';

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

    public readCreate() {
        let _this = this;
        return function(data : string) {
            console.log('Readed: ' + data);
        }
    }

    public constructor(socket: net.Socket) {
        this.socket = socket;
        this.socket.on('data',this.readCreate());
        this.timeout = setTimeout(this.timeoutCreate(), 1000);
    }

    public authenticate() {
        CommunicationFacade.ServerUser(this.socket);
    }
}