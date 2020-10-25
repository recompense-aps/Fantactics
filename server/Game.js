const {FtRequest, FtRequestData}       = require('./requests')

class Game{
    constructor(guid, name){
        this.guid       = guid
        this.players    = []
        this.name       = name || guid
    }

    handleRequest(requestJson){
        const req = new FtRequest(requestJson)
        let response
        switch(req.type){
            case 'SyncUnits':
                response = this.handleSyncUnitsRequest(req)
                break
        }
        return JSON.stringify(response)
    }

    /**
     * 
     * @param {FtRequest} request
     * @returns {FtRequestData}
     */
    handleSyncUnitsRequest(request){
        let player = this.players.find(player => player.guid === request.data.senderGuid)

        if(!player){
            // player doesn't exist yet, create it
            player = new Player(request.data.senderGuid)
            this.players.push(player)
        }

        // sync all the unit data
        this.players.forEach(p => {
            p.syncUnitActions(request.data)
        })

        // send back the actions not synced
        let response = new FtRequestData({
            SenderGuid:     player.guid,
            UnitActions:    player.unitActions.unSynced
        })

        // flush the unsynced actions
        player.unitActions.unSynced = []

        return response
    }

    addPlayer(guid, name){
        this.players.push(new Player(guid, name));
    }
}

class Player{
    /**
     * 
     * @param {String} guid 
     * @param {String} name
     */
    constructor(guid, name){
        this.guid = guid
        this.name = name || guid
        this.unitActions = {
            // Unit actions that belong to this player
            own:        [],
            // Unit actions from other players that need to
            // be sent down to the game
            unSynced:   []
        }
    }

    /**
     * Takes unit actions and if the unit actions
     * belong to this player, the ations are added to the 'own'
     * collection, otherwise they are stored in the 'unSynced'
     * collection
     * @param {FtRequestData} data 
     */
    syncUnitActions(data){
        if(data.senderGuid === this.guid){
            this.unitActions.own.push(...data.unitActions)
        }
        else{
            this.unitActions.unSynced.push(...data.unitActions)
        }
    }
}

module.exports = {
    Game: Game
}