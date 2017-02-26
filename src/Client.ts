import * as net from 'net';
import * as async from 'async';
import * as exception from './Exceptions';
import {Errors, Direction} from './Constants';
import {CommunicationFacade} from './CommunicationFacade';
import {Reader} from "./Reader";
import {Position} from './Position';
import {PausingTimer} from './PausingTimer';

export class Client {
    private socket: net.Socket;
    private timeout : PausingTimer;
    private reader: Reader;
    private position: Position;

    public constructor(socket: net.Socket) {
        let _this = this;
        this.socket = socket;
        this.reader = new Reader();
        this.timeout = null;
        this.socket.addListener('data', function (data: string) {
            _this.reader.appendText(data);
        });
    }

    private factoryCreateTimeout(callback, timeoutLength) {
        let _this = this;
        return function (inCallback: Function = () => { }) {
            if (_this.timeout !== null)
                return inCallback();
            _this.timeout = new PausingTimer(() => { callback(Errors.timeout); }, timeoutLength);
            inCallback();
        };
    }

    private factoryDeleteTimeout() {
        let _this = this;
        return function (inCallback: Function = () => { }) {
            if (_this.timeout === null)
                return inCallback();
            _this.timeout.pause();
            _this.timeout = null;
            inCallback();
        };
    }

