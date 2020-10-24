const {requireValue} = require('./utils')

class FtRequest{
    /**
     * 
     * @param {Object} jsonObject 
     */
    constructor(jsonObject){
        // implicitly transfer over the data
        this.type = requireValue(jsonObject.Type)
        this.data = new FtRequestData( requireValue(jsonObject.Data) )
    }
}

class FtRequestData{
    constructor(rawObject){
        // implicitly transfer over the data
        this.senderGuid     = requireValue(rawObject.SenderGuid)
        this.unitActions    = requireValue(rawObject.UnitActions)
    }
}

module.exports = {
    FtRequest,
    FtRequestData
}