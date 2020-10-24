const {requireValue} = require('./utils')

class FtRequest{
    /**
     * 
     * @param {String} json 
     */
    constructor(json){
        const raw = JSON.parse(json)

        // implicitly transfer over the data
        this.type = requireValue(raw.Type)
        this.data = new FtRequestData( requireValue(raw.Data) )
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