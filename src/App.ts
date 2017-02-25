import * as async from 'async/series';
import * as net from 'net';
import {Client} from './Client';
import * as ex from "./Exceptions";

class App {
    public run(): void {
        let server = net.createServer({}, function (socket) {
            console.log("Socket connected");
            let c: Client = new Client(socket);

            async.series([function (callback: Function) {
                c.authenticate(callback);
            }], function (err, res) {
                if (err) {
                    switch (err) {
                        case ex.LOGIC:
                            break;
                        case ex.SYNTAX:
                            break;
                        case ex.TIMEOUT:
                            socket.end();
                            socket.destroy();
                            console.log("Socket deleted because of timeout");
                        case ex.LOGIN:
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
