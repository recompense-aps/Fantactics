const {FtRequest, FtRequestData}       = require('./requests')
const {logger}                         = require('./logger')

class Game{
    constructor(guid, name){
        this.guid       = guid
        this.players    = []
        this.name       = name || guid
    }

    /**
     * 
     * @param {FtRequest} request
     * @returns {FtRequestData}
     */
    handleRequest(request){
        let response

        switch(request.Type){
            case 'Sync':
                response = this.sync(request.Data)
                break
            default:
                const error = `Cannot handle game request. ${request.Type} is not a valid request type`
                response = new FtRequestData({ SenderGuid:request.Data.SenderGuid, Error: error })
                logger.log('error', error)
        }

        return response
    }

    /**
     * 
     * @param {FtRequestData} data
     * @returns {FtRequestData}
     */
    sync(data){
        const sendingPlayer = this.players.find(player => player.guid === data.SenderGuid)
        const otherPlayers = this.players.filter(player => player.guid !== sendingPlayer.guid)

        // add data to other players that they will need to sync
        otherPlayers.forEach(player => {
            player.toSync.UnitActions.push(...data.UnitActions)
            player.toSync.Notifications.push(...data.Notifications)
        })

        // grab data this player needs to sync
        const returnData = new FtRequestData({
            SenderGuid: data.SenderGuid,
            SenderName: data.SenderName,
            ...sendingPlayer.toSync
        })

        // reset
        sendingPlayer.toSync.reset()

        return returnData
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
        this.guid       = guid
        this.name       = name || guid
        this.toSync     = new SyncablePlayerData()
    }
}

class SyncablePlayerData
{
    constructor(){
        this.UnitActions    = []
        this.Notifications  = []
    }

    reset(){
        this.UnitActions    = []
        this.Notifications  = []
    }
}

module.exports = {
    Game: Game
}