export const OverLength = 1;

export const Direction = {
    up: 1,
    right: 2,
    down: 4,
    left: 8,
    toString(pos: number): string{
        if (pos & this.up)
            return 'up';
        if (pos & this.right)
            return 'right';
        if (pos & this.down)
            return 'down';
        if (pos & this.left)
            return 'left';
    }
};

export const Errors = {
    overLength: 1,
    timeout: 2,
    login: 4,
    syntax: 8,
    logic: 16,
    my: 32,
};