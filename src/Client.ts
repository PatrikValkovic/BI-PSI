import * as net from 'net';
import * as async from 'async';
import * as exception from './Exceptions';
import {CommunicationFacade} from './CommunicationFacade';
import {Reader} from "./Reader";
import {SYNTAX} from "./Exceptions";

export class Client {
    private socket: net.Socket;
    private timeout;
    private reader: Reader;

    public constructor(socket: net.Socket) {
        let _this = this;
        this.socket = socket;
        this.reader = new Reader();
        this.socket.addListener('data', function (data: string) {
            _this.reader.appendText(data);
        });
    }

    public authenticate(callback) {

        let _this = this;
        let timeoutLength = 1000;
        let name = '';

        let createTimeout = function (inCallback) {
            _this.timeout = setTimeout(function () {
                callback(exception.TIMEOUT);
            }, timeoutLength);
            inCallback();
        };
        let deleteTimeout = function (inCallback) {
            clearTimeout(_this.timeout);
            inCallback();
        };


        async.series([
            //SEND USER PACKET
            function (callback) {
                CommunicationFacade.ServerUser(_this.socket, callback);
            },
            //WAIT FOR RESPOND
            createTimeout,
            //GET USERNAME
            function (callback) {
                _this.reader.setCallback(function () {
                    let posOfDelimiter = _this.reader.buffer.indexOf('\r\n');
                    if (posOfDelimiter < 0) //not accepted whole name yet
                    {
                        if (_this.reader.buffer.length > 100) //already arrive more than 100 symbols
                            callback(exception.SYNTAX);
                    }
                    else {
                        name = _this.reader.buffer.substring(0, posOfDelimiter);
                        if (name.length > 100)
                            callback(exception.SYNTAX);
                        //username is now whole and correct
                        _this.reader.buffer = _this.reader.buffer.substring(name.length + 2);
                        callback();
                    }
                });
            },
            //USERNAME ARRIVE IN TIME
            deleteTimeout,
            //ASK FOR PASSWORD
            function (callback) {
                CommunicationFacade.ServerPassword(_this.socket, callback);
            },
            //WAIT FOR PASSWORD
            createTimeout,
            //GET PASSWORD
            function (callback) {
                _this.reader.setCallback(function () {
                    let posOfDelimiter = _this.reader.buffer.indexOf('\r\n');
                    if (posOfDelimiter < 0) //not accepted whole password yet
                    {
                        if (_this.reader.buffer.length > 7) //already arrive more than 7 symbols
                            callback(exception.SYNTAX);
                    }
                    else {
                        let password: string = _this.reader.buffer.substring(0, posOfDelimiter);
                        if (password.length > 7)
                            callback(exception.SYNTAX);
                        //password is now whole and correct accepted
                        _this.reader.buffer = _this.reader.buffer.substring(name.length + 2);
                        //validate
                        let sum: number = 0;
                        for (let i = 0; i < name.length; i++)
                            sum += name.charCodeAt(i);
                        if (parseInt(password) !== sum)
                            callback(exception.LOGIN);
                        callback();
                    }
                });
            },
            //PASSWORD ARRIVE CORRECT
            deleteTimeout,
            function (callback) {
                CommunicationFacade.ServerOk(_this.socket, callback);
            }
        ], callback);
    }
}