const {logger} =    require('./logger')

module.exports = {
    requireValue(val, strict){
        if(val === null || val === undefined || val === NaN){
            const e = new Error("Value is required!")
            logger.log('error', e)
            if(strict) throw e
        }
        return val
    }
}