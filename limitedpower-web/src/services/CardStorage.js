class CardStorage {
    db = [];

    constructor() {
    }

    get(set, live) {
        var c = this.db.filter(d => d.isLive == live && d.setCode == set);
        if (c.length == 1) return c[0].cards;
        return [];
    }

    add(set, live, cards) {

        this.db.push({
            "isLive": live,
            "setCode": set,
            "cards": cards
        });
    }
}
export default new CardStorage();