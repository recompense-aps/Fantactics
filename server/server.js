const express                       = require('express')
const bodyParser                    = require('body-parser')
const {logger}                      = require('./logger')
const {Game}                        = require('./Game')
const {FtRequest, FtRequestData}    = require('./requests')

const jsonParser = bodyParser.json()

class Server{
    constructor(){
        this.________ = 'FANTACTICS SERVER'
        this.games = []
        this.app = express()
        this.initializeRoutes()
    }

    start(port){
        this.port = port
        this.app.listen(port, () => {
            logger.log('info', `Fantactics server started at http://localhost:${this.port}`)
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
        this.bindPost('/game/sync-units', this.post_SyncUnits)
    }

    bindGet(route, handler){
        this.app.get(route, jsonParser, (req,res) => {
            this.handle(handler, this,req,res)
        })
    }

    bindPost(route, handler){
        this.app.post(route, jsonParser, (req,res) => {
            this.handle(handler, this,req,res)
        })
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

    post_SyncUnits(server, req, res){
        const ft = new FtRequest(req.body)
        logger.log('game', `${ft.Data.SenderGuid} is syncing units...`)

        const game = server.findGameFromSenderGuid(ft.Data.SenderGuid)

        if(game){
            // NOTE: req.body is an object, not a json string
            const response = game.handleRequest(req.body)
            
            res.send(response)
        }
        else{
            res.send( JSON.stringify({ Error: 'Could not sync units. Could not find a game' }) )
        }
    }

    post_CreateGame(server, req, res){
        // the games id will be the guid of the player
        // that creates it
        const ft = new FtRequest(req.body)

        logger.log('game', `${ft.Data.SenderName} is requesting to create a game...`)

        const game = new Game(ft.Data.SenderGuid, ft.Data.SenderName)
        server.games.push(game)
        game.addPlayer(ft.Data.SenderGuid, ft.Data.SenderName)

        logger.log('game', `${ft.Data.SenderName} created game successfully`)

        const data = new FtRequestData({
            SenderGuid: ft.Data.SenderGuid,
            Success:    'created game successfully'
        })

        res.send(JSON.stringify(data))
    }

    post_JoinGame(server, req, res){
        // TAKE NOTE:
        // This will just join the first avaliable game
        const ft = new FtRequest(req.body)

        logger.log('game', `${ft.Data.SenderGuid} is looking for a game to join...`)

        const game = server.games.find(game => game.players.lenth < 2)

        if(game){
            game.addPlayer(ft.Data.SenderGuid)
            logger.log('game', `${ft.Data.SenderGuid} joined game (${game.guid})`)
        }
        else{
            logger.log('game', 'could not find any avaliable games')
        }
    }

    /////////////////////////////////////
    //  Helpers
    /////////////////////////////////////
    findGameFromSenderGuid(guid){
        const game = this.games.find(game => game.players.find(player => player.guid === ft.Data.SenderGuid))

        if(game){
            logger.log('game', `found game that ${guid} is a part of`)
        }
        else{
            logger.log('game', `could not find a game that ${guid} is a part of`)
        }

        return game
    }
}


module.exports = {
    Server: Server
}