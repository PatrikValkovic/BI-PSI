import * as net from 'net';
import {MyClient} from './MyClient';

class App {
    public run(): void {
        let server = net.createServer({}, function (socket) {
            let c: MyClient = new MyClient(socket);
            console.log("Socket connected");
        });
        server.listen(1111, 'localhost', function () {
            console.log("Server is running");
        });
    }
}

let app = new App();
app.run();
