/// <reference path="node_modules/@types/node/index.d.ts" />

let net = require('net');

class App {
    public run(): void {
        let server = net.createServer({}, function (socket: net.Socket) {
            console.log("Socket connected");
        });
        server.listen(1111, 'localhost', function () {
            console.log("Server is running");
        });
    }
}

let app = new App();
app.run();