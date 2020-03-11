export class NpcService {
    constructor(uri) {
        this.uri = uri;
    }

    createCharacter() { // TODO parameters
        return fetch(`${this.uri}/create`, {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                Name: 'Qudp',
                Level: 1
            })
        }).then(r => r.json());
    }
}