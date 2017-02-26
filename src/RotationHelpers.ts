import {Direction} from './Constants';
import {Position} from './Position';

export class RotationHelper {
    private static directions: number[] = [Direction.up, Direction.right, Direction.down, Direction.left]; //+1 turn right, -1 turn left;

    public static getDesiredDirection(current: Position, desired: Position) : number {
        let diffX = desired.x - current.x;
        let diffY = desired.y - current.y;

        if(diffX < 0){
            return Direction.left;
        }

        if(diffX > 0){
            return Direction.right;
        }

        if(diffY < 0){
            return Direction.up;
        }

        if(diffY > 0){
            return Direction.down;
        }
    }

    public static getRotation(currentRotation: number, requiredRotation: number) {

    };
}