const {logger} =    require('./logger')

module.exports = {
    requireValue(val, strict){
        if(val === null || val === undefined || val === NaN){
            const e = new Error("Value is required!")
            e.stack.split('\n').forEach(part => logger.log('error', part))
            if(strict) throw e
        }
        return val
    }
}