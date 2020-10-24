const colors = require('colors/safe')

const logger = {

    tags: {
        'error':        colors.red,
        'info':         colors.magenta,
        'dash':         colors.cyan,
        'status':       colors.gray,
        'sync-units':   colors.blue
    },

    disabled: [],

    log(tag, info){
        color = this.tags[tag] || this.tags.info
        
        let output = colors.bold( color('[' + tag + ']') ) + '\t\t' + info

        if(!this.disabled[tag]) console.log(output)
    },

    disable(tag){
        this.disabled.push(tag)
        console.log(this.disabled)
    }

}

module.exports = { logger:logger }