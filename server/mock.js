const {logger} = require('./logger')
const { FtRequest, FtRequestData } = require('./requests')
const {Server}        = require('./server')

class Mock{
    /**
     * 
     * @param {Server} server 
     */
    constructor(server){
        this.server             = server
        this.fakeSenderGuid     = '{1111-1111-1111}'
        this.fakeSenderName     = 'SERVERMOCK'
        this.unitCounter        = 0
    }

    /**
     * 
     * @param {String} command 
     */
    handleCommand(command){
        switch(command){
            case 'create-game':
                this.createGame()
                break
            case 'sync-units':
                this.syncUnits()
                break
        }
    }

    mocking(){
        logger.log('info', '')
    }

    nextUnitId(){
        return 'MOCK-UNIT-ID-' + this.unitCounter++
    }

    createGame(){
        // first, create a game
        this.server.createGame(new FtRequest({
            Type:   'CreateGame',
            Data:   new FtRequestData({
                SenderGuid:     this.fakeSenderGuid,
                SenderName:     this.fakeSenderName
            })
        }))
    }

    syncUnits(){
        // next, add some fake sync data
        this.server.sync(new FtRequest({
            Type:   'Sync',
            Data:   new FtRequestData({
                SenderGuid:     this.fakeSenderGuid,
                SenderName:     this.fakeSenderName,
                UnitActions:    [
                    {
                        UnitGuid:      this.nextUnitId(),
                        JsonData:       {
                            UnitActionType:         'SpawnAction',
                            SpawnWorldPosition:     {x: 50, y: 50},
                            UnitType:               'Zombie'
                        }
                    }
                ]
            })
        }))
    }

}

module.exports = { Mock }