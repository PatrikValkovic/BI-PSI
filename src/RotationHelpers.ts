import {Direction} from './Constants';
import {Position} from './Position';

export class RotationHelper {

    private static log: boolean = true;

    private static directions: number[] = [Direction.up, Direction.right, Direction.down, Direction.left]; //+1 turn right, -1 turn left;

    public static getDesiredDirection(current: Position, desired: Position): number {
        let diffX = desired.x - current.x;
        let diffY = desired.y - current.y;

        if (diffX < 0) {
            return Direction.left;
        }

        if (diffX > 0) {
            return Direction.right;
        }

        if (diffY < 0) {
            return Direction.up;
        }

        if (diffY > 0) {
            return Direction.down;
        }
    }

    public static nextRotation(currentRotation: number, requiredRotation: number): string {
        if (this.log)
            console.log("Rotation from " + Direction.toString(currentRotation) + " to " + Direction.toString(requiredRotation));

        if (currentRotation === requiredRotation)
            return 'none';

        let length = this.directions.length;
        let currentIndex: number = null;
        let requiredIndexes: number[] = [];
        //get indexes
        for (let i = 0; i < length && currentIndex === null; i++)
            if (this.directions[i] & currentRotation)
                currentIndex = i;
        for (let i = 0; i < length && requiredIndexes.length === 0; i++)
            if (this.directions[i] & requiredRotation)
                requiredIndexes.push(i);
        if (this.log)
            console.log("Index of current: " + currentIndex + ", required " + requiredIndexes[0]);

        //extend required indexes
        (() => {
            let currentRequired = requiredIndexes[0];
            requiredIndexes.push(currentRequired - length);
            requiredIndexes.push(currentRequired + length);
        })();
        if (this.log)
            console.log("All options: " + requiredIndexes.toString());
        //find nearest direction
        requiredIndexes.push(currentIndex);
        requiredIndexes.sort();
        let indexInAll = null;
        for (let i = 0, l = requiredIndexes.length; i < l && indexInAll === null; i++)
            if (requiredIndexes[i] === currentIndex)
                indexInAll = i;
        let diffLeft = Math.abs(requiredIndexes[indexInAll] - requiredIndexes[indexInAll - 1]);
        let diffRight = Math.abs(requiredIndexes[indexInAll] - requiredIndexes[indexInAll + 1]);
        if (this.log)
            console.log("diffLeft: " + diffLeft + " diffRight: " + diffRight);
        if (diffLeft < diffRight)
            return 'left';
        return 'right';
    };
}