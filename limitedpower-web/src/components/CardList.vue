<template>
  <div class="container">
    <div class="row">
      <div style="position: fixed; right: 10px; bottom: 10px">
        <button v-on:click="ToggleLive()" style="border-radius: 44px">
          Showing: {{ this.showLiveData ? "Live Ratings" : "Initial Ratings" }}
        </button>
      </div>
      <div v-for="(bundle, index) in this.cardBatches" :key="index" class="row">
        <div
          v-for="card in bundle"
          :key="card.ArenaId"
          class="col-xs-12 col-sm-4 col-md col-lg"
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
            <span v-else>{{ card.initialRating }}</span
            >)
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import CardStorage from "@/services/CardStorage.js";
import jsonCfg from "@/assets/config.json";

export default {
  name: "CardList",
  props: {
    setCode: String,
    apiCall: String,
    callParams: String,
  },
  components: {},
  watch: {
    $route() {
      this.Load(true);
    },
  },
  methods: {
    UpdateCardBatches: function (isLive) {
      this.showLiveData = isLive;
      var storageCards = CardStorage.get(this.setCode, this.apiCall, isLive);
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
      // load cached card
      var c = CardStorage.get(this.setCode, this.apiCall, isLive);
      if (c.length > 0) {
        this.UpdateCardBatches(isLive);
        return;
      }

      // fetch data
      fetch(
        "https://lpconfig.azurewebsites.net/" +
          this.apiCall +
          "/" +
          this.setCode +
          "?live=" +
          isLive.toString() +
          "&callParams=" +
          jsonCfg.Sets.filter((x) => x.code == this.setCode)[0].topcommons
      )
        .then((response) => response.json())
        .then((data) => {
          data.forEach((i) => {
            i.showBack = false;
            i.pathFront = require(`@/assets/img/set/${i.setCode}/${i.arenaId}-0.jpg`);

            if (i.layout == "modal_dfc") {
              i.pathBack = require(`@/assets/img/set/${i.setCode}/${i.arenaId}-1.jpg`);
            }
          });

          CardStorage.add(this.setCode, this.apiCall, isLive, data);
          this.UpdateCardBatches(isLive);
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

/* Larger than mobile screen */
@media (min-width: 40rem) {
}

/* Larger than tablet screen */
@media (min-width: 80rem) {
}

/* Larger than desktop screen */
@media (min-width: 120rem) {
}
</style>
