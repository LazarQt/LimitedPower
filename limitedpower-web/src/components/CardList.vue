<template>
  <div class="hello">
    <div class="container">
      <button v-on:click="ToggleLive()">
        Showing: {{ this.showLiveData ? "Live Ratings" : "Initial Ratings" }}
      </button>
      <div v-for="(bundle, index) in this.cardBatches" :key="index" class="row">
        <div
          v-for="card in bundle"
          :key="card.ArenaId"
          class="column column-20"
        >
          <img
            :style="card.cardFaces.length > 1 ? 'cursor:pointer;' : ''"
            v-on:click="FlipCard(card)"
            :src="
              card.showBack && card.cardFaces.length > 1
                ? card.pathBack
                : card.pathFront
            "
          />
          <div class="cardTextBox">
            <small>{{ card.name }}</small>
            <br />
            <strong v-if="showLiveData">{{ card.liveGrade }}</strong>
            <strong v-else>{{ card.initialGrade }}</strong>

            (<span v-if="showLiveData">{{ card.liveRating }}</span>
            <span v-else>{{ card.initialRating }}</span>)
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import CardStorage from "@/services/CardStorage.js";

export default {
  name: "CardList",
  props: {
    setCode: String,
    apiCall: String,
  },
  components: {},
  watch: {
    $route() {
      this.Load(true);
    },
  },
  methods: {
    GetCardBatches: function (isLive) {
      this.showLiveData = isLive;
      var storageCards = CardStorage.get(this.setCode, isLive);

      var cards = [];
      var pkg = [];
      for (var i = 0; i < storageCards.length; i++) {
        pkg.push(storageCards[i]);
        if (pkg.length >= 5) {
          cards.push(pkg);
          pkg = [];
        }
      }
      if (pkg.length > 0) {
        cards.push(pkg);
      }
      this.cardBatches = cards;
    },
    FlipCard: function (card) {
      if (!card.isDFC) return;
      card.showBack = !card.showBack;
    },
    ToggleLive: function () {
      this.showLiveData = !this.showLiveData;
      this.Load(this.showLiveData);
    },
    Load: function (isLive) {
      window.console.log("i am created");

      var c = CardStorage.get(this.setCode, isLive);
      console.log(c);
      if (c.length > 0) {
        this.GetCardBatches(isLive);
        return;
      }

      fetch(
        "http://localhost:53517/" +
          this.apiCall +
          "/" +
          this.setCode +
          "?live=" +
          isLive.toString()
      )
        .then((response) => response.json())
        .then((data) => {
          console.log("called web api");
          data.forEach((i) => {
            i.showBack = false;
            i.pathFront = require(`@/assets/img/set/${i.setCode}/${i.arenaId}-0.jpg`);
            if (i.cardFaces.length > 1) {
              i.pathBack = require(`@/assets/img/set/${i.setCode}/${i.arenaId}-1.jpg`);
            }
          });

          CardStorage.add(this.setCode, isLive, data);
          this.GetCardBatches(isLive);
        });
    },
  },
  data: function () {
    return {
      cardStorage: {},
      showLiveData: true,
      cardBatches: [],
    };
  },
  created: function () {
    this.Load(true);
  },
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
.cardTextBox {
  margin-top: -10px;
  margin-bottom: 15px;
}
</style>
