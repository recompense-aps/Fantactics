const colors = require('colors/safe')

const logger = {

    tags: {
        'error':        colors.red,
        'info':         colors.magenta,
        'dash':         colors.cyan,
        'status':       colors.gray,
        'game':         colors.blue,
        'pass':         colors.green
    },

    disabled: [],

    log(tag, info){
        color = this.tags[tag] || this.tags.info
        
        let output = colors.bold( color('[' + tag + ']') ) + '\t\t' + info

        if(!this.disabled.includes(tag)) console.log(output)
    },

    disable(tag){
        this.disabled.push(tag)
    }

}

module.exports = { logger:logger }