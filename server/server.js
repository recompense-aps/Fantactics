const express                       = require('express')
const bodyParser                    = require('body-parser')
const fs                            = require('fs')
const { networkInterfaces }         = require('os')
const { logger }                    = require('./logger')
const { Game }                      = require('./game')
const {Mock}                        = require('./mock')
const { FtRequest, FtRequestData }  = require('./requests')

const jsonParser = bodyParser.json()

class Server{
    constructor(){
        this.________ = 'FANTACTICS SERVER'
        this.requests           = []
        this.games              = []
        this.app                = express()
        this.initializeRoutes()
        this.mock               = new Mock(this)
    }

    start(port){
        this.port = port
        this.app.listen(port, () => {
            logger.log('info', `Fantactics server started at http://localhost:${this.port}`)
            const ipInfo = networkInterfaces().Ethernet[1]
            for(let key in ipInfo){
                logger.log('info', key + ':\t\t' + ipInfo[key])
            }
        })
    }

    handle(handler, ...args){
        try{
            handler(...args)
        }
        catch(error){
            error.stack.split('\n').forEach(line => logger.log('error', line))
        }
    }

    initializeRoutes(){
        this.bindGet('/dashboard', this.get_Dashboard)
        this.bindGet('/status', this.get_Status)
        this.bindPost('/game/create-game', this.post_CreateGame)
        this.bindPost('/game/join-game', this.post_JoinGame)
        this.bindPost('/game/sync', this.post_Sync)
    }

    bindGet(route, handler){
        this.app.get(route, jsonParser, (req,res) => {
            this.logRequest(req.body)
            this.handle(handler, this,req,res)
        })
    }

    bindPost(route, handler){
        this.app.post(route, jsonParser, (req,res) => {
            this.logRequest(req.body)
            this.handle(handler, this,req,res)
        })
    }

    logRequest(req){
        this.requests.push(req)
    }

    /**
     * 
     * @param {String} command 
     */
    handleCommand(command){
        if(command.includes('mock')){
            return this.mock.handleCommand(command.split(' ')[1] || logger.log('error', 'mock command requires an argument'))
        }

        switch(command){
            case 'flush':
                this.flush()
                break
            case 'save':
                this.save()
                break
            case 'dump':
                this.dump()
                break
            case 'stop':
                this.stop()
                break
            default:
                logger.log('error', `${command} is not a valid command`)
        }
    }

    flush(){
        this.games = []
        this.requests = []
        logger.log('info', 'Flushed server state')
    }

    save(){
        fs.writeFileSync(`logs/saves/${new Date().toISOString().replace(/\:/g, '-')}.log`, JSON.stringify(this, null, 4))
        logger.log('info', 'Saved server state')
    }

    dump(){
        const clone = Object.assign({}, this)
        delete clone.mock

        const json = JSON.stringify(clone, null, 4)
        json.split('\n').forEach(line => logger.log('info', line))
    }

    stop(){
        process.exit(0)
    }

    ///////////////////////////////////
    //  Route Handlers
    ///////////////////////////////////

    get_Dashboard(server, req, res){
        logger.log('dash', `${req.ip} is requesting the dashboard...`)
        res.sendFile(`${__dirname}/dashboard.html`)
        logger.log('dash', `served dashboard to ${req.ip}`)
    }

    get_Status(server, req, res){
        logger.log('status', `${req.ip} is requesting status...`)
        res.send( JSON.stringify(server, null, 4) )
        logger.log('status', `served status to ${req.ip}`)
    }

    post_Sync(server, req, res){
        const ft = new FtRequest(req.body)

        const data = server.sync(ft)

        res.send(JSON.stringify(data))
    }

    post_CreateGame(server, req, res){
        const ft = new FtRequest(req.body)

        const data = server.createGame(ft)
        
        res.send(JSON.stringify(data))
    }

    post_JoinGame(server, req, res){
        // TAKE NOTE:
        // This will just join the first avaliable game
        const ft = new FtRequest(req.body)

        const data = new FtRequestData({
            SenderGuid: ft.Data.SenderGuid
        })  

        logger.log('game', `${ft.Data.SenderGuid} is looking for a game to join...`)

        const game = server.games.find(game => game.players.length < 2)

        if(game){
            game.addPlayer(ft.Data.SenderGuid)
            logger.log('game', `${ft.Data.SenderGuid} joined game (${game.guid})`)
            data.Success = 'Found a game to join'
        }
        else{
            logger.log('game', 'could not find any avaliable games')
            data.Error = 'Could not find a game to join'
        }

        res.send(JSON.stringify(data))
    }

    /////////////////////////////////////
    //  Helpers and such
    /////////////////////////////////////
    findGameFromSenderGuid(guid){
        const game = this.games.find(game => game.players.find(player => player.guid === guid))

        if(game){
            logger.log('game', `found game that ${guid} is a part of`)
        }
        else{
            logger.log('game', `could not find a game that ${guid} is a part of`)
        }

        return game
    }

    /**
     * Syncs game data
     * @param {FtRequest} ft
     * @returns {FtRequestData}
     */
    sync(ft){
        logger.log('game', `${ft.Data.SenderName} is syncing...`)

        const game = this.findGameFromSenderGuid(ft.Data.SenderGuid)

        if(game){
            const response = game.handleRequest(ft)
            logger.log('pass', `${ft.Data.SenderName} sync succeeded`)
            return response
        }
        else{
            logger.log('error', `${ft.Data.SenderName} sync failed`)
            return { Error: 'Could not sync. Could not find a game' }
        }
    }

    /**
     * Creates a new game with that games GUID being the GUID
     * of the player that created it
     * @param {FtRequest} ft 
     * @returns {FtRequestData}
     */
    createGame(ft){
        logger.log('game', `${ft.Data.SenderName} is requesting to create a game...`)

        const game = new Game(ft.Data.SenderGuid, ft.Data.SenderName)
        this.games.push(game)
        game.addPlayer(ft.Data.SenderGuid, ft.Data.SenderName)

        logger.log('game', `${ft.Data.SenderName} created game successfully`)

        const data = new FtRequestData({
            SenderGuid: ft.Data.SenderGuid,
            Success:    'created game successfully'
        })

        return data
    }
}


module.exports = {
    Server: Server
}