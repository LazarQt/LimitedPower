class CardStorage {
    db = [];

    constructor() {
    }

    get(set, apiCall, live) {
        var c = this.db.filter(d => d.setCode == set && d.apiCall == apiCall && d.isLive == live);
        if (c.length == 1) return c[0].cards;
        return [];
    }

    add(set, apiCall, live, cards) {
        this.db.push({
            "setCode": set,
            "apiCall": apiCall,
            "isLive": live,
            "cards": cards
        });
    }
}
export default new CardStorage();