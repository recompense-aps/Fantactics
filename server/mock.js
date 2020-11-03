const { Bus }                       = require('./bus')
const {logger}                      = require('./logger')
const { FtRequest, FtRequestData }  = require('./requests')
const {Server}                      = require('./server')
const scenarios                     = require('./scenarios')

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
        this.scenarios          = scenarios
        this.bus                = new Bus()

        this.server.bus.on('create-game', (game) => this.bus.emit('create-game', game))
        this.server.bus.on('join-game', (game, joiningPlayer) => this.bus.emit('join-game', game, joiningPlayer))
    }

    /**
     * 
     * @param {String} command 
     */
    handleCommand(command){
        if(command.includes(':')){
            const scenarioName = command.split(':')[1]
            logger.log('info', `running scenario ${scenarioName}`)
            return this.scenarios[scenarioName].run(this)
        }
        switch(command){
            case 'create-game':
                this.createGame()
                break
            case 'sync-units':
                this.syncUnits()
                break
        }
    }

    unitGuid(count){
        return 'MOCK-UNIT-ID-' + count
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

    /**
     * 
     * @param {Array} unitActions 
     */
    mockSyncUnitActionsRequest(unitActions){
        this.server.sync(new FtRequest({
            Type:   'Sync',
            Data:   new FtRequestData({
                SenderGuid:     this.fakeSenderGuid,
                SenderName:     this.fakeSenderName,
                UnitActions:    unitActions
            })
        }))
    }

    unitActions(){ return new UnitActionGenerator(this) }

    syncUnits(){
        // next, add some fake sync data
        this.server.sync(new FtRequest({
            Type:   'Sync',
            Data:   new FtRequestData({
                SenderGuid:     this.fakeSenderGuid,
                SenderName:     this.fakeSenderName,
                UnitActions:    [
                    {
                        UnitGuid:      this.unitGuid(0),
                        JsonData:       {
                            UnitActionType:         'SpawnAction',
                            SpawnWorldPosition:     {x: 150, y: 150},
                            UnitType:               'Zombie'
                        }
                    },
                    {
                        UnitGuid:       this.unitGuid(0),
                        JsonData:       {
                            UnitActionType:         'MoveAction',
                            WorldDestination:       { x:288, y:288 }
                        }
                    }
                ]
            })
        }))
    }

}

class UnitActionGenerator{
    /**
     * 
     * @param {Mock} mock 
     */
    constructor(mock){
        this.mock = mock
        this.unitActions = []
    }

    /**
     * 
     * @param {Object} worldPosition 
     * @param {String} unitType 
     * @returns UnitActionGenerator
     */
    spawnUnit(worldPosition, unitType){
        this.unitActions.push({
            UnitGuid:      this.mock.unitGuid(this.mock.unitCounter++),
            JsonData:       {
                UnitActionType:         'SpawnAction',
                SpawnWorldPosition:     worldPosition,
                UnitType:               unitType
            }
        })

        return this
    }

    /**
     * 
     * @param {Number} whichUnit 
     * @param {Object} worldPositionDestination 
     */
    moveUnit(whichUnit, worldPositionDestination){
        this.unitActions.push({
            UnitGuid:       this.mock.unitGuid(whichUnit),
            JsonData:       {
                UnitActionType:         'MoveAction',
                WorldDestination:       worldPositionDestination
            }
        })

        return this
    }

    val(){ return this.unitActions }
}

module.exports = { Mock }