import {Direction} from './Constants';

export class Position {
    public constructor(public x: number = 0, public y: number = 0, public direction: number = Direction.up) {

    }
}