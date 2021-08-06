class CardStorage {
    db = [];

    constructor() {
    }

    get(set, apiCall, live) {
        var c = this.db.filter(d => d.isLive == live && d.setCode == set && d.apiCall == apiCall);
        if (c.length == 1) return c[0].cards;
        return [];
    }

    add(set, apiCall, live, cards) {
        this.db.push({
            "isLive": live,
            "apiCall" : apiCall,
            "setCode": set,
            "cards": cards
        });
    }
}
export default new CardStorage();