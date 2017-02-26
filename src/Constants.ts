export const OverLength = 1;

export const Direction = {
    up: 1,
    right: 2,
    down: 4,
    left: 8,
    toString(pos: number): string {
        if(pos === null)
            return 'unknown';
        let text : string = '';
        for(let property in this)
            if(pos & this[property])
                text += '_' + property;
        return text.substring(1);
    }
};

export const Errors = {
    overLength: 1,
    timeout: 2,
    login: 4,
    syntax: 8,
    logic: 16,
    my: 32,
    onPosition: 64,
};