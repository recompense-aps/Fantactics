class Bus{
    constructor(){
        this.handlers   = {}
    }

    /**
     * 
     * @param {String} name 
     * @param {Function} handler 
     */
    on(name, handler){
        if(!this.handlers[name]){
            this.handlers[name] = []
        }

        this.handlers[name].push(handler)
    }

    /**
     * 
     * @param {String} name 
     * @param  {...any} params 
     */
    emit(name, ...params){
        if(this.handlers[name]){
            this.handlers[name].forEach(handle => handle(...params))
        }
            
    }
}

module.exports = { Bus }