    public authenticate(callback) {

        let _this = this;
        let timeoutLength = 1000;
        let name = '';

        let createTimeout = this.factoryCreateTimeout(callback, timeoutLength);
        let deleteTimeout = this.factoryDeleteTimeout();

        async.series([
            //SEND USER PACKET
            function (callback) {
                CommunicationFacade.ServerUser(_this.socket, callback);
            },
            //WAIT FOR RESPOND
            createTimeout,
            //GET USERNAME
            function (callback) {
                _this.reader.maxLength = 98;
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
                _this.reader.maxLength = 5;
                _this.reader.setCallback(function (text) {
                    if (text === Errors.overLength)
                        return callback(Errors.syntax);

                    let password: string = text;
                    console.log("Obtained password: " + password);
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

    private static parsePosition(text: string): Position {
        if (text.substr(0, 3) !== 'OK ')
            return null;
        let rest = text.substring(3);
        let pos: string[] = rest.split(' ');
        if (pos.length != 2)
            return null;
        let instance: Position = new Position(parseInt(pos[0]), parseInt(pos[1]));
        if (isNaN(instance.x) || isNaN(instance.y))
            return null;
        return instance;
    }

    public getPosition(callback) {
        let _this = this;

        let createTimeout = this.factoryCreateTimeout(callback, 1000);
        let deleteTimeout = this.factoryDeleteTimeout();

        this.position = new Position();

        async.series([
            function (callback) {
                CommunicationFacade.ServerTurnLeft(_this.socket, callback);
            },
            createTimeout,
            function (callback) { //first get position
                _this.reader.maxLength = 12;
                _this.reader.setCallback(function (text) {
                    if (text === Errors.overLength || !text.startsWith('OK '))
                        return callback(Errors.syntax);

                    let position = Client.parsePosition(text);
                    if (position === null)
                        return callback(Errors.syntax);
                    _this.position = position;

                    if (_this.position.x === 0 && _this.position.y === 0)
                        return callback(Errors.onPosition);

                    console.log("Pozice robota: [" + _this.position.x + ',' + _this.position.y + ']');
                    callback();
                });
            },
            deleteTimeout,
            function (callback) {
                CommunicationFacade.ServerMove(_this.socket, callback);
            },
            createTimeout,
        ], function (err, data) {
            deleteTimeout(function () {
            });
            callback(err, data);
        });
    }

    public rotate(callback) {

        let _this = this;

        let createTimeout: Function = this.factoryCreateTimeout(callback, 1000);
        let deleteTimeout: Function = this.factoryDeleteTimeout();

        let createTest = function () {
            return function () {
                if (_this.position.x < 0) //must be right
                    return _this.position.direction === Direction.left;
                if (_this.position.x > 0) //must be left
                    return _this.position.direction === Direction.right;
                if (_this.position.y < 0) //must be down
                    return _this.position.direction === Direction.down;
                if (_this.position.y > 0) //must be up
                    return _this.position.direction === Direction.up;
            };
        };

        let createRotate = function () {
            return function (callback) {
                if (_this.position.x < 0) //must be right
                {
                    if (_this.position.direction == Direction.up)
                        return CommunicationFacade.ServerTurnRight(_this.socket, callback);
                    return CommunicationFacade.ServerTurnLeft(_this.socket, callback);

                }
                if (_this.position.x > 0) //must be left
                {
                    if (_this.position.direction == Direction.up)
                        return CommunicationFacade.ServerTurnLeft(_this.socket, callback);
                    return CommunicationFacade.ServerTurnRight(_this.socket, callback);
                }
                if (_this.position.y < 0) //must be down
                {
                    if (_this.position.direction == Direction.left)
                        return CommunicationFacade.ServerTurnLeft(_this.socket, callback);
                    return CommunicationFacade.ServerTurnRight(_this.socket, callback);
                }
                if (_this.position.y > 0) //must be up
                {
                    if (_this.position.direction == Direction.left)
                        return CommunicationFacade.ServerTurnRight(_this.socket, callback);
                    return CommunicationFacade.ServerTurnLeft(_this.socket, callback);
                }
            };
        };

        let generateCallback = function () {
            return function (callback) {
                let rotate = createRotate();
                async.series([
                    rotate,
                    createTimeout,
                    function (callback) {
                        _this.reader.maxLength = 12;
                        _this.reader.setCallback(function (text) {
                            if (text === Errors.overLength)
                                callback(Errors.syntax);

                            let position = Client.parsePosition(text);
                            if (position === null)
                                return callback(Errors.syntax);

                            _this.position = position;
                            console.log("Pozice robota: [" + _this.position.x + ',' + _this.position.y + ']' + '-' + Direction.toString(_this.position.direction));
                            callback();
                        });
                    },
                    deleteTimeout,
                ], function (err, data) {
                    deleteTimeout(function () {
                    });
                    callback(err, data);
                });
            };
        };

        async.until(createTest(), generateCallback(), callback);
    }

    public navigate(callback) {
        let _this = this;

        let _rotate = false;

        let createTimeout: Function = this.factoryCreateTimeout(callback, 1000);
        let deleteTimeout: Function = this.factoryDeleteTimeout();

        let createTest = function () {
            return function () {
                return _this.position.x === 0 && _this.position.y === 0;
            };
        };

        let createFn = function () {
            return function (callback) {

                async.series([
                    function (callback) {
                        CommunicationFacade.ServerMove(_this.socket, callback);
                    },
                    createTimeout,
                    function (callback) {
                        _this.reader.maxLength = 12;
                        _this.reader.setCallback(function (text) {
                            if (text === Errors.overLength)
                                callback(Errors.syntax);

                            let position = Client.parsePosition(text);
                            if (position === null)
                                return callback(Errors.syntax);

                            if ((position.x === 0 && _this.position.x !== 0) ||
                                (position.y === 0 && _this.position.y !== 0))
                                _rotate = true;

                            _this.position = position;
                            callback();
                        });
                    },
                    deleteTimeout,
                    function (callback) {
                        if (_rotate)
                            _this.rotate(callback);
                    }
                ], function () {
                    deleteTimeout(function () {
                    });
                    callback();
                });
            }
        };

        async.until(createTest(), createFn(), callback);
    }

    public getMessage(callback) {
        let _this = this;

        let createTimeout = this.factoryCreateTimeout(callback, 1000);
        let deleteTimeout = this.factoryDeleteTimeout();

        async.series([
            function (callback) {
                CommunicationFacade.ServerPickUp(_this.socket, callback);
            },
            createTimeout,
            function (callback) {
                _this.reader.maxLength = 98;
                _this.reader.setCallback(function (text) {
                    if (text === Errors.overLength)
                        callback(Errors.syntax);

                    console.log("Robot pickuped: " + text);
                    callback();
                });
            },
            deleteTimeout,
            function (callback) {
                CommunicationFacade.ServerOk(_this.socket, callback);
            }
        ], function (err, data) {
            deleteTimeout();
            callback(err, data);
        });
    }
}