const {Mock} = require("./mock")



const basic1 = {
    /**
     * 
     * @param {Mock} mock 
     */
    run(mock){

        // create a game
        mock.createGame()

        const actions = mock.unitActions()
                            .spawnUnit({x:96, y:96}, 'Zombie')
                            .spawnUnit({x:416, y:160}, 'Zombie')
                            .spawnUnit({x:736, y:96}, 'Zombie')
                            .spawnUnit({x:800, y:288}, 'Zombie')
                            .val()

        // wait for the testing player to join the game
        // then, sync units
        mock.bus.on('join-game', (game, joiningPlayer) => {
            mock.mockSyncUnitActionsRequest(actions)
        })
    }
}

const basic2 = {
    run(mock){
       
        // create a game 
        mock.createGame()
        
        const actions = mock.unitActions()
                            .spawnUnit({x})
    }
}

module.exports = { basic1 }