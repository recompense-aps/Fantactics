const {Server} =     require('./server')
const {logger} =     require('./logger')
const readline =     require('readline')

console.clear()
logger.disable('status')

const server = new Server()
server.start(3000)

const rl = readline.createInterface({
    input: process.stdin,
    output: process.stdout
})

setTimeout(() => {

    prompt()

}, 1000)

function prompt(){
    rl.question('', (input) => {
        
        server.handleCommand(input)

        prompt()
    })
}