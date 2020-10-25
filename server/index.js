const {Server} =     require('./server')
const {logger} =     require('./logger')

console.clear()
logger.disable('status')

const server = new Server()
server.start(3000)