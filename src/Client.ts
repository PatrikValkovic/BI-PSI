import * as net from 'net';
import * as async from 'async';
import {Errors, Direction} from './Constants';
import {CommunicationFacade} from './CommunicationFacade';
import {Reader} from "./Reader";
import {Position} from './Position';
import {PausingTimer} from './PausingTimer';
import {RotationHelper} from './RotationHelpers';
import {Charging} from './Charging';

export class Client {
    private socket: net.Socket;
    public timeout: PausingTimer;
    private reader: Reader;
    private position: Position;
    private charger: Charging;

    public constructor(socket: net.Socket) {
        let _this = this;
        this.socket = socket;
        this.timeout = null;
        this.charger = new Charging(() => {return _this.timeout;});
        this.reader = new Reader(() => {return _this.timeout === null ? null : _this.timeout.repeat(); });
        this.reader.attachArriveMessage(this.charger.createArriveTimeout());
        this.reader.registerMiddleware(this.charger);
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

    //helpers

    private static parsePosition(text: string): Position {
        if (text.substr(0, 3) !== 'OK ')
            return null;
        let rest = text.substring(3);
        let pos: string[] = rest.split(' ');
        if (pos.length != 2)
            return null;
        let instance: Position = new Position(parseInt(pos[0]), parseInt(pos[1]));
        if (isNaN(instance.x) || isNaN(instance.y) ||
            instance.x.toString() !== pos[0] || instance.y.toString() !== pos[1])
            return null;
        return instance;
    }

    private move(callback: Function) {
        console.log('Moving robot forward');
        this.robotAction(callback, function (socket: net.Socket, callback: Function) {
            CommunicationFacade.ServerMove(socket, callback);
        });
    };

    private turnLeft(callback: Function) {
        console.log('Turning robot left');
        this.position.direction = RotationHelper.nextDirection(this.position.direction, 'left');
        this.robotAction(callback, function (socket: net.Socket, callback: Function) {
            CommunicationFacade.ServerTurnLeft(socket, callback);
        });
    };

    private turnRight(callback: Function) {
        console.log('Turning robot right');
        this.position.direction = RotationHelper.nextDirection(this.position.direction, 'right');
        this.robotAction(callback, function (socket: net.Socket, callback: Function) {
            CommunicationFacade.ServerTurnRight(socket, callback);
        });
    };

    private robotAction(callback: Function, command: Function) {
        let _this = this;

        let createTimeout: Function = this.factoryCreateTimeout(callback, 1000);
        let deleteTimeout: Function = this.factoryDeleteTimeout();

        async.series([
            function (callback) {
                command(_this.socket, callback);
            },
            createTimeout,
            function (callback) {
                _this.reader.maxLength = 10;
                _this.reader.setCallback(function (text) {
                    if (text === Errors.overLength || !text.startsWith('OK '))
                        return callback(Errors.syntax);

                    let position = Client.parsePosition(text);
                    if (position === null)
                        return callback(Errors.syntax);

                    position.direction = _this.position.direction;
                    _this.position = position;

                    if (_this.position.x === 0 && _this.position.y === 0)
                        return callback(Errors.onPosition);

                    console.log("Pozice robota: [" + _this.position.x + ',' + _this.position.y + '] ' + Direction.toString(position.direction));
                    callback();
                });
            },
            deleteTimeout,
        ], function (err, data) {
            deleteTimeout();
            callback(err, data);
        });
    }

    //actions

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
                    if (text !== parseInt(password).toString())
                        return callback(Errors.syntax);
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

    public getPosition(callback) {
        console.log("Getting position");
        let _this = this;

        console.log("Creating unknown position");
        this.position = new Position();

        async.series([
            function (callback) { //turn to get position
                _this.turnLeft(callback);
            },
            function (callback) { //get direction
                console.log("Getting rotation for robot");
                _this.position.direction = null;
                let oldPosition = Object.create(_this.position);
                async.until(() => {return _this.position.direction !== null}, function (callback) {
                    _this.move(function (err, data) {
                        console.log('{this.x:' + _this.position.x + ',this.y:' + _this.position.y +
                            '}{old.x:' + oldPosition.x + ',old.y:' + oldPosition.y + '}');
                        if (_this.position.x + 1 === oldPosition.x && _this.position.y === oldPosition.y)
                            _this.position.direction = Direction.left;
                        if (_this.position.x - 1 === oldPosition.x && _this.position.y === oldPosition.y)
                            _this.position.direction = Direction.right;
                        if (_this.position.x === oldPosition.x && _this.position.y + 1 === oldPosition.y)
                            _this.position.direction = Direction.down;
                        if (_this.position.x === oldPosition.x && _this.position.y - 1 === oldPosition.y)
                            _this.position.direction = Direction.up;
                        callback(err, data);
                    });
                }, callback);
            }, function (callback) {
                console.log("Pozice robota je: [" + _this.position.x + ',' + _this.position.y + '] ' + Direction.toString(_this.position.direction));
                console.log("Pozice a rotace robota zjisteny");
                callback();
            }
        ], callback);
    }

    public navigate(callback) {

        console.log("Starting of navigate");

        let _this = this;

        let createTest = function () {
            return function () {
                return _this.position.x === 0 && _this.position.y === 0;
            };
        };

        let createFn = function () {
            return function (callback) {
                async.series([
                    function (callback) {
                        let desiredRotation = RotationHelper.getDesiredDirection(_this.position, new Position(0, 0));
                        let nextRotation = RotationHelper.nextRotation(_this.position.direction, desiredRotation);
                        if (nextRotation === 'left')
                            return _this.turnLeft(callback);
                        if (nextRotation === 'right')
                            return _this.turnRight(callback);
                        return _this.move(callback);
                    }
                ], callback)
            }
        };

        async.until(createTest(), createFn(), callback);
    }

    public getMessage(callback) {

        console.log("Starting of picking message");

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
                        return callback(Errors.syntax);

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