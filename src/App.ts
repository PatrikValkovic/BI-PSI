import * as net from 'net';
import {Client} from './Client';

class App {
    public run(): void {
        let server = net.createServer({}, function (socket) {
            let c: Client = new Client(socket);
            console.log("Socket connected");
        });
        server.listen(1111, 'localhost', function () {
            console.log("Server is running");
        });
    }
}

let app = new App();
app.run();
