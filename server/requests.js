const {requireValue} = require('./utils')

class FtRequest{
    /**
     * 
     * @param {Object} jsonObject 
     */
    constructor(jsonObject){
        // implicitly transfer over the data
        this.Type = requireValue(jsonObject.Type)
        this.Data = new FtRequestData( requireValue(jsonObject.Data) )
    }
}

class FtRequestData{
    constructor(rawObject){
        // implicitly transfer over the data
        this.SenderGuid     = requireValue(rawObject.SenderGuid)
        this.UnitActions    = rawObject.UnitActions || []
        this.Error          = rawObject.Error
        this.Success        = rawObject.Success
        this.SenderName     = rawObject.SenderName
        this.Message        = rawObject.Message
    }
}

module.exports = {
    FtRequest,
    FtRequestData
}