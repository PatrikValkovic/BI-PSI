import * as async from 'async';
import * as net from 'net';
import {Client} from './Client';
import * as ex from "./Exceptions";

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
                    c.getPosition(callback);
                }
            ], function (err, res) {
                if (err)
                    ex.SendError(socket, err);
            });
        });


        server.listen(1111, 'localhost', function () {
            console.log("Server is running on localhost:1111");
        });
    }
}

let app = new App();
app.run();
