import * as net from 'net';
import {Client} from './Client';
import * as ex from "./Exceptions";

class App {
    public run(): void {
        let server = net.createServer({}, function (socket) {
            try {
                console.log("Socket connected");
                let c: Client = new Client(socket);
            }
            catch (e) {
                switch(e.getType()){
                    case ex.LOGIC:
                        break;
                    case ex.SYNTAX:
                        break;
                    case ex.TIMEOUT:
                        break;
                    case ex.LOGIN:
                        break;
                }
            }
        });
        server.listen(1111, 'localhost', function () {
            console.log("Server is running on localhost:1111");
        });
    }
}

let app = new App();
app.run();
