const express =     require('express')
const bodyParser =  require('body-parser')
const {logger} =    require('./logger')
const {Game} =      require('./Game')

const app = express()
const port = 3000
const jsonParser = bodyParser.json()
const game = new Game()

function handle(handler, ...args){
    try{
        handler(...args)
    }
    catch(error){
        error.stack.split('\n').forEach(line => logger.log('error', line))
    }
}

logger.disable('status')

app.get('/dashboard', (req, res) => {
    handle((req,res) => {
        logger.log('dash', `${req.ip} is requesting the dashboard...`)
        res.sendFile(`${__dirname}/dashboard.html`)
        logger.log('dash', `served dashboard to ${req.ip}`)
    }, req, res)
})

app.get('/status', (req, res) => {
    logger.log('status', `${req.ip} is requesting status...`)
    res.send( JSON.stringify(game, null, 4) )
    logger.log('status', `served status to ${req.ip}`)
})

app.post('/game/sync-units', jsonParser, (req, res) => {
    logger.log('sync-units', 'Syncing units...')
    const response = game.handleRequest(req.body)
    res.send(response)
})
  
app.listen(port, () => {
    logger.log('info', `Example app listening at http://localhost:${port}`)
})