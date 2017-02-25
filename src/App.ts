import * as async from 'async';
import * as net from 'net';
import {Client} from './Client';
import * as ex from "./Exceptions";
import {Errors} from "./Constants";

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
                },
                function(callback){
                    c.navigate(callback);
                },
                function(callback){
                    c.getMessage(callback);
                }
            ], function (err, res) {
                let finish = function(err, data){
                    if (err)
                        return ex.SendError(socket, err);
                    console.log("Robot finished his work");
                    socket.end();
                    socket.destroy();
                };

                if(err === Errors.onPosition)
                    return c.getMessage(finish);
                finish(err,res);
            });
        });


        server.listen(1111, 'localhost', function () {
            console.log("Server is running on localhost:1111");
        });
    }
}

let app = new App();
app.run();
