import { Room, Client } from "colyseus";
import { Schema, type, MapSchema, ArraySchema } from "@colyseus/schema";

export class Vector2Float extends Schema {
    @type ("uint32") id = 0;
    @type("number") x = Math.floor(Math.random() * 256) - 123;
    @type("number") z = Math.floor(Math.random() * 256) - 123;
}

export class Player extends Schema {
    @type("string") login = "";
    @type("number") x = Math.floor(Math.random() * 256) - 123;
    @type("number") z = Math.floor(Math.random() * 256) - 123;
    @type("uint8") d = 0;
    @type("uint16") score = 0;
}

export class State extends Schema {
    @type({ map: Player }) players = new MapSchema<Player>();
    @type([Vector2Float]) apples = new ArraySchema<Vector2Float>();

    appleLastId = 0;
    
    gameOverIDs = [];

    createApple(){
        const apple = new Vector2Float();
        apple.id = this.appleLastId++;
        this.apples.push(apple);        
    }

    collectApple(player: Player, data: any){
        const apple = this.apples.find((value) => value.id === data.id);

        if(apple === undefined) return;

        apple.x = Math.floor(Math.random() * 256) - 123;
        apple.z = Math.floor(Math.random() * 256) - 123;

        player.score++;
        player.d = 1 + Math.floor(Math.log2(player.score));
    }

    createPlayer(sessionId: string, login) {
        const player = new Player();
        player.login = login;
        this.players.set(sessionId, player);
    }

    removePlayer(sessionId: string) {
        if(this.players.has(sessionId)){
            this.players.delete(sessionId);
        }
    }

    movePlayer (sessionId: string, movement: any) {
        this.players.get(sessionId).x = movement.x;
        this.players.get(sessionId).z = movement.z;        
    }

    gameOver(data){
        const detailsPositions = JSON.parse(data);
        const clientID = detailsPositions.id;

        const gameOverID = this.gameOverIDs.find((value) => value === clientID);
        if(gameOverID !== undefined) return;

        this.gameOverIDs.push(clientID);
        this.delayClearGameOverIDs(clientID);
        
        this.removePlayer(clientID);        
            
        for(let i = 0; i < detailsPositions.ds.length; i++){
            const apple = new Vector2Float();
            apple.id = this.appleLastId++;
            apple.x = detailsPositions.ds[i].x;
            apple.z = detailsPositions.ds[i].z;
            this.apples.push(apple);
        }
    }

    async delayClearGameOverIDs(clientID){
        await new Promise(resolve => setTimeout(resolve, 10000) );

        const index = this.gameOverIDs.findIndex((value) => value === clientID);
        if(index <= -1) return;

        this.gameOverIDs.splice(index,1);
    }
}

export class StateHandlerRoom extends Room<State> {
    maxClients = 20;
    startAppleCount = 100;

    onCreate (options) {
        console.log("StateHandlerRoom created!", options);

        this.setState(new State());

        this.onMessage("move", (client, data) => {
            //console.log("StateHandlerRoom received message from", client.sessionId, ":", data);
            this.state.movePlayer(client.sessionId, data);
        });

        this.onMessage("collect",(client, data) => {
            const player = this.state.players.get(client.sessionId);
            this.state.collectApple(player, data);
        });

        this.onMessage("gameOver",(client,data) => {
            this.state.gameOver(data);
        });

        for(let i = 0; i < this.startAppleCount; i++){
            this.state.createApple();
        } 
    }

    onAuth(client, options, req) {
        return true;
    }

    onJoin (client: Client, data) {
        //client.send("hello", "world");
        this.state.createPlayer(client.sessionId, data.login);
    }

    onLeave (client) {
        this.state.removePlayer(client.sessionId);
    }

    onDispose () {
        console.log("Dispose StateHandlerRoom");
    }
}
