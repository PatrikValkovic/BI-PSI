import * as async from 'async';
import * as net from 'net';
import {Client} from './Client';
import * as ex from "./Exceptions";
import {CommunicationFacade} from "./CommunicationFacade";

class App {
    public run(): void {
        let server = net.createServer({}, function (socket) {
            console.log("Socket connected");
            let c: Client = new Client(socket);

            async.series([
                function (callback: Function) {
                    c.authenticate(callback);
                },
                function (callback) {
                    c.navigate(callback);
                }
            ], function (err, res) {
                if (err) {
                    let delSocket = function () {
                        socket.end();
                        socket.destroy();
                    };
                    switch (err) {
                        case ex.LOGIC:
                            CommunicationFacade.ServerLogicError(socket, delSocket);
                            console.log("Socket deleted because of logic error");
                            break;
                        case ex.SYNTAX:
                            CommunicationFacade.ServerSyntaxError(socket, delSocket);
                            console.log("Socket deleted because of syntax error");
                            break;
                        case ex.TIMEOUT:
                            console.log("Socket deleted because of timeout");
                            delSocket();
                            break;
                        case ex.LOGIN:
                            CommunicationFacade.ServerLoginFailed(socket, delSocket);
                            console.log("Socket deleted because of login error");
                            break;
                    }
                }
            });
        });


        server.listen(1111, 'localhost', function () {
            console.log("Server is running on localhost:1111");
        });
    }
}

let app = new App();
app.run();
