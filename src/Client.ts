import * as net from 'net';
import * as async from 'async';
import * as exception from './Exceptions';
import {Errors} from './Constants';
import {CommunicationFacade} from './CommunicationFacade';
import {Reader} from "./Reader";

export class Client {
    private socket: net.Socket;
    private timeout;
    private reader: Reader;

    public constructor(socket: net.Socket) {
        let _this = this;
        this.socket = socket;
        this.reader = new Reader();
        this.timeout = null;
        this.socket.addListener('data', function (data: string) {
            _this.reader.appendText(data);
        });
    }

    public authenticate(callback) {

        let _this = this;
        let timeoutLength = 1000;
        let name = '';

        let createTimeout = function (inCallback) {
            if (_this.timeout !== null)
                return inCallback();
            _this.timeout = setTimeout(function () {
                callback(Errors.timeout);
            }, timeoutLength);
            inCallback();
        };
        let deleteTimeout = function (inCallback) {
            if (_this.timeout === null)
                return inCallback();
            clearTimeout(_this.timeout);
            _this.timeout = null;
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
                _this.reader.maxLength = 100;
                _this.reader.setCallback(function (text) {
                    if (text === Errors.overLength)
                        return callback(Errors.syntax);
                    name = text;
                    return callback();
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
                _this.reader.maxLength = 7;
                _this.reader.setCallback(function (text) {
                    if (text === Errors.overLength)
                        return callback(Errors.syntax);

                    let password: string = text;
                    //validate
                    let sum: number = 0;
                    for (let i = 0; i < name.length; i++)
                        sum += name.charCodeAt(i);
                    console.log("Account validation based on " + parseInt(password) + " and " + sum);
                    if (parseInt(password) !== sum)
                        return callback(Errors.login);
                    return callback();
                });
            },
            //PASSWORD ARRIVE CORRECT
            deleteTimeout,
            function (callback) {
                CommunicationFacade.ServerOk(_this.socket, callback);
            }
        ], function (err, data) {
            deleteTimeout(function () {
            });
            callback(err, data);
        });
    }

    public navigate(callback) {

    }
}