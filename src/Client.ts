import * as net from 'net';
import * as exception from './Exceptions';
import {CommunicationFacade} from './CommunicationFacade';

export class Client {
    private socket: net.Socket;
    private timeout;

    public constructor(socket: net.Socket) {
        this.socket = socket;
    }

    public authenticate(callback) {

        this.timeout = setTimeout(function () {
            callback(exception.TIMEOUT, null);
        }, 1000);

        CommunicationFacade.ServerUser(this.socket);
    }
}